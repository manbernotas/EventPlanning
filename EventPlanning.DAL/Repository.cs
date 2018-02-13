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

        public EventActivity GetEventActivity(int EventId, int activityId)
        {
            return context.EventActivity.SingleOrDefault(ea => ea.EventId == EventId && ea.ActivityId == activityId);
        }

        public IQueryable<Event> GetEvents()
        {
            return context.Event;
        }

        public IQueryable<Event> GetUserEvents(int userId)
        {
            return context.Event.Where(e => e.UserId == userId).Include(i => i.Address);
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

        public IQueryable<Participant> GetParticipants(int eventId)
        {
            return context.Participant.Where(p => p.EventId == eventId);
        }

        public Participant GetParticipant(int eventId, int userId)
        {
            return context.Participant.SingleOrDefault(p => p.EventId == eventId && p.UserId == userId);
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
                context.Participant.Add(participant);
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
                context.Event.Attach(eventData);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            
            return true;
        }

        public bool AddEventActivity(EventActivity eventActivity)
        {
            try
            {
                context.EventActivity.Add(eventActivity);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool Delete(EventActivity eventActivity)
        {
            try
            {
                context.EventActivity.Remove(eventActivity);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool AddInvitation(Invitation invitation)
        {
            try
            {
                context.Invitation.Add(invitation);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool DeleteParticipant(Participant participant)
        {
            try
            {
                context.Participant.Remove(participant);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool SaveAddress(Address address)
        {
            try
            {
                context.Address.Add(address);
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool UpdateAddress(Address address)
        {
            try
            {
                context.Address.Attach(address);
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
