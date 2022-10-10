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

namespace Teza.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class CollectionController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public CollectionController(RepositoryDbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        [HttpGet]
        public async Task<IEnumerable<Collection>> GetAllCollections()
        {
            return await _unitOfWork.CollectionRepository.GetAll().ToListAsync();
        }
        
        [HttpPost]
        public async Task Create(Collection collection)
        {
            _unitOfWork.CollectionRepository.Create(collection);
            await _unitOfWork.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<Collection> Update(Collection collection)
        {
            _unitOfWork.CollectionRepository.Update(collection);
            await _unitOfWork.SaveChangesAsync();
            return collection;
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            Collection collection = _unitOfWork.CollectionRepository.GetById(id);
            _unitOfWork.CollectionRepository.Delete(collection);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
