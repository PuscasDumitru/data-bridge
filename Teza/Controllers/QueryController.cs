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
                        Error = "There's no folder with such ID",
                        Success = false
                    };
                }

                if (!folder.CollectionId.Equals(collectionId))
                {
                    return new ErrorModel
                    {
                        Error = "Wrong collection ID",
                        Success = false
                    };
                }

                return new SuccessModel
                {
                    Data = folder.Queries,
                    Message = "Queries retrieved",
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
                        Error = "There's no query with such an ID",
                        Success = false
                    };
                }

                return new SuccessModel
                {
                    Data = query,
                    Message = "Query retrieved",
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
                        Error = "There's no folder with such ID",
                        Success = false
                    };
                }

                query.FolderId = folderId;
                _unitOfWork.QueryRepository.Create(query);
                folder.Queries.Add(query);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = query,
                    Message = "Query created",
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
                        Error = "There's no query with such ID",
                        Success = false
                    };
                }

                var folder = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folder is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no folder with such ID",
                        Success = false
                    };
                }

                query.Id = queryId;
                query.FolderId = folderId;
                _unitOfWork.QueryRepository.Update(query);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = query,
                    Message = "Query updated",
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
                        Error = "There's no query with such an ID",
                        Success = false
                    };
                }

                var folder = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folder is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no folder with such an ID",
                        Success = false
                    };
                }

                _unitOfWork.QueryRepository.Delete(query);
                folder.Queries.Remove(query);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = query,
                    Message = "Query deleted",
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
