using EventPlanning.DAL;
using EventPlanning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventPlanning.BL
{
    public class EventManager
    {
        private EventContext context;

        private Repository repository;

        public EventManager(EventContext context)
        {
            this.context = context;
            repository = new Repository(this.context);
        }

        /// <summary>
        /// Return all events partial data
        /// </summary>
        /// <returns></returns>
        public List<PartialEvent> GetPartialEvents()
        {
            var events = repository.GetEvents().ToList();
            var partialEvents = new List<PartialEvent>();

            foreach (var ev in events)
            {
                partialEvents.Add(GetPartialEvent(ev.Id));
            }

            return partialEvents;
        }

        /// <summary>
        /// Returns events created by user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Event> GetUserEvents(int userId)
        {
            return repository.GetUserEvents(userId)?.ToList();
        }

        /// <summary>
        /// Returns partial event data
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public PartialEvent GetPartialEvent(int eventId)
        {
            var ev = repository.GetEvent(eventId);

            if (ev == null)
            {
                return null;
            }

            var partialEvent = new PartialEvent()
            {
                Title = ev.Title,
                Creator = GetUserName(ev.UserId),
                Participants = GetParticipants(ev.Id),
                MaxParticipants = GetMaxParticipants(ev.Id),
            };

            return partialEvent;
        }

        /// <summary>
        /// Returns max participants according to event activities
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public int GetMaxParticipants(int eventId)
        {
            var eventActivities = GetEventActivities(eventId);

            if (eventActivities == null)
            {
                return 0;
            }

            return eventActivities.Select(ea => ea.MaxParticipants).Sum();
        }

        /// <summary>
        /// Returns username from user service
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserName(int userId)
        {
            var url = new StringBuilder();
            url.AppendFormat("http://localhost:5011/api/users/{0}/partial", userId);
            var partialUser = new PartialUser();
            partialUser = (PartialUser)Utilities.Utilities.GetAsync(userId, url.ToString(), partialUser);
            
            return partialUser.UserName;
        }

        /// <summary>
        /// Returns participants count
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public int GetParticipants(int eventId)
        {
            return repository.GetParticipants(eventId).ToList().Count;
        }

        /// <summary>
        /// Returns events where any event property contains pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public List<Event> GetEvents(string pattern)
        {
            return repository.GetEvents()
                .Where(e => (e.Title != null && e.Title.Contains(pattern))
                         || (e.Description != null && e.Description.Contains(pattern))
                         || (e.UserId.ToString().Contains(pattern))
                         || (e.Id.ToString().Contains(pattern))
                         || (e.Address.City != null && e.Address.City.Contains(pattern))
                         || (e.Address.Country != null && e.Address.Country.Contains(pattern))
                         || (e.Address.AddressLine1 != null && e.Address.AddressLine1.Contains(pattern))
                         || (e.Address.AddressLine2 != null && e.Address.AddressLine2.Contains(pattern))
                         || (e.Address.Province != null && e.Address.Province.Contains(pattern)))
                .Where(e => e.Type == (int)Event.EventTypes.Public)
                .ToList();
        }

        /// <summary>
        /// Returns events between date intervals
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public List<Event> GetEvents(DateTime dateFrom, DateTime dateTo)
        {
            return repository.GetEvents()
                .Where(e => e.DateFrom >= dateFrom.Date
                         && e.DateTo <= dateTo.Date)
                .Where(e => e.Type == (int)Event.EventTypes.Public)
                .ToList();
        }

        /// <summary>
        /// Returns event activities
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public List<Activity> GetEventActivities(int eventId)
        {
            return repository.GetEventActivities(eventId)?.ToList();
        }

        /// <summary>
        /// Creates new event
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public bool CreateEvent(EventData eventData)
        {
            if (eventData == null)
            {
                return false;
            }

            try
            {
                var newEvent = CopyEventDataToEvent(eventData);

                if (newEvent == null)
                {
                    return false;
                }

                if (!repository.SaveEvent(newEvent))
                {
                    return false;
                }

                var eventId = newEvent.Id; 
                var eventActivities = CopyToEventActivities(eventId, eventData.Activities);

                if (eventActivities != null)
                {
                    repository.SaveEventActivities(eventActivities);
                }

                if (newEvent.Address == null)
                {
                    return false;
                }

                repository.SaveAddress(newEvent.Address);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Copy string array to list of event activities
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="activities"></param>
        /// <returns></returns>
        public List<EventActivity> CopyToEventActivities(int? eventId, string[] activities)
        {
            var eventActivities = new List<EventActivity>();

            foreach (var activity in activities)
            {
                eventActivities.Add(new EventActivity()
                {
                    ActivityId = GetActivityId(activity) ?? 0,
                    EventId = eventId ?? 0,
                    Activity = new Activity(),
                    Event = new Event(),
                });
            }

            return eventActivities;
        }

        /// <summary>
        /// Deletes participant
        /// </summary>
        /// <param name="participantData"></param>
        /// <returns></returns>
        public bool LeaveEvent(ParticipantData participantData)
        {
            if (participantData.UserId == default(int) || participantData.EventId == default(int))
            {
                return false;
            }

            var ev = repository.GetEvent(participantData.EventId);

            if (ev == null)
            {
                return false;
            }

            var participant = repository.GetParticipant(participantData.EventId, participantData.UserId);

            return repository.DeleteParticipant(participant);
        }

        /// <summary>
        /// Returns activity Id
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public int? GetActivityId(string activity)
        {
            return repository.GetActivities().FirstOrDefault(a => a.Title == activity)?.Id;
        }
        
        /// <summary>
        /// Returns copy of EventData type as Event type
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public Event CopyEventDataToEvent(EventData eventData)
        {
            Event.EventTypes type;

            switch (eventData.Type)
            {
                case "Public":
                    type = Event.EventTypes.Public;
                    break;
                case "Private":
                    type = Event.EventTypes.Private;
                    break;
                default:
                    type = Event.EventTypes.Public;
                    break;
            }

            var newEvent = new Event()
            {
                Title = eventData.Title,
                DateFrom = Convert.ToDateTime(eventData.DateFrom),
                DateTo = Convert.ToDateTime(eventData.DateTo),
                Address = GetAddress(eventData.Address),
                Description = eventData.Description,
                UserId = eventData.UserId,
                Type = (int)type,
            };

            return newEvent;
        }

        /// <summary>
        /// Converts array of strings to Address
        /// </summary>
        /// <param name="adrs"></param>
        /// <returns></returns>
        public Address GetAddress(string[] adrs)
        {
            var address = new Address()
            {
                AddressLine1 = adrs[0],
                AddressLine2 = adrs[1],
                Province = adrs[2],
                City = adrs[3],
                Country = adrs[4],
                PostalCode = adrs[5],
            };

            return address;
        }

        /// <summary>
        /// Updates event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public bool PatchEvent(int eventId, EventData eventData)
        {
            var ev = repository.GetEvent(eventId);

            if (ev == null)
            {
                return false;
            }

            var newAddress = GetAddress(eventData.Address);

            if (ev.Address != newAddress)
            {
                ev.Address.Country = newAddress.Country;
                ev.Address.AddressLine1 = newAddress.AddressLine1;
                ev.Address.AddressLine2 = newAddress.AddressLine2;
                ev.Address.City = newAddress.City;
                ev.Address.Province = newAddress.Province;
                ev.Address.PostalCode = newAddress.PostalCode;

                repository.UpdateAddress(ev.Address);
            }

            var dateFrom = Convert.ToDateTime(eventData.DateFrom);
            ev.DateFrom = dateFrom != ev.DateFrom ? dateFrom : ev.DateFrom;
            var dateTo = Convert.ToDateTime(eventData.DateTo);
            ev.DateFrom = dateTo != ev.DateTo ? dateTo : ev.DateTo;
            ev.Description = eventData.Description != ev.Description ? eventData.Description : ev.Description;
            ev.Title = eventData.Title != ev.Title ? eventData.Title : ev.Title;

            return repository.Update(ev);
        }

        /// <summary>
        /// Removes activity from event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="activityData"></param>
        /// <returns></returns>
        public bool RemoveActivityFromEvent(int eventId, ActivityData activityData)
        {
            var ev = repository.GetEvent(eventId);

            if (ev == null)
            {
                return false;
            }

            var activityId = GetActivityId(activityData.Title);
            var ea = repository.GetEventActivity(eventId, (int)activityId);

            if (ea == null)
            {
                return false;
            }

            return repository.Delete(ea);
        }

        /// <summary>
        /// Adds activities to event
        /// </summary>
        /// <param name="activityData"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public bool AddActivityToEvent(int eventId, ActivityData activityData)
        {
            var ev = GetEvent(eventId);

            if (ev == null)
            {
                return false;
            }

            var ea = GetEventActivities(eventId);

            if (!ea.Exists(x => x.Title == activityData.Title))
            {
                var activityId = GetActivityId(activityData.Title);
                    
                if (activityId == null)
                {
                    if (!CreateActivity(activityData))
                    {
                        return false;
                    }

                    activityId = GetActivityId(activityData.Title);
                }

                var newEventActivity = new EventActivity()
                {
                    EventId = ev.Id,
                    ActivityId = (int)activityId,
                };
                
                return repository.AddEventActivity(newEventActivity);
            }

            return false;
        }

        /// <summary>
        /// Returns event by event Id
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Event GetEvent(int eventId)
        {
            return repository.GetEvent(eventId);
        }

        /// <summary>
        /// Return event activity
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public EventActivity GetEventActivity(int eventId, int activityId)
        {
            return repository.GetEventActivity(eventId, activityId);
        }

        /// <summary>
        /// Creates participant
        /// </summary>
        /// <param name="participant"></param>
        /// <returns></returns>
        public bool ParticipateInEvent(ParticipantData participant)
        {
            if (repository.GetEvent(participant.EventId) == null)
            {
                return false;
            }

            var newParticipant = new Participant()
            {
                EventId = participant.EventId,
                UserId = participant.UserId,
            };
            
            return repository.SaveParticipant(newParticipant);
        }

        /// <summary>
        /// Creates new activity
        /// </summary>
        /// <param name="activityData"></param>
        /// <returns></returns>
        public bool CreateActivity(ActivityData activityData)
        {
            var activities = GetActivities();

            if (activityData != null && activities.FirstOrDefault(a => a.Title == activityData.Title) == null)
            {
                var newActivity = new Activity()
                {
                    Title = activityData.Title,
                    Description = activityData.Description,
                    MaxParticipants = activityData.MaxParticipants,
                    MinParticipants = activityData.MinParticipants,
                    ActivityTypeId = GetActivityTypeId(activityData.ActivityType),
                };

                return repository.SaveActivity(newActivity);
            }

            return false;
        }

        /// <summary>
        /// Creates new activity type
        /// </summary>
        /// <param name="activityType"></param>
        /// <returns></returns>
        public bool CreateActivityType(ActivityTypeData activityType)
        {
            if (activityType != null && repository.GetActivityTypes().FirstOrDefault(a => a.Title == activityType.Title) == null)
            {
                var newActivityType = new ActivityType()
                {
                    Title = activityType.Title
                };

                return repository.SaveActivityType(newActivityType);
            }

            return false;
        }

        /// <summary>
        /// Returns activity type Id
        /// </summary>
        /// <param name="activityType"></param>
        /// <returns></returns>
        public int? GetActivityTypeId(string activityType)
        {
            return repository.GetActivityTypes().FirstOrDefault(t => t.Title == activityType)?.Id;
        }

        /// <summary>
        /// Returns list of activities
        /// </summary>
        /// <returns></returns>
        public List<Activity> GetActivities()
        {
            return repository.GetActivities().ToList();
        }

        /// <summary>
        /// Returns list of activities types
        /// </summary>
        /// <returns></returns>
        public List<ActivityType> GetActivityTypes()
        {
            return repository.GetActivityTypes().ToList();
        }
    }
}
