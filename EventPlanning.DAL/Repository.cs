using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventPlanning.DAL
{
    public class Repository
    {
        private EventContext context;

        public Repository(EventContext context)
        {
            this.context = context;
        }

        public IQueryable<Activity> GetActivities()
        {
            return context.Activity;
        }

        public IQueryable<ActivityType> GetActivityTypes()
        {
            return context.ActivityType;
        }

        public IQueryable<Event> GetEvents()
        {
            return context.Event;
        }

        public IQueryable<Event> GetUserEvents(int userId)
        {
            return context.Event.Where(e => e.UserId == userId);
        }

        public IQueryable<Activity> GetEventActivities(int eventId)
        {
            return context.EventActivity.Where(e => e.EventId == eventId).Select(ea => ea.Activity);
        }

        public bool SaveActivityType(ActivityType activityType)
        {
            try
            {
                context.ActivityType.Add(activityType);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool SaveActivity(Activity activity)
        {
            try
            {
                context.Activity.Add(activity);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool SaveEvent(Event newEvent)
        {
            try
            {
                context.Event.Add(newEvent);
                context.SaveChanges();
            }
            catch (DbUpdateException) 
            {
                return false;
            }

            return true;
        }

        public bool SaveEventActivities(List<EventActivity> eventActivities)
        {
            try
            {
                context.EventActivity.AddRange(eventActivities);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool SaveParticipant(Participant participant)
        {
            try
            {
                context.Particiant.Add(participant);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public Event GetEvent(int eventId)
        {
            return context.Event.FirstOrDefault(e => e.Id == eventId);
        }

        public bool Update(Event eventData)
        {
            try
            {
                context.Event.Update(eventData);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            
            return true;
        }
    }
}
