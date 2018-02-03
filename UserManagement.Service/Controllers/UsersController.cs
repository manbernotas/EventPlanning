using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Model;
using UserManagement.BL;

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

        // GET api/users
        [HttpGet]
        public List<DAL.User> GetUsers()
        {
            return userManager.GetUsers();
        }

        [HttpGet("{user}")]
        public int GetUserId(string user)
        {
            return userManager.GetUserId(user);
        }

        // POST api/users/validate-user
        /// <summary>
        /// Validates if password is valid for user
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
        [HttpPost("validate-user")]
        public bool IsPasswordValid([FromBody]UserData user)
        {
            return userManager.IsPasswordValid(user);
        }

        // POST api/users/create-user
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
        [HttpPost("create-user")]
        public IActionResult CreateUser([FromBody]UserData user)
        {
            return userManager.CreateUser(user) ? StatusCode(200) : StatusCode(400);
        }
    }
}
