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
        [Authorize]
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
                        Data = newCollaboratorId,
                        Message = "New collaborator added to workspace",
                        Success = true
                    };
                }

                return new ErrorModel()
                {
                    Success = false,
                    Error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

        [HttpGet]
        [Authorize]
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
                        Data = allWorkspaces,
                        Message = "Workspaces retrieved",
                        Success = true
                    };
                }

                return new ErrorModel()
                {
                    Success = false,
                    Error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
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
                        Error = "There's no workspace with such an ID",
                        Success = false
                    };
                }

                return new SuccessModel
                {
                    Data = workspace,
                    Message = "Workspace retrieved",
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

        [HttpPost]
        [Authorize]
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
                        Data = workspace,
                        Message = "Workspace created",
                        Success = true
                    };
                }

                return new ErrorModel()
                {
                    Success = false,
                    Error = "Claim userId missing"
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

        [HttpPatch("{workspaceId}")]
        [Authorize]
        public async Task<ActionResult<object>> Update([FromRoute] Guid workspaceId, Workspace workspace)
        {
            var workspaceToUpdate = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

            if (workspaceToUpdate is null)
            {
                return new ErrorModel
                {
                    Error = "There's no workspace with such and ID",
                    Success = false
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
                        Data = workspace,
                        Message = "Workspace updated",
                        Success = true
                    };
                }

                return new ErrorModel()
                {
                    Success = false,
                    Error = "Claim userId missing"
                };

            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }

        [HttpDelete("{workspaceId}")]
        [Authorize]
        public async Task<ActionResult<object>> Delete([FromRoute] Guid workspaceId)
        {
            try
            {
                Workspace workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                if (workspace is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no workspace with such an ID",
                        Success = false
                    };
                }

                _unitOfWork.WorkspaceRepository.Delete(workspace);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = workspace,
                    Message = "Workspace deleted",
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    Error = e.Message,
                    Success = false
                };
            }
        }
    }
}
