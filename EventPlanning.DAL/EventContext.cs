using Microsoft.EntityFrameworkCore;

namespace EventPlanning.DAL
{
    public class EventContext : DbContext
    {
        public EventContext(DbContextOptions<EventContext> options) : base(options)
        {
        }

        public DbSet<IEventType> BoardGame { get; set; }
        public DbSet<Participant> User { get; set; }
        public DbSet<Event> Event { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IEventType>();
            modelBuilder.Entity<Participant>();
            modelBuilder.Entity<Event>();
        }
    }
}
