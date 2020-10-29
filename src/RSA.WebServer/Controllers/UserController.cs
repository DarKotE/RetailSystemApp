using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RSA.WebServer.Data;
using RSA.WebServer.Library.DataAccess;
using RSA.WebServer.Library.Models;
using RSA.WebServer.Models;

namespace RSA.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationDbContext context,
                              UserManager<IdentityUser> userManager,
                              IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new UserData(_configuration).GetUserById(userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            var output = new List<ApplicationUserModel>();

            var users = _context.Users.ToList();
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles 
                            on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };
            foreach (var u in users.Select(user => new ApplicationUserModel
            {
                Id = user.Id,
                Email = user.Email
            }))
            {
                u.Roles = userRoles.Where(x => x.UserId == u.Id)
                    .ToDictionary(key => key.RoleId,
                        val => val.Name);
                output.Add(u);
            }

            return output;

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {

            return _context.Roles.ToDictionary(x => x.Id, x => x.Name);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        public async Task AddRole(UserRolePairModel pair)
        {
            var userIdentity =await _userManager.FindByIdAsync(pair.UserId);
            await _userManager.AddToRoleAsync(userIdentity, pair.RoleName);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveRole(UserRolePairModel pair)
        {
            var userIdentity = await _userManager.FindByIdAsync(pair.UserId);
            await _userManager.RemoveFromRoleAsync(userIdentity, pair.RoleName);
        }
    }
}
