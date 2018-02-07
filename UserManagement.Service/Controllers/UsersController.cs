using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Model;
using UserManagement.BL;
using UserManagement.DTO;

namespace UserManagement.Service.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly DAL.UserContext context;
        private UserManager userManager;

        public UsersController(DAL.UserContext context)
        {
            this.context = context;
            userManager = new UserManager(this.context);
        }

        // GET api/users/1
        [HttpGet("{userId}")]
        public PartialUser GetPartialUser(int userId)
        {
            return userManager.GetPartialUser(userId);
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
        [HttpPost]
        public IActionResult CreateUser([FromBody]UserData user)
        {
            return userManager.CreateUser(user) ? StatusCode(200) : StatusCode(400);
        }
    }
}
