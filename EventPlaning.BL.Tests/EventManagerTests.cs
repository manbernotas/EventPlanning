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
                Type = (int)Event.EventTypes.Public,
                Address = new Address()
                {
                    AddressLine1 = "Ateities g. 10",
                    City = "Vilnius",
                    Country = "Lithuania",
                    Province = "Vilniaus m.",
                    PostalCode = "08303",
                },
                EventActivities = new List<EventActivity>()
                {
                    new EventActivity()
                    {
                        ActivityId = 1,
                        EventId = 1,
                    },
                    new EventActivity()
                    {
                        ActivityId = 2,
                        EventId = 1,
                    }
                },
                Participants = new List<Participant>()
                {
                    new Participant()
                    {
                        EventId = 1,
                        UserId = 1,
                    },
                    new Participant()
                    {
                        EventId = 1,
                        UserId = 2,
                    },
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
        public void EventContainsOKAddressContains()
        {
            var ev = new Event()
            {
                Address = new Address()
                {
                    AddressLine1 = "New address",
                    City = "Vilnius"
                }
            };

            Assert.IsTrue(eventManager.EventContains(ev, "New"));
        }

        [TestMethod]
        public void EventContainsFailAddressNotContains()
        {
            var ev = new Event()
            {
                Address = new Address()
                {
                    AddressLine1 = "New address",
                    City = "Vilnius"
                }
            };

            Assert.IsFalse(eventManager.EventContains(ev, "Old"));
        }

        [TestMethod]
        public void EventContainsOK()
        {
            var ev = new Event()
            {
                Title = "BG and VG event",
            };

            Assert.IsTrue(eventManager.EventContains(ev, "BG"));
        }

        [TestMethod]
        public void EventContainsFail()
        {
            var ev = new Event()
            {
            };
                

            Assert.IsFalse(eventManager.EventContains(ev, "BG"));
        }

        [TestMethod]
        public void LeaveEventOK()
        {
            var participant = new ParticipantData()
            {
                EventId = 1,
                UserId = 1,
            };

            eventManager.LeaveEvent(participant);

            Assert.AreEqual(1, eventManager.GetParticipants(1));
        }

        [TestMethod]
        public void LeaveEventFailNoUserId()
        {
            var participant = new ParticipantData()
            {
                EventId = 1,
            };

            Assert.IsFalse(eventManager.LeaveEvent(participant));
        }

        [TestMethod]
        public void GetParticipantsOK()
        {
            Assert.AreEqual(2, eventManager.GetParticipants(1));
        }

        [TestMethod]
        public void GetParticipantsFailEventNotExist()
        {
            Assert.AreEqual(0, eventManager.GetParticipants(2));
        }

        [TestMethod]
        public void GetEventActivityOK()
        {
            Assert.AreEqual(1, eventManager.GetEventActivity(1, 1).ActivityId);
        }

        [TestMethod]
        public void GetEventActivityFail()
        {
            Assert.IsNull(eventManager.GetEventActivity(2, 1));
        }

        [TestMethod]
        public void RemoveActivityOK()
        {
            var activityData = new ActivityData()
            {
                Title = "Monopoly",
            };

            eventManager.RemoveActivityFromEvent(1, activityData);

            Assert.AreEqual(1, eventManager.GetEventActivities(1).Count);

            Assert.IsFalse(eventManager.GetEventActivities(1).Exists(x => x.Title == activityData.Title));
        }

        [TestMethod]
        public void RemoveActivityFail()
        {
            var activityData = new ActivityData()
            {
                Title = "Monopoly",
            };

            eventManager.RemoveActivityFromEvent(2, activityData);

            Assert.AreEqual(2, eventManager.GetEventActivities(1).Count);

            Assert.IsTrue(eventManager.GetEventActivities(1).Exists(x => x.Title == activityData.Title));
        }

        [TestMethod]
        public void AddActivitesToEventOK()
        {
            context.Activity.Add(new Activity()
            {
                Id = 3,
                Title = "Ghost stories",
                Description = "Boo",
                MinParticipants = 2,
                MaxParticipants = 6,
                ActivityTypeId = 2,
            });

            context.SaveChanges();

            var activity = new ActivityData()
            {
                Title = "Ghost stories",
                Description = "Boo",
                MinParticipants = 2,
                MaxParticipants = 6,
                ActivityType = "Board game",
            };

            eventManager.AddActivityToEvent(1, activity);

            Assert.AreEqual(3, eventManager.GetEventActivities(1).Count);
        }

        [TestMethod]
        public void AddActivitesToEventFail()
        {
            context.Activity.Add(new Activity()
            {
                Id = 3,
                Title = "Ghost stories",
                Description = "Boo",
                MinParticipants = 2,
                MaxParticipants = 6,
                ActivityTypeId = 2,
            });

            context.SaveChanges();

            var activity = new ActivityData()
            {
                Title = "Ghost stories",
                Description = "Boo",
                MinParticipants = 2,
                MaxParticipants = 6,
                ActivityType = "Board game",
            };

            Assert.IsFalse(eventManager.AddActivityToEvent(2, activity));
        }

        [TestMethod]
        public void GetMaxParticipantsOK()
        {
            Assert.AreEqual(8, eventManager.GetMaxParticipants(1));
        }

        [TestMethod]
        public void GetMaxParticipantsFailEventNotExist()
        {
            Assert.AreEqual(0, eventManager.GetMaxParticipants(2));
        }

        [TestMethod]
        public void PatchEventOK()
        {
            var newEventData = new EventData()
            {
                UserId = 1,
                Description = "Board and video games",
                Title = "BG and VG event",
                DateFrom = "2018-01-23",
                DateTo = "2018-01-23",
                Address = new Address()
                {
                    AddressLine1 = "new g.1",
                    City = "Vilnius",
                    Country = "Lithuania",
                    Province = "Vilniaus m.",
                    PostalCode = "08303",
                },
            };

            eventManager.PatchEvent(1, newEventData);

            Assert.AreEqual("new g.1", eventManager.GetEvent(1).Address.AddressLine1);
        }

        [TestMethod]
        public void PatchEventFailEventNotExist()
        {
            var newEventData = new EventData()
            {
                UserId = 1,
                Description = "Board and video games",
                Title = "BG and VG event",
                DateFrom = "2018-01-23",
                DateTo = "2018-01-23",
                Address = new Address()
                {
                    AddressLine1 = "new g.1",
                    City = "Vilnius",
                    Country = "Lithuania",
                    Province = "Vilniaus m.",
                    PostalCode = "08303",
                },
            };

            Assert.IsFalse(eventManager.PatchEvent(2, newEventData));
        }

        [TestMethod]
        public void GetEventsOK()
        {
            Assert.AreEqual(1, eventManager.GetEvents("BG and VG").Count);
        }

        [TestMethod]
        public void GetEventsOKWithNullTitle()
        {
            context.Event.Add(new Event()
            {
                Id = 2,
                UserId = 1,
                Description = "Board games",
                DateFrom = new DateTime(2018, 01, 23),
                DateTo = new DateTime(2018, 01, 23),
                Address = new Address()
                {
                    AddressLine1 = "Ateities g. 10",
                    City = "Vilnius",
                    Country = "Lithuania",
                    Province = "Vilniaus m.",
                    PostalCode = "08303",
                }
            });

            context.SaveChanges();

            Assert.IsNotNull(eventManager.GetEvents("2"));
        }

        [TestMethod]
        public void GetEventOK()
        {
            Assert.IsNotNull(eventManager.GetEvent(1));
        }

        [TestMethod]
        public void GetEventFail()
        {
            Assert.IsNull(eventManager.GetEvent(2));
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
        public void GetEventsByDateAndTimeOK()
        {
            Assert.AreEqual(1, eventManager.GetEvents(new DateTime(2018, 01, 23, 12, 15, 30),
                new DateTime(2018, 01, 24, 1, 16, 30)).Count);
        }

        [TestMethod]
        public void GetEventsByDateAndTimeFail()
        {
            Assert.AreEqual(0, eventManager.GetEvents(new DateTime(2018, 01, 24, 10, 5, 8),
                 new DateTime(2018, 10, 07, 8, 9, 19)).Count);
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
    }
}
