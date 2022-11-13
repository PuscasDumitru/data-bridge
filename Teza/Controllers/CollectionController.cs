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
                        error = "There's no workspace with such an ID",
                        success = false
                    };
                }

                //var allCollections = await _unitOfWork.CollectionRepository.GetByCondition(x => x.WorkspaceId.Equals(workspaceId)).ToListAsync();

                return new SuccessModel
                {
                    data = workspace.Collections,
                    message = "Collections retrieved",
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
                        error = "There's no collection with such an ID",
                        success = false
                    };
                }

                return new SuccessModel
                {
                    data = collection,
                    message = "Collection retrieved",
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
                        error = "There's no workspace with such an ID",
                        success = false
                    };
                }

                collection.WorkspaceId = workspaceId;
                _unitOfWork.CollectionRepository.Create(collection);

                workspace.Collections.Add(collection);

                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    data = collection,
                    message = "Collection created",
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
                        error = "There's no workspace with such an ID",
                        success = false
                    };
                }

                var collectionToUpdate = await _unitOfWork.CollectionRepository.GetCollectionByIdAsync(collectionId);

                if (collectionToUpdate is null)
                {
                    return new ErrorModel
                    {
                        error = "Wrong collection ID",
                        success = false
                    };
                }

                collection.Id = collectionToUpdate.Id;
                collection.WorkspaceId = workspaceId;
                _unitOfWork.CollectionRepository.Update(collection);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    data = collection,
                    message = "Collection updated",
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
                        error = "There's no workspace with such an ID",
                        success = false
                    };
                }

                var collection = await _unitOfWork.CollectionRepository.GetCollectionByIdAsync(collectionId);

                if (collection is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no collection with such an ID",
                        success = false
                    };
                }

                _unitOfWork.CollectionRepository.Delete(collection);
                workspace.Collections.Remove(collection);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    data = collection,
                    message = "Collection deleted",
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
