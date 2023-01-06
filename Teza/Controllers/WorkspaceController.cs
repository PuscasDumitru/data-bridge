using System;
using System.Collections.Generic;
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
using AutoMapper;
using Data.DTOs;
using Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Teza.Filters;
using Teza.Services;

namespace Teza.Controllers
{
    [ApiController]
    [Route("api")]
    public class WorkspaceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public WorkspaceController(RepositoryDbContext context, IAuthService authService, IMapper mapper)
        {
            _unitOfWork = new UnitOfWork(context);
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("workspace/{workspaceId}/collaborator/{collaboratorId}")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> AddCollaboratorAsync([FromRoute] Guid workspaceId, [FromRoute] Guid collaboratorId)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace == null)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no workspace with such an ID"
                        };
                    }

                    if (workspace.Collaborators.Any(x => x.UserId == collaboratorId))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "This user is already a part of this workspace"
                        };
                    }

                    var userResult = await _authService.GetUser(collaboratorId);

                    if (userResult.success == false)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no user with such an ID"
                        };
                    }

                    var collaboratorToAdd = await _unitOfWork.UserRepository.GetUserByIdAsync(collaboratorId);

                    if (collaboratorToAdd == null || collaboratorToAdd.WorkspaceId != null)
                    {
                        collaboratorToAdd = new User
                        {
                            UserId = collaboratorId,
                            UserName = userResult.data.UserName,
                            WorkspaceId = workspace.Id
                        };
                        _unitOfWork.UserRepository.Create(collaboratorToAdd);
                    }

                    else
                    {
                        collaboratorToAdd.WorkspaceId = workspace.Id;
                        _unitOfWork.UserRepository.Update(collaboratorToAdd);
                    }

                    workspace.Collaborators.Add(collaboratorToAdd);
                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = collaboratorToAdd,
                        message = "New collaborator added to workspace",
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

        [HttpPost("workspace/{workspaceId}/collaborator/{collaboratorId}/update-role")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> UpdateRoleCollaboratorAsync([FromRoute] Guid workspaceId, [FromRoute] Guid collaboratorId,
                                                                                        UpdateCollaboratorRoleDto role)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace == null)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no workspace with such an ID"
                        };
                    }

                    if (!workspace.Collaborators.Any(x => x.UserId == collaboratorId))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "This workspace doesn't have such a user"
                        };
                    }

                    var collaborator = workspace.Collaborators
                        .FirstOrDefault(x => x.UserId == collaboratorId && x.WorkspaceId == workspace.Id);

                    if (collaborator.Role == role.Role)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "This collaborator already has this role"
                        };
                    }

                    collaborator.Role = role.Role;
                    _unitOfWork.UserRepository.Update(collaborator);

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
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }

        [HttpDelete("workspace/{workspaceId}/collaborator/{collaboratorId}")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> RemoveCollaboratorAsync([FromRoute] Guid workspaceId, [FromRoute] Guid collaboratorId)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                    if (workspace == null)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no workspace with such an ID"
                        };
                    }

                    if (!workspace.Collaborators.Any(x => x.UserId == collaboratorId))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "This workspace doesn't have such a user"
                        };
                    }

                    var collaboratorToRemove = workspace.Collaborators
                        .FirstOrDefault(x => x.UserId == collaboratorId && x.WorkspaceId == workspace.Id);

                    _unitOfWork.UserRepository.Delete(collaboratorToRemove);
                    workspace.Collaborators.Remove(collaboratorToRemove);
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
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }

        [HttpGet("workspaces")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> GetWorkspacesByUserIdAsync()
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var userId = new Guid(identity.FindFirst("userId").Value);

                    var allUserWorkspaces = await _unitOfWork.WorkspaceRepository.GetWorkspacesByUserIdAsync(userId);
                    
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
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> Create([FromRoute] Guid workspaceId, object createResourceDto)
        {
            try
            {
                var typeIdentifier = JsonConvert.DeserializeObject<ResourceDto>(createResourceDto.ToString());

                var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                if (workspace is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no workspace with such an ID",
                        success = false
                    };
                }

                switch (typeIdentifier.Type)
                {
                    case EntityType.collection:
                        var collection = JsonConvert.DeserializeObject<Collection>(createResourceDto.ToString());
                        collection.WorkspaceId = workspaceId;

                        _unitOfWork.CollectionRepository.Create(collection);
                        workspace.Collections.Add(collection);
                        await _unitOfWork.SaveChangesAsync();
                        break;

                    case EntityType.folder:
                        var folder = JsonConvert.DeserializeObject<Folder>(createResourceDto.ToString());

                        _unitOfWork.FolderRepository.Create(folder);
                        var collectionContainingThisFolder =
                            workspace.Collections.FirstOrDefault(col => col.Id == folder.CollectionId);
                        collectionContainingThisFolder.Folders.Add(folder);
                        await _unitOfWork.SaveChangesAsync();
                        break;

                    case EntityType.query:
                        var query = JsonConvert.DeserializeObject<Query>(createResourceDto.ToString());

                        var collectionWithFolder =
                            workspace.Collections.FirstOrDefault(col => col.Id == query.CollectionId);

                        var folderContainingThisQuery =
                            collectionWithFolder.Folders.FirstOrDefault(fol => fol.Id == query.FolderId);

                        _unitOfWork.QueryRepository.Create(query);
                        folderContainingThisQuery.Queries.Add(query);
                        await _unitOfWork.SaveChangesAsync();
                        break;
                }

                return new SuccessModel
                {
                    data = createResourceDto,
                    message = "Resource created",
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

        [HttpPost("workspace/{workspaceId}/rename-resource")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> Rename([FromRoute] Guid workspaceId, object renameResourceDto)
        {
            try
            {
                var typeIdentifier = JsonConvert.DeserializeObject<ResourceDto>(renameResourceDto.ToString());

                var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                if (workspace is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no workspace with such an ID",
                        success = false
                    };
                }

                switch (typeIdentifier.Type)
                {
                    case EntityType.collection:
                        var collection = JsonConvert.DeserializeObject<Collection>(renameResourceDto.ToString());

                        var existingCollection = workspace.Collections.FirstOrDefault(col => col.Id == collection.Id);

                        if (existingCollection is null)
                        {
                            return new ErrorModel
                            {
                                error = "There's no collection with such an ID",
                                success = false
                            };
                        }

                        existingCollection.Name = collection.Name;
                        _unitOfWork.CollectionRepository.Update(existingCollection);

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

                        existingFolder.Name = folder.Name;
                        _unitOfWork.FolderRepository.Update(existingFolder);

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

                        var existingQuery = folderContainingThisQuery.Queries.FirstOrDefault(qry => qry.Id == query.Id);

                        if (existingQuery is null)
                        {
                            return new ErrorModel
                            {
                                error = "There's no query with such an ID",
                                success = false
                            };
                        }

                        existingQuery.Name = query.Name;
                        _unitOfWork.QueryRepository.Update(existingQuery);

                        await _unitOfWork.SaveChangesAsync();
                        break;
                }

                return new SuccessModel
                {
                    data = renameResourceDto,
                    message = "Resource updated",
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

        [HttpDelete("workspace/{workspaceId}/delete-resource")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> Delete([FromRoute] Guid workspaceId, object renameResourceDto)
        {
            try
            {
                var typeIdentifier = JsonConvert.DeserializeObject<ResourceDto>(renameResourceDto.ToString());

                var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                if (workspace is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no workspace with such an ID",
                        success = false
                    };
                }

                switch (typeIdentifier.Type)
                {
                    case EntityType.collection:
                        var collection = JsonConvert.DeserializeObject<Collection>(renameResourceDto.ToString());

                        var existingCollection = workspace.Collections.FirstOrDefault(col => col.Id == collection.Id);

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

                        var existingQuery = folderContainingThisQuery.Queries.FirstOrDefault(qry => qry.Id == query.Id);

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

                        await _unitOfWork.SaveChangesAsync();
                        break;
                }

                return new SuccessModel
                {
                    data = renameResourceDto,
                    message = "Resource deleted",
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

        [HttpPatch("workspace/{workspaceId}/query/{queryId}")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> UpdateQuery([FromRoute] Guid workspaceId, [FromRoute] Guid queryId, Query query)
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

                var existingQuery = await _unitOfWork.QueryRepository.GetQueryByIdAsync(queryId); //folderContainingThisQuery.Queries.FirstOrDefault(qry => qry.Id == queryId);

                if (existingQuery is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no query with such an ID",
                        success = false
                    };
                }


                var updatedQuery = _unitOfWork.QueryRepository.UpdateEntity(existingQuery, query);
                _unitOfWork.QueryRepository.Update(updatedQuery);

                await _unitOfWork.SaveChangesAsync();
             
                return new SuccessModel
                {
                    data = updatedQuery,
                    message = "Query updated",
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

        [HttpPost("workspace")]
        //[Authorize]
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
                    
                    var userResult = await _authService.GetUser(userGuid);

                    if (userResult.success == false)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no user with such an ID"
                        };
                    }

                    workspace.UserId = userGuid;
                    var collaboratorToAdd = await _unitOfWork.UserRepository.GetUserByIdAsync(userGuid);

                    if (collaboratorToAdd == null)
                    {
                        collaboratorToAdd = new User
                        {
                            UserId = userGuid,
                            UserName = userResult.data.UserName,
                            WorkspaceId = workspace.Id
                        };
                        _unitOfWork.UserRepository.Create(collaboratorToAdd);
                    }

                    else
                    {
                        _unitOfWork.UserRepository.Update(collaboratorToAdd);
                    }

                    workspace.Collaborators.Add(collaboratorToAdd);

                    _unitOfWork.WorkspaceRepository.Create(workspace);
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
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }

        [HttpPatch("workspace/{workspaceId}")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> UpdateWorkspace([FromRoute] Guid workspaceId, [FromBody] Workspace workspace)
        {
            var workspaceToUpdate = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

            if (workspaceToUpdate is null)
            {
                return new ErrorModel
                {
                    error = "There's no workspace with such an ID",
                    success = false
                };
            }

            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var userId = identity.FindFirst("userId").Value;

                    var updatedWorkspace = _unitOfWork.WorkspaceRepository.UpdateEntity(workspaceToUpdate, workspace);

                    _unitOfWork.WorkspaceRepository.Update(updatedWorkspace);
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
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }

        [HttpDelete("workspace/{workspaceId}")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> DeleteWorspace([FromRoute] Guid workspaceId)
        {
            try
            {
                Workspace workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                if (workspace is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no workspace with such an ID",
                        success = false
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
            catch (Exception e)
            {
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }
    }
}
