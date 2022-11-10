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

namespace Teza.Controllers
{
    [ApiController]
    [Route("api")]
    public class CollectionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CollectionController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet("Workspace/{workspaceId}/Collection")]
        public async Task<ActionResult<object>> GetCollectionsByWorkspaceIdAsync([FromRoute] Guid workspaceId)
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

                //var allCollections = await _unitOfWork.CollectionRepository.GetByCondition(x => x.WorkspaceId.Equals(workspaceId)).ToListAsync();

                return new SuccessModel
                {
                    Data = workspace.Collections,
                    Message = "Collections retrieved",
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

        [HttpGet("Workspace/{workspaceId}/Collection/{collectionId}")]
        public async Task<ActionResult<object>> GetCollectionByIdAsync([FromRoute] Guid collectionId, [FromRoute] Guid workspaceId)
        {
            try
            {
                var collection = await _unitOfWork.CollectionRepository.GetCollectionByIdAsync(collectionId);

                if (collection is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no collection with such an ID",
                        Success = false
                    };
                }

                return new SuccessModel
                {
                    Data = collection,
                    Message = "Collection retrieved",
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

        [HttpPost("Workspace/{workspaceId}/Collection")]
        public async Task<ActionResult<object>> Create([FromRoute] Guid workspaceId, Collection collection)
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

                collection.WorkspaceId = workspaceId;
                _unitOfWork.CollectionRepository.Create(collection);

                workspace.Collections.Add(collection);

                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = collection,
                    Message = "Collection created",
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

        [HttpPatch("Workspace/{workspaceId}/Collection/{collectionId}")]
        public async Task<ActionResult<object>> Update([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId, Collection collection)
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

                var collectionToUpdate = await _unitOfWork.CollectionRepository.GetCollectionByIdAsync(collectionId);

                if (collectionToUpdate is null)
                {
                    return new ErrorModel
                    {
                        Error = "Wrong collection ID",
                        Success = false
                    };
                }

                collection.Id = collectionToUpdate.Id;
                collection.WorkspaceId = workspaceId;
                _unitOfWork.CollectionRepository.Update(collection);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = collection,
                    Message = "Collection updated",
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

        [HttpDelete("Workspace/{workspaceId}/Collection/{collectionId}")]
        public async Task<ActionResult<object>> Delete([FromRoute] Guid collectionId, [FromRoute] Guid workspaceId)
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

                var collection = await _unitOfWork.CollectionRepository.GetCollectionByIdAsync(collectionId);

                if (collection is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no collection with such an ID",
                        Success = false
                    };
                }

                _unitOfWork.CollectionRepository.Delete(collection);
                workspace.Collections.Remove(collection);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = collection,
                    Message = "Collection deleted",
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
