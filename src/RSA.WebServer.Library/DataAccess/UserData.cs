using System.Linq;
using Microsoft.Extensions.Configuration;
using RSA.WebServer.Library.Internal.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public class UserData
    {
        private readonly IConfiguration _configuration;

        public UserData(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public UserModel GetUserById(string id)
        {
            using var sqlAccess = new SqlDataAccess(_configuration);
            var p = new {Id = id};
            return sqlAccess
                .LoadData<UserModel, dynamic>(storedProcedure: "[dbo].[spUserLookup]",
                                              parameters: p,
                                              connectionStringName: "RSAData")
                .First();
        }
    }
}
