using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RSADataManager.Library.Internal.DataAccess;
using RSADataManager.Library.Models;

namespace RSADataManager.Library.DataAccess
{
    public class UserData
    {
        private readonly IConfiguration _configuration;

        public UserData(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public UserModel GetUserById(string Id)
        {
            var sqlAccess = new SqlDataAccess(_configuration);
            var p = new { Id = Id };
            return sqlAccess
                .LoadData<UserModel, dynamic>(storedProcedure: "[dbo].[spUserLookup]",
                                              parameters: p,
                                              connectionStringName: "RSAData")
                .First();
        }
    }
}
