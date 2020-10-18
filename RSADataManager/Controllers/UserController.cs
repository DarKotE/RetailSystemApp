using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using RSADataManager.Library.DataAccess;
using RSADataManager.Library.Models;

namespace RSADataManager.Controllers
{
    [Authorize]    
    public class UserController : ApiController
    {
        
        public List<UserModel> GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();

            return new UserData().GetUserById(userId);
        }

    }
}
