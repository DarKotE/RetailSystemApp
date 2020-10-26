using System.Collections.Generic;
using System.Threading.Tasks;
using RSADesktopUI.Library.Models;

namespace RSADesktopUI.Library.Api
{
    public interface IUserEndpoint
    {
        Task<List<ApplicationUserModel>> GetAll();
    }
}