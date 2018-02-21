using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UserManagement.BL;

namespace UserManagement.Service.Controllers
{
    [Route("api/[controller]"), Authorize]
    public class AuthenticateController : Controller
    {
        private readonly DAL.UserContext context;
        private UserManager userManager;

        public AuthenticateController(DAL.UserContext context, IConfiguration config)
        {
            this.context = context;
            userManager = new UserManager(this.context, config);
        }

        // POST api/authenticate (login)
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody]Model.UserData userData)
        {
            IActionResult response = Unauthorized();
            var user = userManager.IsPasswordValid(userData);

            if (user != null)
            {
                var tokenString = userManager.CreateAccessToken(user);
                response = Ok(new { token = tokenString });
                userManager.CreateLoginRecord(user);
            }

            return response;
        }

        // DELETE api/authenticate (logout)
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
