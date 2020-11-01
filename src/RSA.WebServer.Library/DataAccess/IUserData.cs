using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public interface IUserData
    {
        UserModel GetUserById(string id);
    }
}