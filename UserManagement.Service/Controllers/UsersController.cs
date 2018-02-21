using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Model;
using UserManagement.BL;
using UserManagement.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System;

namespace UserManagement.Service.Controllers
{
    [Route("api/[controller]"), Authorize]
    public class UsersController : Controller
    {
        private readonly DAL.UserContext context;
        private UserManager userManager;

        public UsersController(DAL.UserContext context, IConfiguration config)
        {
            this.context = context;
            userManager = new UserManager(this.context, config);
        }

        // GET api/users
        [HttpGet]
        public List<string> GetAllUserNames()
        {
            return userManager.GetAllUserNames();
        }

        // GET api/users/partial
        [HttpGet("partial")]
        public PartialUser GetPartialUser()
        {
            Int32.TryParse(User.FindFirstValue("jti"), out var id);

            return userManager.GetPartialUser(id);
        }
        
        // GET api/users/Mantas
        [HttpGet("{user}")]
        public int GetUserId(string user)
        {
            return userManager.GetUserId(user);
        }

        // POST api/users
        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks>
        ///
        /*
        {
            "name":"Mantas",
            "password":"test"
        }
        */
        /// </remarks>
        [HttpPost, AllowAnonymous]
        public IActionResult CreateUser([FromBody]UserData user)
        {
            return userManager.CreateUser(user) ? StatusCode(200) : StatusCode(400);
        }
    }
}
