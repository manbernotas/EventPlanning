using EventPlanning.DAL;
using EventPlanning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
        /// Return all events
        /// </summary>
        /// <returns></returns>
        public List<Event> GetEvents()
        {
            return repository.GetEvents().ToList();
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
            if (ev != null)
            {
                var partialEvent = new PartialEvent()
                {
                    Title = ev.Title,
                    Creator = GetUserName(ev.UserId),
                    Participants = GetParticipants(ev.Id),
                    MaxParticipants = GetMaxParticipants(ev.Id),
                };

                return partialEvent;
            }

            return null;
        }

        private int GetMaxParticipants(int id)
        {
            throw new NotImplementedException();
        }

        public string GetUserName(int userId)
        {
            // Should I call user service here?
            throw new NotImplementedException();
        }

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
                .Where(e => (e.Address != null && e.Address.Contains(pattern))
                         || (e.Title != null && e.Title.Contains(pattern))
                         || (e.Description != null && e.Description.Contains(pattern))
                         || (e.UserId.ToString().Contains(pattern))
                         || (e.Id.ToString().Contains(pattern)))
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
                .Where(e => e.DateFrom >= dateFrom.Date && e.DateTo <= dateTo.Date)
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
            var events = GetEvents();

            if (eventData != null && events.FirstOrDefault(e => e.Title == eventData.Title) == null)
            {
                var newEvent = CopyEventDataToEvent(eventData);
                
                try
                {
                    repository.SaveEvent(newEvent);
                    var eventActivities = new List<EventActivity>();

                    foreach (var activity in eventData.Activities)
                    {
                        eventActivities.Add(new EventActivity()
                        {
                            ActivityId = GetActivityId(activity) ?? 0,
                            EventId = GetEventId(newEvent.Title) ?? 0,
                        });
                    }
                    repository.SaveEventActivities(eventActivities);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
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
        /// Returns event Id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int? GetEventId(string title)
        {
            return repository.GetEvents().FirstOrDefault(e => e.Title == title)?.Id;
        }
        
        /// <summary>
        /// Returns copy of EventData type as Event type
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public Event CopyEventDataToEvent(EventData eventData)
        {
            var newEvent = new Event()
            {
                Title = eventData.Title,
                DateFrom = Convert.ToDateTime(eventData.DateFrom),
                DateTo = Convert.ToDateTime(eventData.DateTo),
                Address = eventData.Address,
                Description = eventData.Description,
                UserId = eventData.UserId,
            };

            return newEvent;
        }

        public bool PatchEvent(int eventId, EventData eventData)
        {
            var ev = repository.GetEvent(eventId);

            if (ev == null)
            {
                return false;
            }

            ev.Address = eventData.Address != ev.Address ? eventData.Address : ev.Address;
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
