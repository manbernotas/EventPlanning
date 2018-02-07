using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.BL;

namespace UserManagement.Service.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticateController : Controller
    {
        private readonly DAL.UserContext context;
        private UserManager userManager;

        public AuthenticateController(DAL.UserContext context)
        {
            this.context = context;
            userManager = new UserManager(this.context);
        }

        // POST api/authenticate (login)
        [HttpPost]
        public void Post([FromBody]Model.UserData user)
        {
            // TODO: implement
        }

        // DELETE api/authenticate (logout)
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            // TODO: implement
        }
    }
}
