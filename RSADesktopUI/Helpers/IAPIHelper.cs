using System.Threading.Tasks;
using RSADesktopUI.Models;

namespace RSADesktopUI.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
    }
}