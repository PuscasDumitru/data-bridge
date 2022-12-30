using System;
using System.Threading.Tasks;
using Teza.Models;

namespace Teza.Services
{
    public interface IAuthService
    {
        Task<AuthServiceModel> GetUser(Guid userId);
    }
}