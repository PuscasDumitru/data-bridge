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
    public class QueryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public QueryController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllQueries()
        {
            try
            {
                var allQueries = await _unitOfWork.QueryRepository.GetAll().ToListAsync();

                return new SuccessModel
                {
                    Data = allQueries,
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

        [HttpPost]
        public async Task<ActionResult<object>> Create(Query query)
        {
            try
            {
                _unitOfWork.QueryRepository.Create(query);
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

        [HttpPost]
        public async Task<ActionResult<object>> Update(Query query)
        {
            try
            {
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

        [HttpDelete]
        public async Task<ActionResult<object>> Delete(int id)
        {
            try
            {
                Query query = _unitOfWork.QueryRepository.GetById(id);
                _unitOfWork.QueryRepository.Delete(query);
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
