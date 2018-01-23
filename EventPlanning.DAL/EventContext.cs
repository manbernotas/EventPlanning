using Microsoft.EntityFrameworkCore;

namespace EventPlanning.DAL
{
    public class EventContext : DbContext
    {
        public EventContext(DbContextOptions<EventContext> options) : base(options)
        {
        }

        public DbSet<Activity> Activity { get; set; }
        public DbSet<ActivityType> ActivityType { get; set; }
        public DbSet<Participant> Particiant { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventActivity> EventActivity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityType>();
            modelBuilder.Entity<Activity>();
            modelBuilder.Entity<EventActivity>();
            modelBuilder.Entity<Participant>();
            modelBuilder.Entity<Event>();
        }
    }
}
