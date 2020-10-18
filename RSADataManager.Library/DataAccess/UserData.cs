using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSADataManager.Library.Internal.DataAccess;
using RSADataManager.Library.Models;

namespace RSADataManager.Library.DataAccess
{
    public class UserData
    {
        public List<UserModel> GetUserById(string Id)
        {
            var sqlAccess = new SqlDataAccess();
            var p = new { Id = Id };
            return sqlAccess.LoadData<UserModel, dynamic>("[dbo].[spUserLookup]", p, "RSAData");
        }
    }
}
