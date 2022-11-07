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
        public async Task<ActionResult<object>> GetAllQueriesAsync()
        {
            try
            {
                var allQueries = await _unitOfWork.QueryRepository.GetAllQueriesAsync();

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

        [HttpGet]
        public async Task<ActionResult<object>> GetQueryByIdAsync(Guid queryId)
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

        [HttpGet]
        public async Task<ActionResult<object>> GetQueriesByFolderIdAsync(Guid folderId)
        {
            try
            {
                var allQueries = await _unitOfWork.QueryRepository.GetByCondition(x => x.FolderId.Equals(folderId)).ToListAsync();

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
        public async Task<ActionResult<object>> Delete(Guid queryId)
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
