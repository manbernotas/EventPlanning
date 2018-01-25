using System.Collections.Generic;
using EventPlanning.BL;
using EventPlanning.Model;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanning.service.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        private readonly DAL.EventContext context;
        private EventManager eventManager;

        public EventsController(DAL.EventContext context)
        {
            this.context = context;
            eventManager = new EventManager(this.context);
        }

        // GET api/events
        [HttpGet]
        public List<DAL.Event> GetEvents()
        {
            return eventManager.GetEvents();
        }

        // GET api/events/1
        [HttpGet("{userId}")]
        public List<DAL.Event> GetEvents(int userId)
        {
            return eventManager.GetUserEvents(userId);
        }

        [HttpGet("activities/{eventId}")]
        public List<DAL.Activity> GetEventsActivities(int eventId)
        {
            return eventManager.GetEventActivities(eventId);
        }

        // GET api/events/activities
        [HttpGet("activities")]
        public List<DAL.Activity> GetActivities()
        {
            return eventManager.GetActivities();
        }

        // GET api/events/activity-types
        [HttpGet("activity-types")]
        public List<DAL.ActivityType> GetActivityTypes()
        {
            return eventManager.GetActivityTypes();
        }

        // POST api/events/create-event
        /// <summary>
        /// Creates new event
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        /// <remarks>
        /*
        {
	        "title":"Pandemic challenge",
	        "description":"Save the world together",
	        "userid":1,
	        "datefrom":"2017-01-02",
	        "dateto":"2017-01-02",
	        "address":"baltupio 1",
	        "activities":["Pandemic"]
        }
        */
        ///</remarks>
        [HttpPost("create-event")]
        public IActionResult CreateEvent([FromBody]EventData eventData)
        {
            return eventManager.CreateEvent(eventData) ? StatusCode(200) : StatusCode(400);
        }

        // POST api/events/create-activity
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
        [HttpPost("create-activity")]
        public IActionResult CreateActivity([FromBody]ActivityData activityData)
        {
            return eventManager.CreateActivity(activityData) ? StatusCode(200) : StatusCode(400);
        }

        // POST api/events/create-activity-type
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
        [HttpPost("create-activity-type")]
        public IActionResult CreateActivityType([FromBody]ActivityTypeData activityTypeData)
        {
            return eventManager.CreateActivityType(activityTypeData) ? StatusCode(200) : StatusCode(400);
        }

        /// <summary>
        /// Creates new participant
        /// </summary>
        /// <param name="participant"></param>
        /// <returns></returns>
        /// <remarks>
        /*
        {
            "userid": 1,
            "eventid: 1
        }
        */
        /// </remarks>
        [HttpPost("participate")]
        public IActionResult ParticipateInEvent([FromBody]ParticipantData participant)
        {
            return eventManager.ParticipateInEvent(participant) ? StatusCode(200) : StatusCode(400);
        }
    }
}
