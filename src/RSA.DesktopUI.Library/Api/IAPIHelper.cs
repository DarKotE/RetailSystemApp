using System.Net.Http;
using System.Threading.Tasks;
using RSA.DesktopUI.Library.Models;

namespace RSA.DesktopUI.Library.Api
{
    public interface IApiHelper
    {
        HttpClient ApiClient { get; }
        Task<AuthenticatedUser> Authenticate(string username, string password);
        void LogOffUser();
        Task GetLoggedInUserInfo(string token);
    }
}