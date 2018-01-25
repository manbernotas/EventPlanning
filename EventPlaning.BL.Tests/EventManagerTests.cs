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
                Activities = new List<EventActivity>()
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

        //[TestMethod]
        //public void CreateActivityOK()
        //{
        //    var activity = new ActivityData()
        //    {
        //        Name = "Pandemic",
        //        ActivityType = "Board game",
        //        Description = "Save the world!",
        //        MinParticipants = 1,
        //        MaxParticipants = 4,
        //    };

        //    Assert.IsTrue(eventManager.CreateActivity(activity));
        //}

        //[TestMethod]
        //public void CreateActivityFail()
        //{
        //    var activity = new ActivityData()
        //    {
        //        Name = "Battle city",
        //        ActivityType = "Board game",
        //        Description = "Save the world!",
        //        MinParticipants = 1,
        //        MaxParticipants = 4,
        //    };

        //    Assert.IsFalse(eventManager.CreateActivity(activity));
        //}

        //[TestMethod]
        //public void CreateActivityTypeOK()
        //{
        //    var activityType = new ActivityTypeData()
        //    {
        //        Name = "Puzzle",
        //    };

        //    Assert.IsTrue(eventManager.CreateActivityType(activityType));
        //}

        //[TestMethod]
        //public void CreateActivityTypeFail()
        //{
        //    var activityType = new ActivityTypeData()
        //    {
        //        Name = "Board game",
        //    };

        //    Assert.IsFalse(eventManager.CreateActivityType(activityType));
        //}

        //[TestMethod]
        //public void CreateEventOK()
        //{
        //    var eventData = new EventData()
        //    {
        //        Name = "Board game challenge",
        //        Date = "2018-02-08",
        //        Address = "Ateities g. 4F, Vilnius",
        //        Duration = "5",
        //        MinParticipants = 4,
        //        MaxParticipants = 12,
        //        Activities = new string[1] { "Monopoly" },
        //    };

        //    Assert.IsTrue(eventManager.CreateEvent(eventData));
        //}

        //[TestMethod]
        //public void CreateEventFailNameExists()
        //{
        //    var eventData = new EventData()
        //    {
        //        Name = "BG and VG event",
        //        Date = "2018-02-08",
        //        Address = "Ateities g. 4F, Vilnius",
        //        Duration = "5",
        //        MinParticipants = 4,
        //        MaxParticipants = 12,
        //        Activities = new string[1] { "Monopoly" },
        //    };

        //    Assert.IsFalse(eventManager.CreateEvent(eventData));
        //}
    }
}
