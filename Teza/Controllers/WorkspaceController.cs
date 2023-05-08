using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Entities;
using Data.Repositories.Implementation;
using Data.Repositories.Interfaces;
using Teza.Models;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Data.DTOs;
using Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using Teza.Extensions;
using Teza.Filters;
using Teza.Services;
using System.Linq.Expressions;

namespace Teza.Controllers
{
    [ApiController]
    [Route("api")]
    public class WorkspaceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IMailingService _mailingService;

        public WorkspaceController(RepositoryDbContext context, IAuthService authService, IMailingService mailingService)
        {
            _unitOfWork = new UnitOfWork(context);
            _authService = authService;
            _mailingService = mailingService;
        }

        [HttpGet("confirm-email/{workspaceId}/{email}/{confirmationToken}")]
        public async Task<ActionResult<object>> ConfirmEmail([FromRoute] Guid workspaceId, [FromRoute] string email, [FromRoute] string confirmationToken)
        {
            try
            {
                var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);
                var confirmation =
                    await _unitOfWork.UserEmailConfirmationRepository.GetEmailConfirmationByTokenAsync(
                        confirmationToken);

                if (confirmation != null && email == confirmation.Email)
                {
                    if (confirmation.IsEmailConfirmed)
                    {
                        return new ErrorModel
                        {
                            error = "The invitation has already been accepted.",
                            success = false
                        };
                    }

                    if (confirmation.ValidTo < DateTime.UtcNow)
                    {
                        return new ErrorModel
                        {
                            error = "The confirmation link has expired.",
                            success = false
                        };
                    }

                    var collaboratorToAdd = new User
                    {
                        Email = email,
                        WorkspaceId = workspace.Id,
                        Role = Role.Collaborator
                    };

                    confirmation.IsEmailConfirmed = true;

                    _unitOfWork.UserEmailConfirmationRepository.Update(confirmation);
                    _unitOfWork.UserRepository.Create(collaboratorToAdd);

                    workspace.Collaborators.Add(collaboratorToAdd);

                    var activityHistory = new ActivityHistory
                    {
                        EntityName = email,
                        EntityType = EntityType.user,
                        UserName = email,
                        ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                        Action = Data.Enums.Action.joined,
                        WorkspaceId = workspaceId
                    };

                    _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                    workspace.ActivityHistories.Add(activityHistory);

                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = null,
                        message = $"{email} has been added to the workspace {workspace.Name} successfully!",
                        success = true
                    };
                }

