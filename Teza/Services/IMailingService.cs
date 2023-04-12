using System;
using System.Threading.Tasks;
using Data.Repositories.Implementation;
using Teza.Models;

namespace Teza.Services
{
    public interface IMailingService
    {
        bool SendMail(string email, string workspaceName, Guid workspaceId, UserEmailConfirmationRepository userEmailConfirmationRepository);
    }
}
