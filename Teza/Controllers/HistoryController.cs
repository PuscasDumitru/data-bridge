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
    public class HistoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public HistoryController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllHistories()
        {
            try
            {
                var allHistories = await _unitOfWork.HistoryRepository.GetAllHistoriesAsync();

                return new SuccessModel
                {
                    Data = allHistories,
                    Message = "Histories retrieved",
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
        public async Task<ActionResult<object>> GetHistoryByIdAsync(Guid historyId)
        {
            try
            {
                var history = await _unitOfWork.HistoryRepository.GetHistoryByIdAsync(historyId);

                if (history is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no history with such an ID",
                        Success = false
                    };
                }

                return new SuccessModel
                {
                    Data = history,
                    Message = "History retrieved",
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
        public async Task<ActionResult<object>> Create(History history)
        {
            try
            {
                _unitOfWork.HistoryRepository.Create(history);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = history,
                    Message = "History created",
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
        public async Task<ActionResult<object>> Update(History history)
        {
            try
            {
                _unitOfWork.HistoryRepository.Update(history);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = history,
                    Message = "History updated",
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
        public async Task<ActionResult<object>> Delete(Guid historyId)
        {
            try
            {
                var history = await _unitOfWork.HistoryRepository.GetHistoryByIdAsync(historyId);

                if (history is null)
                {
                    return new ErrorModel
                    {
                        Error = "There's no history with such an ID",
                        Success = false
                    };
                }

                _unitOfWork.HistoryRepository.Delete(history);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = history,
                    Message = "History deleted",
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
