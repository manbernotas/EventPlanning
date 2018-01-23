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
        /// Creates new event
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public bool CreateEvent(EventData eventData)
        {
            var events = GetEvents();

            if (eventData != null && events.FirstOrDefault(e => e.Name == eventData.Name) == null)
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
                            EventId = GetEventId(newEvent.Name) ?? 0,
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
        private int? GetActivityId(string activity)
        {
            return repository.GetActivities().FirstOrDefault(a => a.Name == activity)?.Id;
        }

        /// <summary>
        /// Returns event Id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int? GetEventId(string name)
        {
            return repository.GetEvents().FirstOrDefault(e => e.Name == name)?.Id;
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
                Name = eventData.Name,
                Date = Convert.ToDateTime(eventData.Date),
                Address = eventData.Address,
                Duration = eventData.Duration,
                MaxParticipants = eventData.MaxParticipants,
                MinParticipants = eventData.MinParticipants,
            };

            return newEvent;
        }

        /// <summary>
        /// Creates new activity
        /// </summary>
        /// <param name="activityData"></param>
        /// <returns></returns>
        public bool CreateActivity(ActivityData activityData)
        {
            var activities = GetActivities();

            if (activityData != null && activities.FirstOrDefault(a => a.Name == activityData.Name) == null)
            {
                var newActivity = new Activity()
                {
                    Name = activityData.Name,
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
            if (activityType != null && repository.GetActivityTypes().FirstOrDefault(a => a.Name == activityType.Name) == null)
            {
                var newActivityType = new ActivityType()
                {
                    Name = activityType.Name
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
            return repository.GetActivityTypes().FirstOrDefault(t => t.Name == activityType)?.Id;
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
