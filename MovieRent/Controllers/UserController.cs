using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieRent.Data;
using MovieRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieRent.Controllers
{

    public class UserController : ApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;
        public UserController(UserManager<User> userManager, RoleManager<MongoRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }



        [Authorize(Roles = "Admin")]
        [HttpPost("AddRole/{roleName}")]
        public async Task<IActionResult> CreatRoleAsync(string roleName)
        {
            //var u = await _userManager.FindByNameAsync("admin@admin.com");
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

        [Authorize(Roles = "Admin")]
        [HttpPost("AddRoleToUser")]
        public async Task<IActionResult> AddRoleToUser([FromBody] UserToRole userToRole)
        {
            var user = await _userManager.FindByNameAsync(userToRole.User);

            if(!await _userManager.IsInRoleAsync(user, userToRole.Role))
            {
                await _userManager.AddToRoleAsync(user, userToRole.Role);
                return Ok("Role added to User");
            }else
            {
                return Ok("User has this role already");

            }
        }


        [HttpGet("AllRoles")]
        public IActionResult ListRoles()
        {
            var allRole = _roleManager.Roles;
            List<string> roleList = new List<string>();
            foreach (var role in allRole)
            {
                roleList.Add(role.Name);
            }
            return Ok(roleList);
        }

        [HttpGet("AllUsers")]
        public IActionResult ListUsers()
        {
            var allUser = _userManager.Users;
            List<string> userList = new List<string>();
            foreach (var user in allUser)
            {
                userList.Add(user.UserName);
            }
            return Ok(userList);
        }

    }

}
