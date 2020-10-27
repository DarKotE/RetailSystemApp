using System.Collections.Generic;
using System.Threading.Tasks;
using RSADesktopUI.Library.Models;

namespace RSADesktopUI.Library.Api
{
    public interface IUserEndpoint
    {
        Task<List<ApplicationUserModel>> GetAll();
        Task<Dictionary<string, string>> GetAllRoles();
        Task AddUserToRole(string userId, string roleName);
        Task RemoveUserFromRole(string userId, string roleName);
    }
}