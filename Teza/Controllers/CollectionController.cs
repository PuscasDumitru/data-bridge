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
        public async Task<ActionResult<object>> GetAllCollections()
        {
            try
            {
                var allCollections = await _unitOfWork.CollectionRepository.GetAll().ToListAsync();

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

        [HttpPost]
        public async Task<ActionResult<object>> Create(Collection collection)
        {
            try
            {
                _unitOfWork.CollectionRepository.Create(collection);
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
        public async Task<ActionResult<object>> Delete(int id)
        {
            try
            {
                Collection collection = _unitOfWork.CollectionRepository.GetById(id);
                _unitOfWork.CollectionRepository.Delete(collection);
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
