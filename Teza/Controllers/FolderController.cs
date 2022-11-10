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
    public class FolderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FolderController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet("Workspace/{workspaceId}/Collection/{collectionId}/Folder")]
        public async Task<ActionResult<object>> GetFoldersByCollectionId([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId)
        {
            try
            {
                var collection = await _unitOfWork.CollectionRepository.GetCollectionByIdAsync(collectionId);

                if (collection is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no collection with such ID",
                        Success = false
                    };
                }

                if (!workspaceId.Equals(collection.WorkspaceId))
                {
                    return new ErrorModel
                    {
                        Error = "Wrong workspace ID",
                        Success = false
                    };
                }

                return new SuccessModel
                {
                    Data = collection.Folders,
                    Message = "Folders retrieved",
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

        [HttpGet("Workspace/{workspaceId}/Collection/{collectionId}/Folder/{folderId}")]
        public async Task<ActionResult<object>> GetFolderByIdAsync([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId,
                                                                                    [FromRoute] Guid folderId)
        {
            try
            {
                var collection = await _unitOfWork.CollectionRepository.GetCollectionByIdAsync(collectionId);

                if (collection is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no collection with such ID",
                        Success = false
                    };
                }

                if (!workspaceId.Equals(collection.WorkspaceId))
                {
                    return new ErrorModel
                    {
                        Error = "Wrong workspace ID",
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

                return new SuccessModel
                {
                    Data = folder,
                    Message = "Folder retrieved",
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

        [HttpPost("Workspace/{workspaceId}/Collection/{collectionId}/Folder")]
        public async Task<ActionResult<object>> Create([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId, Folder folder)
        {
            try
            {
                var collection = await _unitOfWork.CollectionRepository.GetCollectionByIdAsync(collectionId);

                if (collection is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no collection with such ID",
                        Success = false
                    };
                }

                if (!workspaceId.Equals(collection.WorkspaceId))
                {
                    return new ErrorModel
                    {
                        Error = "Wrong workspace ID",
                        Success = false
                    };
                }

                folder.CollectionId = collectionId;
                _unitOfWork.FolderRepository.Create(folder);
                collection.Folders.Add(folder);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = folder,
                    Message = "Folder created",
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

        [HttpPatch("Workspace/{workspaceId}/Collection/{collectionId}/Folder/{folderId}")]
        public async Task<ActionResult<object>> Update([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId,
                                                                                    [FromRoute] Guid folderId, Folder folder)
        {
            try
            {
                var collection = await _unitOfWork.CollectionRepository.GetCollectionByIdAsync(collectionId);

                if (collection is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no collection with such ID",
                        Success = false
                    };
                }

                if (!workspaceId.Equals(collection.WorkspaceId))
                {
                    return new ErrorModel
                    {
                        Error = "Wrong workspace ID",
                        Success = false
                    };
                }

                var folderToUpdate = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folderToUpdate is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no folder with such ID",
                        Success = false
                    };
                }

                folder.Id = folderId;
                folder.CollectionId = collectionId;
                
                _unitOfWork.FolderRepository.Update(folder);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = folder,
                    Message = "Folder updated",
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

        [HttpDelete("Workspace/{workspaceId}/Collection/{collectionId}/Folder/{folderId}")]
        public async Task<ActionResult<object>> Delete([FromRoute] Guid workspaceId, [FromRoute] Guid collectionId,
                                                                                    [FromRoute] Guid folderId)
        {
            try
            {
                var collection = await _unitOfWork.CollectionRepository.GetCollectionByIdAsync(collectionId);

                if (collection is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no collection with such ID",
                        Success = false
                    };
                }

                if (!workspaceId.Equals(collection.WorkspaceId))
                {
                    return new ErrorModel
                    {
                        Error = "Wrong workspace ID",
                        Success = false
                    };
                }

                var folderToDelete = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folderToDelete is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no folder with such ID",
                        Success = false
                    };
                }
                
                _unitOfWork.FolderRepository.Delete(folderToDelete);
                collection.Folders.Remove(folderToDelete);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = folderToDelete,
                    Message = "Folder deleted",
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
