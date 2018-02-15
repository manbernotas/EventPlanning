using System;
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
        private InvitationManager invitationManager;

        public EventsController(DAL.EventContext context, ISmtpClient smtpclient)
        {
            this.context = context;
            eventManager = new EventManager(this.context);
            invitationManager = new InvitationManager(this.context, smtpclient);
        }

        // GET api/events
        [HttpGet]
        public List<DAL.PartialEvent> GetPartialEvents()
        {
            return eventManager.GetPartialEvents();
        }

        // GET api/events/1/partial
        [HttpGet("{eventId}/partial")]
        public DAL.PartialEvent GetPartialEventData(int eventId)
        {
            return eventManager.GetPartialEvent(eventId);
        }

        // GET api/events/user/1/
        [HttpGet("user/{userId}")]
        public List<DAL.Event> GetUserEvents(int userId)
        {
            return eventManager.GetUserEvents(userId);
        }

        // GET api/events/search/Pan
        [HttpGet("search/{pattern:minlength(3)}")]
        public List<DAL.Event> GetEvents(string pattern)
        {
            return eventManager.GetEvents(pattern);
        }

        // GET api/events/search?dateFrom=2017-01-03&dateTo=2017-05-08
        [HttpGet("search")]
        public List<DAL.Event> GetEvents(DateTime dateFrom, DateTime? dateTo = null)
        {
            return eventManager.GetEvents(dateFrom, dateTo ?? dateFrom);
        }

        // GET api/events/1/activities
        [HttpGet("{eventId}/activities")]
        public List<DAL.Activity> GetEventActivities(int eventId)
        {
            return eventManager.GetEventActivities(eventId);
        }

        // POST api/events
        /// <summary>
        /// Creates new event
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        /// <remarks>
        /*
        {
            "userId": 1,
            "title": "Pandemic challenge",
            "description": "Save the world together",
            "dateFrom": "2017-01-02T00:00:00",
            "dateTo": "2017-01-02T00:00:00",
            "address": {
                "addressLine1": "Test 2",
                "addressLine2": "",
                "city": "Vilnius",
                "province": "Vilniaus m.",
                "country": "Lithuania",
                "postalCode": "LT-08303"
            }
        }
        */
        ///</remarks>
        [HttpPost]
        public IActionResult CreateEvent([FromBody]EventData eventData)
        {
            return eventManager.CreateEvent(eventData) ? StatusCode(200) : StatusCode(400);
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

        [HttpDelete("leave")]
        public IActionResult LeaveEvent([FromBody]ParticipantData participant)
        {
            return eventManager.LeaveEvent(participant) ? StatusCode(200) : StatusCode(400);
        }

        [HttpPost("{eventId}/invite")]
        public IActionResult InviteToEvent([FromRoute]int eventId, [FromBody]UserData user)
        {
            return invitationManager.InviteToEvent(eventId, user) ? StatusCode(200) : StatusCode(400);
        }
    }
}
