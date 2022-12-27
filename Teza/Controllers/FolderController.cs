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
                        error = "There's no collection with such ID",
                        success = false
                    };
                }

                if (!workspaceId.Equals(collection.WorkspaceId))
                {
                    return new ErrorModel
                    {
                        error = "Wrong workspace ID",
                        success = false
                    };
                }

                return new SuccessModel
                {
                    data = collection.Folders,
                    message = "Folders retrieved",
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
                        error = "There's no collection with such ID",
                        success = false
                    };
                }

                if (!workspaceId.Equals(collection.WorkspaceId))
                {
                    return new ErrorModel
                    {
                        error = "Wrong workspace ID",
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

                return new SuccessModel
                {
                    data = folder,
                    message = "Folder retrieved",
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
                        error = "There's no collection with such ID",
                        success = false
                    };
                }

                if (!workspaceId.Equals(collection.WorkspaceId))
                {
                    return new ErrorModel
                    {
                        error = "Wrong workspace ID",
                        success = false
                    };
                }

                folder.CollectionId = collectionId;
                _unitOfWork.FolderRepository.Create(folder);
                collection.Folders.Add(folder);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    data = folder,
                    message = "Folder created",
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
                        error = "There's no collection with such ID",
                        success = false
                    };
                }

                if (!workspaceId.Equals(collection.WorkspaceId))
                {
                    return new ErrorModel
                    {
                        error = "Wrong workspace ID",
                        success = false
                    };
                }

                var folderToUpdate = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folderToUpdate is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no folder with such ID",
                        success = false
                    };
                }

                var updatedFolder = _unitOfWork.FolderRepository.UpdateEntity(folderToUpdate, folder);
                _unitOfWork.FolderRepository.Update(updatedFolder);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    data = updatedFolder,
                    message = "Folder updated",
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
                        error = "There's no collection with such ID",
                        success = false
                    };
                }

                if (!workspaceId.Equals(collection.WorkspaceId))
                {
                    return new ErrorModel
                    {
                        error = "Wrong workspace ID",
                        success = false
                    };
                }

                var folderToDelete = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folderToDelete is null)
                {
                    return new ErrorModel
                    {
                        error = "There's no folder with such ID",
                        success = false
                    };
                }
                
                _unitOfWork.FolderRepository.Delete(folderToDelete);
                collection.Folders.Remove(folderToDelete);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    data = folderToDelete,
                    message = "Folder deleted",
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
