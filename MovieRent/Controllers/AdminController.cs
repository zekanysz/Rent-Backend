using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieRent.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieRent.Controllers
{

    public class AdminController : ApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;
        public AdminController(UserManager<User> userManager, RoleManager<MongoRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("Roles")]
        [Authorize(Roles = "Admin")]
        public IActionResult ListRoles()
        {
            var allRole = _roleManager.Roles;
            return Ok(allRole);
        }

        [HttpPost("AddRole/{roleName}")]
        public async Task<IActionResult> CreatRoleAsync(string roleName)
        {
            //var u = await _userManager.FindByNameAsync("sz.zekany@gmail.com");
            //await _userManager.AddClaimAsync(u, new Claim(ClaimTypes.Role, roleName));
            //await _userManager.AddToRoleAsync(u, roleName);
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new MongoRole(roleName));
                return Ok("ADMIN CREATED");

            }
            else
            {
                return Ok("ADMIN ALREADY EXISTS");

            }

        }

    }
}
