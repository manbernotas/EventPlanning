using EventPlanning.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventPlanning.BL.Tests
{
    [TestClass]
    public class EventManagerTests
    {
        private EventManager eventManager;
        private EventContext context;
        private DbContextOptions<EventContext> options = new DbContextOptionsBuilder<EventContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

        [TestInitialize]
        public void Initialize()
        {
            context = new EventContext(options);
            eventManager = new EventManager(context);
        }

        [TestCleanup]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
            eventManager = null;
            context = null;
        }
    }
}
