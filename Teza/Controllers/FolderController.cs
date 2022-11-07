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
    [Route("api/[Controller]/[Action]")]
    public class FolderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FolderController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllFolders()
        {
            try
            {
                var allFolders = await _unitOfWork.FolderRepository.GetAllFoldersAsync();

                return new SuccessModel
                {
                    Data = allFolders,
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

        [HttpGet]
        public async Task<ActionResult<object>> GetFolderByIdAsync(Guid folderId)
        {
            try
            {
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

        [HttpGet]
        public async Task<ActionResult<object>> GetAllFoldersByCollectionIdAsync(Guid collectionId)
        {
            try
            {
                var allFolders = await _unitOfWork.FolderRepository.GetByCondition(x => x.CollectionId.Equals(collectionId)).ToListAsync();

                return new SuccessModel
                {
                    Data = allFolders,
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

        [HttpPost]
        public async Task<ActionResult<object>> Create(Folder folder)
        {
            try
            {
                _unitOfWork.FolderRepository.Create(folder);
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

        [HttpPost]
        public async Task<ActionResult<object>> Update(Folder folder)
        {
            try
            {
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

        [HttpDelete]
        public async Task<ActionResult<object>> Delete(Guid folderId)
        {
            try
            {
                var folder = await _unitOfWork.FolderRepository.GetFolderByIdAsync(folderId);

                if (folder is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no folder with such an ID",
                        Success = false
                    };
                }

                _unitOfWork.FolderRepository.Delete(folder);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = folder,
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