                return new ErrorModel
                {
                    error = "Invalid token for confirmation.",
                    success = false
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }

        }

        [HttpPost("workspace/{workspaceId}/collaborator/{email}")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> AddCollaboratorAsync([FromRoute] Guid workspaceId, [FromRoute] string email)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var userId = identity.FindFirst("userId").Value;
                    var userGuid = new Guid(userId);
                    var userEmail = identity.FindFirst("userEmail").Value;

                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace == null)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no workspace with such an ID"
                        };
                    }

                    if (!UserExistsInWorkspace(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not a part of this workspace."
                        };
                    }

                    if (!UserIsAdmin(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not an admin to do this action."
                        };
                    }

                    if (workspace.Collaborators.Any(x => x.Email == email))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "This user is already a part of this workspace"
                        };
                    }

                    var userResult = await _authService.GetUser(email);

                    if (userResult.success == false)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no user with such an email"
                        };
                    }

                    var inviteIsSent = _mailingService.SendMail(email, workspace.Name, workspaceId, _unitOfWork.UserEmailConfirmationRepository);

                    if (inviteIsSent)
                    {
                        var activityHistory = new ActivityHistory
                        {
                            EntityName = email,
                            EntityType = EntityType.user,
                            UserName = userEmail,
                            ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                            Action = Data.Enums.Action.invited,
                            WorkspaceId = workspaceId
                        };

                        _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                        workspace.ActivityHistories.Add(activityHistory);

                        await _unitOfWork.SaveChangesAsync();

                        return new SuccessModel
                        {
                            data = null,
                            message = $"The invitation to join the workspace {workspace.Name} has been sent to {email}",
                            success = true
                        };
                    }

                    return new ErrorModel()
                    {
                        success = false,
                        error = "Something went wrong with sending the invitation."
                    };

                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }

        [HttpPost("workspace/{workspaceId}/collaborator/{email}/update-role")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> UpdateRoleCollaboratorAsync([FromRoute] Guid workspaceId, [FromRoute] string email,
                                                                                        UpdateCollaboratorRoleDto stringRole)
        {
            try
            {
                var role = (Role?)Enum.Parse(typeof(Role), stringRole.Role);

                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var userEmail = identity.FindFirst("userEmail").Value;

                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace == null)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no workspace with such an ID"
                        };
                    }


                    if (!UserExistsInWorkspace(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not a part of this workspace."
                        };
                    }

                    if (!UserIsAdmin(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not an admin to do this action."
                        };
                    }

                    if (!workspace.Collaborators.Any(x => x.Email == email))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "This workspace doesn't have such a user"
                        };
                    }

                    var collaborator = workspace.Collaborators
                        .FirstOrDefault(x => x.Email == email && x.WorkspaceId == workspace.Id);

                    if (collaborator.Role == role)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "This collaborator already has this role"
                        };
                    }

                    if (collaborator.Role == Role.Admin && role != Role.Admin &&
                        workspace.Collaborators.Count(x => x.Role == Role.Admin) == 1)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "You cannot leave a workspace without an admin"
                        };
                    }

                    collaborator.Role = role;
                    _unitOfWork.UserRepository.Update(collaborator);

                    var activityHistory = new ActivityHistory
                    {
                        EntityName = email,
                        EntityType = EntityType.user,
                        UserName = userEmail,
                        ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                        Action = Data.Enums.Action.edited,
                        WorkspaceId = workspaceId
                    };

                    _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                    workspace.ActivityHistories.Add(activityHistory);

                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = collaborator,
                        message = "Collaborator role updated",
                        success = true
                    };
                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return ExceptionHandlerExtension.HandleException(e);
            }
        }

        [HttpDelete("workspace/{workspaceId}/collaborator/{email}")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> RemoveCollaboratorAsync([FromRoute] Guid workspaceId, [FromRoute] string email)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var userEmail = identity.FindFirst("userEmail").Value;

                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace == null)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no workspace with such an ID"
                        };
                    }


                    if (!UserExistsInWorkspace(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not a part of this workspace."
                        };
                    }

                    if (!UserIsAdmin(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not an admin to do this action."
                        };
                    }

                    if (!workspace.Collaborators.Any(x => x.Email == email))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "This workspace doesn't have such a user"
                        };
                    }

                    var collaboratorToRemove = workspace.Collaborators
                        .FirstOrDefault(x => x.Email == email && x.WorkspaceId == workspace.Id);

                    if (workspace.Collaborators.Count(x => x.Role == Role.Admin) == 1 && userEmail == email)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "You're the only admin left, please give admin rights to at least one user before leaving the workspace."
                        };
                    }

                    _unitOfWork.UserRepository.Delete(collaboratorToRemove);
                    workspace.Collaborators.Remove(collaboratorToRemove);

                    var activityHistory = new ActivityHistory
                    {
                        EntityName = email,
                        EntityType = EntityType.user,
                        UserName = userEmail,
                        ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                        Action = Data.Enums.Action.removed,
                        WorkspaceId = workspaceId
                    };

                    _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                    workspace.ActivityHistories.Add(activityHistory);

                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = collaboratorToRemove,
                        message = "Collaborator removed from workspace",
                        success = true
                    };
                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return ExceptionHandlerExtension.HandleException(e);
            }
        }

        [HttpGet("workspace/{workspaceId}/activityHistory")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> GetActivityHistoryByWorkspaceIdAsync([FromRoute] Guid workspaceId, string collaborator, Data.Enums.Action? actionType, EntityType? resource)
        {
            try
            {
                var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                if (workspace is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no workspace with such an ID",
                        success = false
                    };
                }

                var activityHistory = _unitOfWork.ActivityHistoryRepository.GetByCondition(x => x.WorkspaceId == workspaceId);

                if (collaborator != null)
                {
                    activityHistory = activityHistory.Where(x => x.UserName == collaborator);
                }

                if (actionType != null)
                {
                    activityHistory = activityHistory.Where(x => x.Action == actionType);
                }

                if (resource != null)
                {
                    activityHistory = activityHistory.Where(x => x.EntityType == resource);
                }

                return new SuccessModel
                {
                    data = activityHistory.ToList(),
                    message = $"Activity history of the workspace {workspace.Name} has been retrieved",
                    success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }

        [HttpGet("workspaces")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> GetWorkspacesByUserAsync()
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var email = identity.FindFirst("userEmail").Value;

                    var allUserWorkspaces = await _unitOfWork.WorkspaceRepository.GetWorkspacesByEmailAsync(email);

                    return new SuccessModel
                    {
                        data = allUserWorkspaces,
                        message = "User workspaces retrieved",
                        success = true
                    };
                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }

        [HttpGet("workspace/{workspaceId}")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> GetWorkspaceByIdAsync([FromRoute] Guid workspaceId)
        {
            try
            {
                var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                if (workspace is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no workspace with such an ID",
                        success = false
                    };
                }

                return new SuccessModel
                {
                    data = workspace,
                    message = "Workspace retrieved",
                    success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }

        [HttpPost("workspace/{workspaceId}/create-resource")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> Create([FromRoute] Guid workspaceId, object createResourceDto)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var userEmail = identity.FindFirst("userEmail").Value;

                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no workspace with such an ID",
                            success = false
                        };
                    }

                    if (!UserExistsInWorkspace(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not a part of this workspace."
                        };
                    }

                    var typeIdentifier = JsonConvert.DeserializeObject<ResourceDto>(createResourceDto.ToString());

                    if (Enum.GetName(typeof(EntityType), typeIdentifier.Type) is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no such type of entity.",
                            success = false
                        };
                    }

                    switch (typeIdentifier.Type)
                    {
                        case EntityType.collection:
                            var collection = JsonConvert.DeserializeObject<Collection>(createResourceDto.ToString());

                            if (!await IsCollectionUnique(workspaceId, collection))
                            {
                                return new ErrorModel
                                {
                                    error = "There's already a collection with such a name in this workspace.",
                                    success = false
                                };
                            }

                            collection.WorkspaceId = workspaceId;

                            _unitOfWork.CollectionRepository.Create(collection);
                            workspace.Collections.Add(collection);

                            var activityHistory = new ActivityHistory
                            {
                                EntityName = collection.Name,
                                EntityType = EntityType.collection,
                                UserName = userEmail,
                                ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                                Action = Data.Enums.Action.created,
                                WorkspaceId = workspaceId
                            };

                            _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                            workspace.ActivityHistories.Add(activityHistory);

                            await _unitOfWork.SaveChangesAsync();
                            break;

                        case EntityType.folder:
                            var folder = JsonConvert.DeserializeObject<Folder>(createResourceDto.ToString());

                            var existingCollection =
                                workspace.Collections.FirstOrDefault(col => col.Id == folder.CollectionId);

                            if (existingCollection is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no collection with such an ID",
                                    success = false
                                };
                            }

                            if (!IsFolderUnique(existingCollection, folder.Name, new Guid()))
                            {
                                return new ErrorModel
                                {
                                    error = "There's already a folder with such a name in this collection.",
                                    success = false
                                };
                            }

                            _unitOfWork.FolderRepository.Create(folder);
                            existingCollection.Folders.Add(folder);

                            activityHistory = new ActivityHistory
                            {
                                EntityName = folder.Name,
                                EntityType = EntityType.folder,
                                UserName = userEmail,
                                ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                                Action = Data.Enums.Action.created,
                                WorkspaceId = workspaceId
                            };

                            _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                            workspace.ActivityHistories.Add(activityHistory);

                            await _unitOfWork.SaveChangesAsync();
                            break;

                        case EntityType.query:
                            var query = JsonConvert.DeserializeObject<Query>(createResourceDto.ToString());

                            var coll =
                                workspace.Collections.FirstOrDefault(col => col.Id == query.CollectionId);

                            if (coll is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no collection with such an ID",
                                    success = false
                                };
                            }

                            var folderContainingThisQuery =
                                coll.Folders.FirstOrDefault(fol => fol.Id == query.FolderId);

                            if (folderContainingThisQuery is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no folder with such an ID",
                                    success = false
                                };
                            }

                            if (!IsQueryUnique(folderContainingThisQuery, query.Name, new Guid()))
                            {
                                return new ErrorModel
                                {
                                    error = "There's already a query with such a name in this folder.",
                                    success = false
                                };
                            }

                            _unitOfWork.QueryRepository.Create(query);
                            folderContainingThisQuery.Queries.Add(query);

                            activityHistory = new ActivityHistory
                            {
                                EntityName = query.Name,
                                EntityType = EntityType.query,
                                UserName = userEmail,
                                ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                                Action = Data.Enums.Action.created,
                                WorkspaceId = workspaceId
                            };

                            _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                            workspace.ActivityHistories.Add(activityHistory);

                            await _unitOfWork.SaveChangesAsync();
                            break;
                    }

                    return new SuccessModel
                    {
                        message = "Resource created",
                        success = true
                    };
                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };

            }
            catch (Exception e)
            {
                return ExceptionHandlerExtension.HandleException(e);
            }
        }

        [HttpPost("workspace/{workspaceId}/rename-resource")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> Rename([FromRoute] Guid workspaceId, object renameResourceDto)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var userEmail = identity.FindFirst("userEmail").Value;

                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no workspace with such an ID",
                            success = false
                        };
                    }

                    if (!UserExistsInWorkspace(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not a part of this workspace."
                        };
                    }

                    var typeIdentifier = JsonConvert.DeserializeObject<ResourceDto>(renameResourceDto.ToString());

                    if (Enum.GetName(typeof(EntityType), typeIdentifier.Type) is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no such type of entity.",
                            success = false
                        };
                    }

                    switch (typeIdentifier.Type)
                    {
                        case EntityType.collection:
                            var collection = JsonConvert.DeserializeObject<Collection>(renameResourceDto.ToString());

                            var existingCollection =
                                workspace.Collections.FirstOrDefault(col => col.Id == collection.Id);

                            if (existingCollection is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no collection with such an ID",
                                    success = false
                                };
                            }

                            if (!await IsCollectionUnique(workspaceId, collection))
                            {
                                return new ErrorModel
                                {
                                    error = "There's already a collection with such a name in this workspace.",
                                    success = false
                                };
                            }

                            existingCollection.Name = collection.Name;
                            _unitOfWork.CollectionRepository.Update(existingCollection);

                            var activityHistory = new ActivityHistory
                            {
                                EntityName = collection.Name,
                                EntityType = EntityType.collection,
                                UserName = userEmail,
                                ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                                Action = Data.Enums.Action.renamed,
                                WorkspaceId = workspaceId
                            };

                            _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                            workspace.ActivityHistories.Add(activityHistory);

                            await _unitOfWork.SaveChangesAsync();
                            break;

                        case EntityType.folder:
                            var folder = JsonConvert.DeserializeObject<Folder>(renameResourceDto.ToString());

                            var collectionWithFolder =
                                workspace.Collections.FirstOrDefault(col => col.Id == folder.CollectionId);

                            if (collectionWithFolder is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no collection with such an ID",
                                    success = false
                                };
                            }

                            var existingFolder =
                                collectionWithFolder.Folders.FirstOrDefault(fol => fol.Id == folder.Id);

                            if (existingFolder is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no folder with such an ID",
                                    success = false
                                };
                            }

                            if (!IsFolderUnique(collectionWithFolder, folder.Name, (Guid)folder.Id))
                            {
                                return new ErrorModel
                                {
                                    error = "There's already a folder with such a name in this collection.",
                                    success = false
                                };
                            }

                            existingFolder.Name = folder.Name;
                            _unitOfWork.FolderRepository.Update(existingFolder);

                            activityHistory = new ActivityHistory
                            {
                                EntityName = folder.Name,
                                EntityType = EntityType.folder,
                                UserName = userEmail,
                                ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                                Action = Data.Enums.Action.renamed,
                                WorkspaceId = workspaceId
                            };

                            _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                            workspace.ActivityHistories.Add(activityHistory);

                            await _unitOfWork.SaveChangesAsync();
                            break;

                        case EntityType.query:
                            var query = JsonConvert.DeserializeObject<Query>(renameResourceDto.ToString());

                            var collectionFolder =
                                workspace.Collections.FirstOrDefault(col => col.Id == query.CollectionId);

                            if (collectionFolder is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no collection with such an ID",
                                    success = false
                                };
                            }

                            var folderContainingThisQuery =
                                collectionFolder.Folders.FirstOrDefault(fol => fol.Id == query.FolderId);

                            if (folderContainingThisQuery is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no folder with such an ID",
                                    success = false
                                };
                            }

                            var existingQuery =
                                folderContainingThisQuery.Queries.FirstOrDefault(qry => qry.Id == query.Id);

                            if (existingQuery is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no query with such an ID",
                                    success = false
                                };
                            }

                            if (!IsQueryUnique(folderContainingThisQuery, query.Name, (Guid)query.Id))
                            {
                                return new ErrorModel
                                {
                                    error = "There's already a query with such a name in this folder.",
                                    success = false
                                };
                            }

                            existingQuery.Name = query.Name;
                            _unitOfWork.QueryRepository.Update(existingQuery);

                            activityHistory = new ActivityHistory
                            {
                                EntityName = query.Name,
                                EntityType = EntityType.query,
                                UserName = userEmail,
                                ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                                Action = Data.Enums.Action.renamed,
                                WorkspaceId = workspaceId
                            };

                            _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                            workspace.ActivityHistories.Add(activityHistory);

                            await _unitOfWork.SaveChangesAsync();
                            break;
                    }

                    return new SuccessModel
                    {
                        message = "Resource renamed",
                        success = true
                    };
                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return ExceptionHandlerExtension.HandleException(e);
            }
        }

        [HttpPost("workspace/{workspaceId}/delete-resource")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> Delete([FromRoute] Guid workspaceId, object renameResourceDto)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var userEmail = identity.FindFirst("userEmail").Value;

                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no workspace with such an ID",
                            success = false
                        };
                    }

                    if (!UserExistsInWorkspace(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not a part of this workspace."
                        };
                    }

                    var typeIdentifier = JsonConvert.DeserializeObject<ResourceDto>(renameResourceDto.ToString());

                    if (Enum.GetName(typeof(EntityType), typeIdentifier.Type) is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no such type of entity.",
                            success = false
                        };
                    }

                    switch (typeIdentifier.Type)
                    {
                        case EntityType.collection:
                            var collection = JsonConvert.DeserializeObject<Collection>(renameResourceDto.ToString());

                            var existingCollection =
                                workspace.Collections.FirstOrDefault(col => col.Id == collection.Id);

                            if (existingCollection is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no collection with such an ID",
                                    success = false
                                };
                            }

                            _unitOfWork.CollectionRepository.Delete(existingCollection);
                            workspace.Collections.Remove(existingCollection);

                            var activityHistory = new ActivityHistory
                            {
                                EntityName = collection.Name,
                                EntityType = EntityType.collection,
                                UserName = userEmail,
                                ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                                Action = Data.Enums.Action.removed,
                                WorkspaceId = workspaceId
                            };

                            _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                            workspace.ActivityHistories.Add(activityHistory);

                            await _unitOfWork.SaveChangesAsync();
                            break;

                        case EntityType.folder:
                            var folder = JsonConvert.DeserializeObject<Folder>(renameResourceDto.ToString());

                            var collectionWithFolder =
                                workspace.Collections.FirstOrDefault(col => col.Id == folder.CollectionId);

                            if (collectionWithFolder is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no collection with such an ID",
                                    success = false
                                };
                            }

                            var existingFolder =
                                collectionWithFolder.Folders.FirstOrDefault(fol => fol.Id == folder.Id);

                            if (existingFolder is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no folder with such an ID",
                                    success = false
                                };
                            }

                            _unitOfWork.FolderRepository.Delete(existingFolder);
                            collectionWithFolder.Folders.Remove(existingFolder);

                            activityHistory = new ActivityHistory
                            {
                                EntityName = folder.Name,
                                EntityType = EntityType.folder,
                                UserName = userEmail,
                                ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                                Action = Data.Enums.Action.removed,
                                WorkspaceId = workspaceId
                            };

                            _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                            workspace.ActivityHistories.Add(activityHistory);

                            await _unitOfWork.SaveChangesAsync();
                            break;

                        case EntityType.query:
                            var query = JsonConvert.DeserializeObject<Query>(renameResourceDto.ToString());

                            var collectionFolder =
                                workspace.Collections.FirstOrDefault(col => col.Id == query.CollectionId);

                            if (collectionFolder is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no collection with such an ID",
                                    success = false
                                };
                            }

                            var folderContainingThisQuery =
                                collectionFolder.Folders.FirstOrDefault(fol => fol.Id == query.FolderId);

                            if (folderContainingThisQuery is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no folder with such an ID",
                                    success = false
                                };
                            }

                            var existingQuery =
                                folderContainingThisQuery.Queries.FirstOrDefault(qry => qry.Id == query.Id);

                            if (existingQuery is null)
                            {
                                return new ErrorModel
                                {
                                    error = "There's no query with such an ID",
                                    success = false
                                };
                            }

                            _unitOfWork.QueryRepository.Delete(existingQuery);
                            folderContainingThisQuery.Queries.Remove(existingQuery);

                            activityHistory = new ActivityHistory
                            {
                                EntityName = query.Name,
                                EntityType = EntityType.query,
                                UserName = userEmail,
                                ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                                Action = Data.Enums.Action.removed,
                                WorkspaceId = workspaceId
                            };

                            _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                            workspace.ActivityHistories.Add(activityHistory);

                            await _unitOfWork.SaveChangesAsync();
                            break;
                    }

                    return new SuccessModel
                    {
                        message = "Resource deleted",
                        success = true
                    };
                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return ExceptionHandlerExtension.HandleException(e);
            }
        }

        [HttpPatch("workspace/{workspaceId}/query/{queryId}")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> UpdateQuery([FromRoute] Guid workspaceId, [FromRoute] Guid queryId, [FromBody] Query query)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var userEmail = identity.FindFirst("userEmail").Value;

                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no workspace with such an ID",
                            success = false
                        };
                    }

                    if (!UserExistsInWorkspace(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not a part of this workspace."
                        };
                    }

                    var collectionFolder =
                        workspace.Collections.FirstOrDefault(col => col.Id == query.CollectionId);

                    if (collectionFolder is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no collection with such an ID",
                            success = false
                        };
                    }

                    var folderContainingThisQuery =
                        collectionFolder.Folders.FirstOrDefault(fol => fol.Id == query.FolderId);

                    if (folderContainingThisQuery is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no folder with such an ID",
                            success = false
                        };
                    }

                    var existingQuery = await _unitOfWork.QueryRepository.GetQueryByIdAsync(queryId);

                    if (existingQuery is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no query with such an ID",
                            success = false
                        };
                    }

                    if (!IsQueryUnique(folderContainingThisQuery, query.Name, queryId))
                    {
                        return new ErrorModel
                        {
                            error = "There's already a query with such a name in this folder.",
                            success = false
                        };
                    }

                    var updatedQuery = _unitOfWork.QueryRepository.UpdateEntity(existingQuery, query);
                    _unitOfWork.QueryRepository.Update(updatedQuery);

                    var activityHistory = new ActivityHistory
                    {
                        EntityName = query.Name,
                        EntityType = EntityType.query,
                        UserName = userEmail,
                        ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                        Action = Data.Enums.Action.edited,
                        WorkspaceId = workspaceId
                    };

                    _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                    workspace.ActivityHistories.Add(activityHistory);

                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = updatedQuery,
                        message = "Query updated",
                        success = true
                    };
                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return ExceptionHandlerExtension.HandleException(e);
            }
        }

        [HttpPost("workspace")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> CreateWorkspace([FromBody] Workspace workspace)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var userId = identity.FindFirst("userId").Value;
                    var userGuid = new Guid(userId);
                    var email = identity.FindFirst("userEmail").Value;

                    if (!await IsWorkspaceUnique(userGuid, email, workspace))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's already a workspace with such a name created by this user."
                        };
                    }

                    var userResult = await _authService.GetUser(email);

                    if (userResult.success == false)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no user with such an ID"
                        };
                    }

                    workspace.UserId = userGuid;
                    var collaboratorToAdd = new User
                    {
                        UserId = userGuid,
                        Email = email,
                        WorkspaceId = workspace.Id,
                        Role = Role.Admin
                    };

                    _unitOfWork.UserRepository.Create(collaboratorToAdd);
                    workspace.Collaborators.Add(collaboratorToAdd);

                    _unitOfWork.WorkspaceRepository.Create(workspace);

                    var activityHistory = new ActivityHistory
                    {
                        EntityName = workspace.Name,
                        EntityType = EntityType.workspace,
                        UserName = email,
                        ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                        Action = Data.Enums.Action.created,
                        WorkspaceId = workspace.Id
                    };

                    _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                    workspace.ActivityHistories.Add(activityHistory);

                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = workspace,
                        message = "Workspace created",
                        success = true
                    };
                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return ExceptionHandlerExtension.HandleException(e);
            }
        }

        [HttpPatch("workspace/{workspaceId}")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> UpdateWorkspace([FromRoute] Guid workspaceId, [FromBody] Workspace workspace)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var userId = identity.FindFirst("userId").Value;
                    var userGuid = new Guid(userId);
                    var userEmail = identity.FindFirst("userEmail").Value;

                    var workspaceToUpdate = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspaceToUpdate is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no workspace with such an ID",
                            success = false
                        };
                    }

                    workspace.Id = workspaceId;

                    if (!UserExistsInWorkspace(workspaceToUpdate, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not a part of this workspace."
                        };
                    }


                    if (!UserIsAdmin(workspaceToUpdate, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not an admin to do this action."
                        };
                    }

                    if (!await IsWorkspaceUnique(userGuid, userEmail, workspace))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's already a workspace with such a name created by this user."
                        };
                    }

                    var updatedWorkspace = _unitOfWork.WorkspaceRepository.UpdateEntity(workspaceToUpdate, workspace);

                    _unitOfWork.WorkspaceRepository.Update(updatedWorkspace);

                    var activityHistory = new ActivityHistory
                    {
                        EntityName = workspace.Name,
                        EntityType = EntityType.workspace,
                        UserName = userEmail,
                        ActionPerformedTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                        Action = Data.Enums.Action.edited,
                        WorkspaceId = workspaceId
                    };

                    _unitOfWork.ActivityHistoryRepository.Create(activityHistory);
                    workspace.ActivityHistories.Add(activityHistory);

                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = updatedWorkspace,
                        message = "Workspace updated",
                        success = true
                    };
                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };

            }
            catch (Exception e)
            {
                return ExceptionHandlerExtension.HandleException(e);
            }
        }

        [HttpDelete("workspace/{workspaceId}")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> DeleteWorkspace([FromRoute] Guid workspaceId)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var userEmail = identity.FindFirst("userEmail").Value;

                    Workspace workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace is null)
                    {
                        return new ErrorModel
                        {
                            error = "There's no workspace with such an ID",
                            success = false
                        };
                    }

                    if (!UserExistsInWorkspace(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not a part of this workspace."
                        };
                    }


                    if (!UserIsAdmin(workspace, userEmail))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "The logged user is not an admin to do this action."
                        };
                    }

                    _unitOfWork.WorkspaceRepository.Delete(workspace);
                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = workspace,
                        message = "Workspace deleted",
                        success = true
                    };
                }

                return new ErrorModel()
                {
                    success = false,
                    error = "Claim userId missing"
                };

            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool UserExistsInWorkspace(Workspace workspace, string email)
        {
            if (workspace.Collaborators.Any(user => user.Email == email))
            {
                return true;
            }

            return false;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool UserIsAdmin(Workspace workspace, string email)
        {
            if (workspace.Collaborators.FirstOrDefault(user => user.Email == email).Role == Role.Admin)
            {
                return true;
            }

            return false;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> IsWorkspaceUnique(Guid userId, string userEmail, Workspace updatedWorkspace)
        {
            var userWorkspaces = await _unitOfWork.WorkspaceRepository.GetWorkspacesByEmailAsync(userEmail);

            if (userWorkspaces.Any(workspace => workspace.UserId == userId &&
                                                workspace.Name == updatedWorkspace.Name && workspace.Id != updatedWorkspace.Id))
            {
                return false;
            }

            return true;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> IsCollectionUnique(Guid workspaceId, Collection updatedCollection)
        {
            var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

            if (workspace.Collections.Any(collection => collection.Name == updatedCollection.Name && collection.Id != updatedCollection.Id))
            {
                return false;
            }

            return true;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool IsFolderUnique(Collection collection, string folderName, Guid folderId)
        {

            if (collection.Folders.Any(folder => folder.Name == folderName && folder.Id != folderId))
            {
                return false;
            }

            return true;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool IsQueryUnique(Folder folder, string queryName, Guid queryId)
        {

            if (folder.Queries.Any(query => query.Name == queryName && query.Id != queryId))
            {
                return false;
            }

            return true;
        }
    }
}
