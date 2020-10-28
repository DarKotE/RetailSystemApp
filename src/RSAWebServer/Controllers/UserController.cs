using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RSADataManager.Library.DataAccess;
using RSADataManager.Library.Models;
using RSAWebServer.Data;
using RSAWebServer.Models;

namespace RSAWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new UserData().GetUserById(userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]

        public List<ApplicationUserModel> GetAllUsers()
        {
            var output = new List<ApplicationUserModel>();

            var users = _context.Users.ToList();
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles 
                            on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };
            foreach (var user in users)
            {
                var u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email
                };
                u.Roles = userRoles.Where(x => x.UserId == u.Id)
                                  .ToDictionary(key => key.RoleId,
                                                val => val.Name);
                output.Add(u);
            }

            return output;

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {

            return _context.Roles.ToDictionary(x => x.Id, x => x.Name);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/AddRole")]
        public async Task AddRole(UserRolePairModel pair)
        {
            var userIdentity =await _userManager.FindByIdAsync(pair.UserId);
            await _userManager.AddToRoleAsync(userIdentity, pair.RoleName);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/RemoveRole")]
        public async Task RemoveRole(UserRolePairModel pair)
        {
            var userIdentity = await _userManager.FindByIdAsync(pair.UserId);
            await _userManager.RemoveFromRoleAsync(userIdentity, pair.RoleName);
        }
    }
}
