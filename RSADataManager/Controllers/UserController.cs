using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RSADataManager.Library.DataAccess;
using RSADataManager.Library.Models;
using RSADataManager.Models;

namespace RSADataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        [HttpGet]
        public UserModel GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();

            return new UserData().GetUserById(userId);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = userManager.Users.ToList();
                var roles = context.Roles.ToList();

                return
                    (from user in users
                     let role = from userRole in user.Roles
                                join role in roles on userRole.RoleId equals role.Id
                                select new { userRole.RoleId, role.Name }
                     select new ApplicationUserModel {
                         Id = user.Id,
                         Email = user.Email,
                         Roles = role.ToDictionary(o => o.RoleId, o => o.Name)
                     }).ToList();
            }
        }
    }
}
