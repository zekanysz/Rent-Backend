using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieRent.Data;
using MovieRent.Models.Identity;

namespace MovieRent.Controllers
{
    public class IdentityController : ApiController
    {

        private readonly UserManager<User> userManager;
        private readonly RoleManager<MongoRole> _roleManager;
        private readonly ApplicationSettings appSettings;

        public IdentityController(UserManager<User> userManager, RoleManager<MongoRole> roleManager, IOptions<ApplicationSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
            this.userManager = userManager;
            _roleManager = roleManager;
        }

        [Route(nameof(Register))]
        public async Task<ActionResult> Register(RegisterRequestModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
            };
            var result = await this.userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(JsonSerializer.Serialize("Account has been successfully registered"));
            }

            return this.BadRequest(result.Errors);
        }


        [Route(nameof(Login))]
        public async Task<ActionResult> Login(LoginRequestModel model)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized();
            }

            var passwordValid = await this.userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordValid)
            {
                return Unauthorized();
            }


            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var defaultRoleName = "";
            if (user.Roles.Count != 0)
            {
                for (int i = 0; i < user.Roles.Count; i++)
                {

                    var t = _roleManager.FindByIdAsync(user.Roles[i]).Result;
                    tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, t.Name));
                }

                var defaultRoleObject = _roleManager.FindByIdAsync(user.Roles.FirstOrDefault()).Result;
                defaultRoleName = defaultRoleObject.Name;
            }

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);


            return Ok(new { token = encryptedToken, expiration = tokenDescriptor.Expires, userName = user.UserName, userRole = defaultRoleName });
        }
    }
}