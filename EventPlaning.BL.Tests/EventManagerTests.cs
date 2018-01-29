using EventPlanning.DAL;
using EventPlanning.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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

            context.Event.Add(new Event()
            {
                Id = 1,
                UserId = 1,
                Description = "Board and video games",
                Title = "BG and VG event",
                DateFrom = new DateTime(2018, 01, 23),
                DateTo = new DateTime(2018, 01, 23),
                Address = "Test g. 93-3",
                EventActivities = new List<EventActivity>()
                {
                    new EventActivity()
                    {
                        Id = 1,
                        ActivityId = 1,
                        EventId = 1,
                    },
                    new EventActivity()
                    {
                        Id = 2,
                        ActivityId = 2,
                        EventId = 1,
                    }
                }
            });

            context.Activity.Add(new Activity()
            {
                Id = 1,
                Title = "Battle city",
                Description = "Tanks",
                MinParticipants = 1,
                MaxParticipants = 2,
                ActivityTypeId = 1,
            });

            context.Activity.Add(new Activity()
            {
                Id = 2,
                Title = "Monopoly",
                Description = "Everybody knows it",
                MinParticipants = 2,
                MaxParticipants = 6,
                ActivityTypeId = 2,
            });

            context.ActivityType.Add(new ActivityType()
            {
                Id = 1,
                Title = "Video game",
            });

            context.ActivityType.Add(new ActivityType()
            {
                Id = 2,
                Title = "Board game",
            });

            context.SaveChanges();
        }

        [TestCleanup]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
            eventManager = null;
            context = null;
        }

        [TestMethod]
        public void GetEventsOK()
        {
            Assert.AreEqual(1, eventManager.GetEvents("BG and VG").Count);
        }

        [TestMethod]
        public void GetEventsFail()
        {
            Assert.AreEqual(0, eventManager.GetEvents("NotExistingString").Count);
        }

        [TestMethod]
        public void GetEventsByDateOK()
        {
            Assert.AreEqual(1, eventManager.GetEvents(new DateTime(2018, 01, 23),
                new DateTime(2018, 01, 24)).Count);
        }

        [TestMethod]
        public void GetEventsByDateFail()
        {
            Assert.AreEqual(0, eventManager.GetEvents(new DateTime(2018, 01, 24),
                 new DateTime(2018, 10, 07)).Count);
        }

        [TestMethod]
        public void GetUserEventsOK()
        {
            Assert.AreEqual(1, eventManager.GetUserEvents(1).Count);
        }

        [TestMethod]
        public void GetUserEventsFail()
        {
            Assert.AreEqual(0, eventManager.GetUserEvents(2).Count);
        }

        [TestMethod]
        public void GetEventActivitiesOK()
        {
            Assert.AreEqual(2, eventManager.GetEventActivities(1).Count);
        }

        [TestMethod]
        public void GetEventActivitiesFail()
        {
            Assert.AreEqual(0, eventManager.GetUserEvents(2).Count);
        }

        [TestMethod]
        public void GetActivitiesOK()
        {
            Assert.AreEqual(2, eventManager.GetActivities().Count);
        }

        [TestMethod]
        public void GetActivityTypesOK()
        {
            Assert.AreEqual(2, eventManager.GetActivityTypes().Count);
        }

        [TestMethod]
        public void GetActivityTypeIdOK()
        {
            Assert.AreEqual(2, eventManager.GetActivityTypeId("Board game"));
        }

        [TestMethod]
        public void GetActivityTypeIdFail()
        {
            Assert.IsNull(eventManager.GetActivityTypeId("Board"));
        }

        [TestMethod]
        public void GetActivityIdOK()
        {
            Assert.AreEqual(2, eventManager.GetActivityId("Monopoly"));
        }

        [TestMethod]
        public void GetActivityIdFail()
        {
            Assert.IsNull(eventManager.GetActivityId("Mono"));
        }

        [TestMethod]
        public void GetEventIdOK()
        {
            Assert.AreEqual(1, eventManager.GetEventId("BG and VG event"));
        }

        [TestMethod]
        public void GetEventIdFail()
        {
            Assert.IsNull(eventManager.GetEventId("Mono"));
        }
    }
}
