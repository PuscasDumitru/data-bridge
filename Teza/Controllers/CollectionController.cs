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
    public class CollectionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CollectionController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllCollectionsAsync()
        {
            try
            {
                var allCollections = await _unitOfWork.CollectionRepository.GetAllCollectionsAsync();

                return new SuccessModel
                {
                    Data = allCollections,
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

        [HttpGet]
        public async Task<ActionResult<object>> GetAllCollectionsByWorkspaceIdAsync(Guid workspaceId)
        {
            try
            {
                //var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(workspaceId);

                //if (workspace is null)
                //{
                //    return new ErrorModel
                //    {
                //        Error = "There's no workspace with such an ID",
                //        Success = false
                //    };
                //}

                var allCollections = await _unitOfWork.CollectionRepository.GetByCondition(x => x.WorkspaceId.Equals(workspaceId)).ToListAsync();

                return new SuccessModel
                {
                    Data = allCollections,
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

        [HttpGet]
        public async Task<ActionResult<object>> GetCollectionByIdAsync(Guid collectionId)
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

        [HttpPost]
        public async Task<ActionResult<object>> Create(Collection collection)
        {
            try
            {
                _unitOfWork.CollectionRepository.Create(collection);

                var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(collection.WorkspaceId);
                
                if (workspace is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no workspace with such an ID",
                        Success = false
                    };
                }

                workspace.Collections.Add(collection);
                //collection.Workspace.Collections.Add(collection);

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

        [HttpPost]
        public async Task<ActionResult<object>> Update(Collection collection)
        {
            try
            {
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

        [HttpDelete]
        public async Task<ActionResult<object>> Delete(Guid collectionId)
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

                _unitOfWork.CollectionRepository.Delete(collection);

                var workspace = await _unitOfWork.WorkspaceRepository.GetWorkspaceByIdAsync(collection.WorkspaceId);

                if (workspace is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no workspace with such an ID",
                        Success = false
                    };
                }

                //workspace.Collections.Remove(collection);

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
