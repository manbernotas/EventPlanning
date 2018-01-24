﻿using Microsoft.EntityFrameworkCore;
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
            return context.Activity.Include(a => a.ActivityType);
        }

        public IQueryable<ActivityType> GetActivityTypes()
        {
            return context.ActivityType;
        }

        public IQueryable<Event> GetEvents()
        {
            return context.Event;
        }

        public IQueryable<Event> GetEventsWithActivities()
        {
            return context.Event.Include(e => e.Activities);
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
    }
}
