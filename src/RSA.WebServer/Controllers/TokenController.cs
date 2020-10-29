using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RSA.WebServer.Data; //using Microsoft.IdentityModel.JsonWebTokens;


namespace RSA.WebServer.Controllers
{
    public class TokenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TokenController(ApplicationDbContext context,
               UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string grant_type)
        {
            if (await IsValidUserPassword(username, password))
            {
                return new ObjectResult(await GenerateToken(username));
            }
            else
            {
                return BadRequest();
            }

        }

        public async Task<bool> IsValidUserPassword(string username, string password)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(username);
            return await _userManager.CheckPasswordAsync(user, password);
        }

        private async Task<dynamic> GenerateToken(string username)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(username);
            //TODO change this to var roles = _userManager.GetRolesAsync(user); and remove all the roleID's for auth'ed user
            var roles = from ur in _context.UserRoles
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select new { ur.UserId, ur.RoleId, r.Name };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString())
            };
            claims.AddRange(roles.Select(role => 
                new Claim(ClaimTypes.Role, role.Name)));
            JwtSecurityToken token =new JwtSecurityToken(
                header:             new JwtHeader(
                signingCredentials: new SigningCredentials(
                key:                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("iEYAFytP7xsmQUxndJXviEYAFytP7xsmQUxndJXv")), //TODO make env variable
                algorithm:          SecurityAlgorithms.HmacSha256)),
                payload:            new JwtPayload(claims));

            return new
            {
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = username
            };
        }
    }
}
