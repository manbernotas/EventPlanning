using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Model;
using UserManagement.BL;
using UserManagement.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using static EventPlanning.Utilities.Utilities;

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
            var ip = HttpContext.Connection.RemoteIpAddress;
            return userManager.GetAllUserNames();
        }

        // GET api/users/partial
        [HttpGet("partial")]
        public PartialUser GetPartialUser()
        {
            return userManager.GetPartialUser(GetCurrentUserId(User));
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
