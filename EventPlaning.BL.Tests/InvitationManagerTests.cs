using EventPlanning.DAL;
using EventPlanning.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventPlanning.BL.Tests
{
    class InvitationManagerTests
    {
        [TestClass]
        public class EventManagerTests
        {
            private EventManager eventManager;
            private InvitationManager invitationManager;
            private EventContext context;
            private DbContextOptions<EventContext> options = new DbContextOptionsBuilder<EventContext>()
                    .UseInMemoryDatabase("TestDatabase")
                    .Options;

            [TestInitialize]
            public void Initialize()
            {
                context = new EventContext(options);
                eventManager = new EventManager(context);
                Mock<ISmtpClient> smtpClient = new Mock<ISmtpClient>();
                invitationManager = new InvitationManager(context, smtpClient.Object);

                context.Event.Add(new Event()
                {
                    Id = 1,
                    UserId = 1,
                    Description = "Board and video games",
                    Title = "BG and VG event",
                    DateFrom = new DateTime(2018, 01, 23),
                    DateTo = new DateTime(2018, 01, 23),
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
            public void InviteToEventOK()
            {
                var user = new UserData()
                {
                    Email = "eventinvitator@gmail.com",
                    Name = "Event",
                    Id = 2,
                };

                Assert.IsTrue(invitationManager.InviteToEvent(1, user));
            }

            [TestMethod]
            public void InviteToEventFailEventNotExist()
            {
                var user = new UserData()
                {
                    Name = "UserName",
                };

                Assert.IsFalse(invitationManager.InviteToEvent(2, user));
            }

            [TestMethod]
            public void SendMailOK()
            {
                var user = new UserData()
                {
                    Email = "eventinvitator@gmail.com",
                    Name = "Event",
                    Id = 2,
                };

                Assert.IsTrue(invitationManager.SendMail(1, user));
            }

            [TestMethod]
            public void SendMailFailIncorrectEmailFormat()
            {
                var user = new UserData()
                {
                    Email = "eventinvitator",
                    Name = "Event",
                    Id = 2,
                };

                Assert.IsFalse(invitationManager.SendMail(1, user));
            }

            [TestMethod]
            public void SendInvitationOK()
            {
                var user = new UserData()
                {
                    Email = "eventinvitator@gmail",
                    Name = "Event",
                    Id = 2,
                };

                Assert.AreEqual("Email", invitationManager.SendInvitation(1, user));
            }

            [TestMethod]
            public void SendInvitationFailNoEmailOrId()
            {
                var user = new UserData()
                {
                    Name = "Event",
                };

                Assert.AreEqual(string.Empty, invitationManager.SendInvitation(1, user));
            }

            [TestMethod]
            public void CreateInvitationOK()
            {
                var user = new UserData()
                {
                    Email = "eventinvitator@gmail.com",
                    Name = "Event",
                    Id = 2,
                };

                Assert.IsTrue(invitationManager.CreateInvitation(1, user, "Email"));
            }

            [TestMethod]
            public void CreateInvitationFailSentToEmpty()
            {
                var user = new UserData()
                {
                    Name = "Event",
                };

                Assert.IsFalse(invitationManager.CreateInvitation(1, user, string.Empty));
            }
        }
    }
}
