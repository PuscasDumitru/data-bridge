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
using Microsoft.AspNetCore.Authorization;
using Teza.Filters;
using Teza.Services;

namespace Teza.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class WorkspaceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public WorkspaceController(RepositoryDbContext context, IAuthService authService)
        {
            _unitOfWork = new UnitOfWork(context);
            _authService = authService;
        }

        [HttpPost("{workspaceId}/User/{newCollaboratorId}")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> AddCollaboratorAsync([FromRoute] Guid workspaceId, [FromRoute] Guid newCollaboratorId)
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

                    if (workspace.Collaborators.Any(x => x.UserId == newCollaboratorId))
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "This user is already a part of this workspace"
                        };
                    }
                    
                    var userResult = await _authService.GetUser(newCollaboratorId);

                    if (userResult.success == false)
                    {
                        return new ErrorModel()
                        {
                            success = false,
                            error = "There's no user with such an ID"
                        };
                    }
                    
                    var collaboratorToAdd = await _unitOfWork.UserRepository.GetUserByIdAsync(newCollaboratorId);

                    if (collaboratorToAdd == null || collaboratorToAdd.WorkspaceId != null)
                    {
                        collaboratorToAdd = new User
                        {
                            UserId = newCollaboratorId,
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

        [HttpDelete("{workspaceId}/User/{collaboratorId}")]
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

        //[HttpGet]
        ////[Authorize]
        //[ServiceFilter(typeof(AuthorizationAttribute))]
        //public async Task<ActionResult<object>> GetWorkspacesByUserIdAsync()
        //{
        //    try
        //    {
        //        if (HttpContext.User.Identity is ClaimsIdentity identity)
        //        {
        //            IEnumerable<Claim> claims = identity.Claims;
        //            var userId = identity.FindFirst("userId").Value;

        //            var allWorkspaces = await _unitOfWork.WorkspaceRepository.GetWorkspacesByUserIdAsync(new Guid(userId));
        //            await _unitOfWork.SaveChangesAsync();

        //            return new SuccessModel
        //            {
        //                data = allWorkspaces,
        //                message = "Workspaces retrieved",
        //                success = true
        //            };
        //        }

        //        return new ErrorModel()
        //        {
        //            success = false,
        //            error = "Claim userId missing"
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        return new ErrorModel
        //        {
        //            error = e.Message,
        //            success = false
        //        };
        //    }
        //}

        [HttpGet("{workspaceId}")]
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

        [HttpPost]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> Create(Workspace workspace)
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var userId = identity.FindFirst("userId").Value;
                    workspace.UserId = new Guid(userId);

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

        [HttpPatch("{workspaceId}")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> Update([FromRoute] Guid workspaceId, [FromBody] Workspace workspace)
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

        [HttpDelete("{workspaceId}")]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> Delete([FromRoute] Guid workspaceId)
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
