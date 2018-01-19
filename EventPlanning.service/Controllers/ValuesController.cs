using System.Collections.Generic;
using EventPlanning.BL;
using EventPlanning.Model;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanning.service.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly DAL.EventContext context;
        private UserManager userManager;
        private EventManager eventManager;

        public ValuesController(DAL.EventContext context)
        {
            this.context = context;
            userManager = new UserManager(this.context);
            eventManager = new EventManager(this.context);
        }

        // GET api/values
        [HttpGet]
        public List<DAL.User> GetUsers()
        {
            return userManager.GetUsers();
        }

        // POST api/values/validate-user
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

        // POST api/values/create-user
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

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
