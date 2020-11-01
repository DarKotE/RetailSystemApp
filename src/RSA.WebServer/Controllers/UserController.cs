using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IUserData _userData;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context,
                              UserManager<IdentityUser> userManager,
                              IUserData userData,
                              ILogger<UserController> logger)
        {
            _context = context;
            _userManager = userManager;
            _userData = userData;
            _logger = logger;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return _userData.GetUserById(userId);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public IEnumerable<ApplicationUserModel> GetAllUsers()
        {
            //var output = new List<ApplicationUserModel>();

            IEnumerable<IdentityUser> users = _context.Users;
            var userRoles =
                _context.UserRoles.Join(_context.Roles, ur => ur.RoleId, r => r.Id,
                                        (ur, r) => new {ur.UserId, ur.RoleId, r.Name});
            
            foreach (IdentityUser user in users)
            {
                var u = new ApplicationUserModel(user.Id, user.Email);
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                foreach (var role in userRoles
                   .Where(x => x.UserId == u.Id))
                    dictionary.Add(role.RoleId, role.Name);
                u.Roles = dictionary;
                yield return u;
            }
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
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedUser =_userData.GetUserById(loggedUserId);

            var userIdentity = await _userManager.FindByIdAsync(pair.UserId);
            if (userIdentity is null)
            {
                // TODO log?
            }
            else
            {
                _logger.LogInformation("Admin {Admin} added user {User} to role {Role}",
                                       loggedUser.EmailAddress, userIdentity.Email, pair.RoleName);
                await _userManager.AddToRoleAsync(userIdentity, pair.RoleName);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveRole(UserRolePairModel pair)
        {
            string loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedUser = _userData.GetUserById(loggedUserId);

            var userIdentity = await _userManager.FindByIdAsync(pair.UserId);
            if (userIdentity is null)
            {
                // TODO log?
            }
            else
            {
                _logger.LogInformation("Admin {Admin} removed user {User} to role {Role}",
                                       loggedUser.EmailAddress, userIdentity.Email, pair.RoleName);
                await _userManager.RemoveFromRoleAsync(userIdentity, pair.RoleName);
            }
        }
    }
}
