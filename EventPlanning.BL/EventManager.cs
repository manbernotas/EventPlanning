using EventPlanning.DAL;
using EventPlanning.Model;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool PatchEvent(EventData eventData, int eventId)
        {
            var ev = repository.GetEvent(eventId);

            if (ev == null)
            {
                return false;
            }

            ev.Address = eventData.Address != ev.Address ? eventData.Address : ev.Address;
            ev.DateFrom = Convert.ToDateTime(eventData.DateFrom) != (ev.DateFrom) ? Convert.ToDateTime(eventData.DateFrom) : ev.DateFrom;
            ev.DateFrom = Convert.ToDateTime(eventData.DateTo) != (ev.DateTo) ? Convert.ToDateTime(eventData.DateTo) : ev.DateTo;
            ev.Description = eventData.Description != ev.Description ? eventData.Description : ev.Description;
            ev.Title = eventData.Title != ev.Title ? eventData.Title : ev.Title;

            return repository.Update(ev);
        }

        public Event GetEvent(int eventId)
        {
            return repository.GetEvent(eventId);
        }

        /// <summary>
        /// Creates participant
        /// </summary>
        /// <param name="participant"></param>
        /// <returns></returns>
        public bool ParticipateInEvent(ParticipantData participant)
        {
            var newParticipant = new Participant()
            {
                EventId = participant.EventId,
                UserId = participant.UserId,
            };

            try
            {
                return repository.SaveParticipant(newParticipant);
            }
            catch (Exception)
            {
                return false;
            }
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

                try
                {
                    return repository.SaveActivity(newActivity);
                }
                catch (Exception)
                {
                    return false;
                }
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

                try
                {
                    return repository.SaveActivityType(newActivityType);
                }
                catch (Exception)
                {
                    return false;
                }
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
