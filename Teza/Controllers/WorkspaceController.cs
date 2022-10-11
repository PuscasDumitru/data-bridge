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
    public class WorkspaceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkspaceController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllWorkspaces()
        {
            try
            {
                var allWorkspaces = await _unitOfWork.WorkspaceRepository.GetAll().ToListAsync();

                return new SuccessModel
                {
                    Data = allWorkspaces,
                    Message = "Workspaces retrieved",
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
        public async Task<ActionResult<object>> Create(Workspace workspace)
        {
            try
            {
                _unitOfWork.WorkspaceRepository.Create(workspace);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = workspace,
                    Message = "Workspace created",
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
        public async Task<ActionResult<object>> Update(Workspace workspace)
        {
            try
            {
                _unitOfWork.WorkspaceRepository.Update(workspace);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = workspace,
                    Message = "Workspace updated",
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
                Workspace workspace = _unitOfWork.WorkspaceRepository.GetById(id);
                _unitOfWork.WorkspaceRepository.Delete(workspace);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessModel
                {
                    Data = workspace,
                    Message = "Workspace deleted",
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
