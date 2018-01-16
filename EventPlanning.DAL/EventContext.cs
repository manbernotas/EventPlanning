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
        public DbSet<Participant> User { get; set; }
        public DbSet<Event> Event { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityType>();
            modelBuilder.Entity<Activity>();
            modelBuilder.Entity<Participant>();
            modelBuilder.Entity<Event>();
        }
    }
}
