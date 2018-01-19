using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventPlanning.DAL
{
    public class Repository
    {
        private EventContext context;

        public Repository(EventContext context)
        {
            this.context = context;
        }

        public List<ActivityType> GetActivityTypes()
        {
            return context.ActivityType.ToList();
        }

        public List<Event> GetEvents()
        {
            return context.Event.ToList();
        }

        public List<User> GetUsers()
        {
            return context.User.ToList();
        }

        public bool SaveActivityType(ActivityType activityType)
        {
            try
            {
                context.ActivityType.Add(activityType);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool SaveUser(User user)
        {
            try
            {
                context.User.Add(user);
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
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool SaveEvent(Event @event)
        {
            try
            {
                context.Event.Add(@event);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
