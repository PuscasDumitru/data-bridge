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
    public class QueryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public QueryController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet("Workspace/{workspaceId}/Collection/{collectionId}/Folder/{folderId}/Query")]
        public async Task<ActionResult<object>> GetQueriesByFolderIdAsync([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId, 
                                                                                [FromRoute] Guid folderId)
        {
            try
            {
                var folder = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folder is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no folder with such ID",
                        success = false
                    };
                }

                if (!folder.CollectionId.Equals(collectionId))
                {
                    return new ErrorModel
                    {
                        error = "Wrong collection ID",
                        success = false
                    };
                }

                return new SuccessModel
                {
                    data = folder.Queries,
                    message = "Queries retrieved",
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

        [HttpGet("Workspace/{workspaceId}/Collection/{collectionId}/Folder/{folderId}/Query/{queryId}")]
        public async Task<ActionResult<object>> GetQueryByIdAsync([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId,
                                                                       [FromRoute] Guid folderId, [FromRoute] Guid queryId)
        {
            try
            {
                var query = await _unitOfWork.QueryRepository.GetQueryByIdAsync(queryId);

                if (query is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no query with such an ID",
                        success = false
                    };
                }

                return new SuccessModel
                {
                    data = query,
                    message = "Query retrieved",
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

        [HttpPost("Workspace/{workspaceId}/Collection/{collectionId}/Folder/{folderId}/Query")]
        public async Task<ActionResult<object>> Create([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId,
                                                                 [FromRoute] Guid folderId, Query query)
        {
            try
            {
                var folder = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folder is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no folder with such ID",
                        success = false
                    };
                }

                query.FolderId = folderId;
                _unitOfWork.QueryRepository.Create(query);
                folder.Queries.Add(query);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    data = query,
                    message = "Query created",
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

        [HttpPatch("Workspace/{workspaceId}/Collection/{collectionId}/Folder/{folderId}/Query/{queryId}")]
        public async Task<ActionResult<object>> Update([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId,
                                                        [FromRoute] Guid folderId, [FromRoute] Guid queryId, Query query)
        {
            try
            {
                var queryToUpdate = await _unitOfWork.QueryRepository.GetQueryByIdAsync(queryId);

                if (queryToUpdate is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no query with such ID",
                        success = false
                    };
                }

                var folder = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folder is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no folder with such ID",
                        success = false
                    };
                }

                var updatedQuery = _unitOfWork.QueryRepository.UpdateEntity(queryToUpdate, query);
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

        [HttpDelete("Workspace/{workspaceId}/Collection/{collectionId}/Folder/{folderId}/Query/{queryId}")]
        public async Task<ActionResult<object>> Delete([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId,
                                                        [FromRoute] Guid folderId, [FromRoute] Guid queryId)
        {
            try
            {
                var query = await _unitOfWork.QueryRepository.GetQueryByIdAsync(queryId);

                if (query is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no query with such an ID",
                        success = false
                    };
                }

                var folder = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folder is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no folder with such an ID",
                        success = false
                    };
                }

                _unitOfWork.QueryRepository.Delete(query);
                folder.Queries.Remove(query);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    data = query,
                    message = "Query deleted",
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
