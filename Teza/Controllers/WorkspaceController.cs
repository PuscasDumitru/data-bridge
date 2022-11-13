using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.AspNetCore.Authorization;
using Teza.Filters;

namespace Teza.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class WorkspaceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkspaceController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
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
                    IEnumerable<Claim> claims = identity.Claims;
                    var userId = identity.FindFirst("userId").Value;
                    //validate collaborator id

                    var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);
                    workspace.Users += newCollaboratorId + ";";
                    _unitOfWork.WorkspaceRepository.Update(workspace);
                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = newCollaboratorId,
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

        [HttpGet]
        //[Authorize]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public async Task<ActionResult<object>> GetWorkspacesByUserIdAsync()
        {
            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var userId = identity.FindFirst("userId").Value;

                    var allWorkspaces = await _unitOfWork.WorkspaceRepository.GetWorkspacesByUserIdAsync(new Guid(userId));
                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = allWorkspaces,
                        message = "Workspaces retrieved",
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
        public async Task<ActionResult<object>> Update([FromRoute] Guid workspaceId, Workspace workspace)
        {
            var workspaceToUpdate = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

            if (workspaceToUpdate is null)
            {
                return new ErrorModel
                {
                    error = "There's no workspace with such and ID",
                    success = false
                };
            }

            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var userId = identity.FindFirst("userId").Value;
                    workspace.UserId = new Guid(userId);

                    workspace.Id = workspaceToUpdate.Id;
                    _unitOfWork.WorkspaceRepository.Update(workspace);
                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessModel
                    {
                        data = workspace,
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
