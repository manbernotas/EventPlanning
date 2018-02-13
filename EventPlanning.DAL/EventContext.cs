using Microsoft.EntityFrameworkCore;

namespace EventPlanning.DAL
{
    public class EventContext : DbContext
    {
        public EventContext(DbContextOptions<EventContext> options) : base(options)
        {
        }

        public DbSet<Address> Address { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<ActivityType> ActivityType { get; set; }
        public DbSet<Participant> Participant { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventActivity> EventActivity { get; set; }
        public DbSet<Invitation> Invitation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>();
            modelBuilder.Entity<ActivityType>();
            modelBuilder.Entity<Activity>();
            modelBuilder.Entity<EventActivity>().HasKey(ea => new { ea.ActivityId, ea.EventId });
            modelBuilder.Entity<Participant>().HasKey(p => new { p.EventId, p.UserId });
            modelBuilder.Entity<Event>();
            modelBuilder.Entity<Invitation>().HasKey(i => new { i.UserId, i.EventId });
        }
    }
}
