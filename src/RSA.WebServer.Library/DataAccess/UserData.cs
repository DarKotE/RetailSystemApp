using System.Linq;
using RSA.WebServer.Library.Internal.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess _sql;

        public UserData(ISqlDataAccess sql)
        {
            _sql = sql;
        }

        public UserModel GetUserById(string id)
        {
            var p = new {Id = id};
            return _sql
                .LoadData<UserModel, dynamic>(storedProcedure: "[dbo].[spUserLookup]",
                                              parameters: p,
                                              connectionStringName: "RSAData")
                .First();
        }
    }
}
