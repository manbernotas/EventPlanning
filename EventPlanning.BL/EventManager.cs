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
        /// Returns all events with activities
        /// </summary>
        /// <returns></returns>
        public List<Event> GetEventsWithActivities()
        {
            return repository.GetEventsWithActivities().ToList();
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
