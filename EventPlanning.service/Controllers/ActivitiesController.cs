using System.Collections.Generic;
using EventPlanning.BL;
using EventPlanning.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanning.service.Controllers
{
    [Route("api/[controller]"), Authorize]
    public class ActivitiesController : Controller
    {
        private readonly DAL.EventContext context;
        private EventManager eventManager;

        public ActivitiesController(DAL.EventContext context, ISmtpClient smtpclient)
        {
            this.context = context;
            eventManager = new EventManager(this.context);
        }

        // GET api/activities
        [HttpGet("activities")]
        public List<DAL.Activity> GetActivities()
        {
            return eventManager.GetActivities();
        }

        // GET api/activities/types
        [HttpGet("types")]
        public List<DAL.ActivityType> GetActivityTypes()
        {
            return eventManager.GetActivityTypes();
        }

        // POST api/activities
        /// <summary>
        /// Creates new activity
        /// </summary>
        /// <param name="activityData"></param>
        /// <returns></returns>
        /// <remarks>
        /*
        {
	        "title":"Pandemic",
	        "description":"Save the world",
	        "minparticipants":2,
	        "maxparticipants":4,
	        "activitytype":"Board game"
        }
        */
        ///</remarks>
        [HttpPost]
        public IActionResult CreateActivity([FromBody]ActivityData activityData)
        {
            return eventManager.CreateActivity(activityData) ? StatusCode(200) : StatusCode(400);
        }

        // POST api/activities/types
        /// <summary>
        /// Creates new activity type
        /// </summary>
        /// <param name="activityTypeData"></param>
        /// <returns></returns>
        /// <remarks>
        /*
        {
           "title":"Video game"
        }
        */
        ///</remarks>
        [HttpPost("types")]
        public IActionResult CreateActivityType([FromBody]ActivityTypeData activityTypeData)
        {
            return eventManager.CreateActivityType(activityTypeData) ? StatusCode(200) : StatusCode(400);
        }
    }
}
