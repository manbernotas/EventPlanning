using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using EventPlanning.BL;
using EventPlanning.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EventPlanning.service.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        private readonly DAL.EventContext context;
        private EventManager eventManager;
        private InvitationManager invitationManager;

        public EventsController(DAL.EventContext context, ISmtpClient smtpclient)
        {
            this.context = context;
            eventManager = new EventManager(this.context);
            invitationManager = new InvitationManager(this.context, smtpclient);
        }

        // GET api/events
        [HttpGet]
        public List<DAL.Event> GetEvents()
        {
            return eventManager.GetEvents();
        }

        // events/{1}/partial
        // GET api/user/1/events
        [HttpGet("{userId}")]
        [Route("user/{userId}/events")] // TODO: test this
        public List<DAL.Event> GetEvents(int userId)
        {
            return eventManager.GetUserEvents(userId);
        }

        // GET api/events/search/Pan
        [HttpGet("search/{pattern:minlength(3)}")]
        public List<DAL.Event> GetEvents(string pattern)
        {
            return eventManager.GetEvents(pattern);
        }

        // TODO: api/events/search?dateFrom=2017-01-03&dateTo=xxxx
        // GET api/events/search-by-date/2017-01-03/2017-05-08
        //[HttpGet("search-by-date/{dateFrom:datetime}/{dateTo:datetime?}")]
        [HttpGet("search")]
        public List<DAL.Event> GetEvents(DateTime dateFrom, DateTime? dateTo = null)
        {
            return eventManager.GetEvents(dateFrom, dateTo ?? dateFrom);
        }

        // TODO: events/{id}/activities
        [HttpGet("activities/{eventId}")]
        public List<DAL.Activity> GetEventActivities(int eventId)
        {
            return eventManager.GetEventActivities(eventId);
        }

        // TODO: move this to activities controller
        // GET api/events/activities
        [HttpGet("activities")]
        public List<DAL.Activity> GetActivities()
        {
            return eventManager.GetActivities();
        }

        // TODO: move this to activities controller
        // TODO: api/activities/types
        // GET api/events/activity-types
        [HttpGet("activity-types")]
        public List<DAL.ActivityType> GetActivityTypes()
        {
            return eventManager.GetActivityTypes();
        }

        // TODO: api/events
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

        // TODO: Move this to activities controller
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

        // TODO: Move this to activities controller
        // api/activities/types POST
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

        /// <summary>
        /// Updates an existing event
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        /// <remarks>
        /*
        {
            "title":"Pandemic",
            "description":"Save the world together",
            "userid":1,
            "datefrom":"2017-01-02",
            "dateto":"2017-01-02",
            "address":"baltupio 1"
        }
        */
        /// </remarks>
        [HttpPatch("{eventId}")]
        public IActionResult PatchEvent([FromRoute]int eventId, [FromBody]EventData eventData)
        {
            return eventManager.PatchEvent(eventId, eventData) ? StatusCode(200) : StatusCode(400);
        }

        [HttpPut("{eventId}")]
        public IActionResult AddActivityToEvent([FromRoute]int eventId, [FromBody]ActivityData activityData)
        {
            return eventManager.AddActivityToEvent(eventId, activityData) ? StatusCode(200) : StatusCode(400);
        }

        [HttpDelete("{eventId}")]
        public IActionResult RemoveActivityFromEvent([FromRoute]int eventId, [FromBody]ActivityData activityData)
        {
            return eventManager.RemoveActivityFromEvent(eventId, activityData) ? StatusCode(200) : StatusCode(400);
        }

        [HttpPost("{eventId}/invite")]
        public IActionResult InviteToEvent([FromRoute]int eventId, [FromBody]UserData user)
        {
            return invitationManager.InviteToEvent(eventId, user) ? StatusCode(200) : StatusCode(400);
        }
    }
}
