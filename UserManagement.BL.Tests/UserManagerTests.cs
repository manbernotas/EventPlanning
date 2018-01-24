using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserManagement.Model;
using UserManagement.DAL;

namespace UserManagement.BL.Tests
{
    [TestClass]
    public class UserManagerTests
    {
        private UserManager userManager;
        private UserContext context;
        private DbContextOptions<UserContext> options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

        [TestInitialize]
        public void Initialize()
        {
            context = new UserContext(options);
            context.User.Add(new User()
            {
                Id = 1,
                UserName = "Test1",
                Password = "uYTTWRFXloUMrWS1n5lZR67N8SLsCcFo2KEsQC/pmEg=", // test
                Salt = "uDTjmWVqzGjYoWUJt4BO3w==",
            });
            context.User.Add(new User()
            {
                Id = 2,
                UserName = "Test2",
                Password = "w4xUdwFZcpP2NjF1I4X5mLyyg2mzVV3hMkK9+MHRHN8=", // test3
                Salt = "6ej1O3xzARQ2WRGbqM8NNw==",
            });

            context.SaveChanges();

            userManager = new UserManager(context);
        }

        [TestCleanup]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
            userManager = null;
            context = null;
        }

        [TestMethod]
        public void GetUsersOK()
        {
            var users = userManager.GetUsers();

            Assert.AreEqual(2, users.Count);
        }

        [TestMethod]
        public void GetPasswordHashOK()
        {
            var passwordHash = userManager.GetPasswordHash("test", "uDTjmWVqzGjYoWUJt4BO3w==");

            Assert.AreEqual("uYTTWRFXloUMrWS1n5lZR67N8SLsCcFo2KEsQC/pmEg=", passwordHash);
        }

        [TestMethod]
        public void IsPasswordValidOK()
        {
            var user = new UserData()
            {
                Name = "Test1",
                Password = "test",
            };

            Assert.IsTrue(userManager.IsPasswordValid(user));
        }

        [TestMethod]
        public void IsPasswordValidFailWrongPassword()
        {
            var user = new UserData()
            {
                Name = "Test1",
                Password = "test2",
            };

            Assert.IsFalse(userManager.IsPasswordValid(user));
        }

        [TestMethod]
        public void IsPasswordValidFailNoPassword()
        {
            var user2 = new UserData()
            {
                Name = "Test1",
            };

            Assert.IsFalse(userManager.IsPasswordValid(user2));
        }

        [TestMethod]
        public void CreateUserOK()
        {
            var user = new UserData()
            {
                Name = "Test3",
                Password = "Test",
            };

            Assert.IsTrue(userManager.CreateUser(user));
        }

        [TestMethod]
        public void CreateUserFail()
        {
            var user = new UserData()
            {
                Name = "Test1",
                Password = "Test",
            };

            Assert.IsFalse(userManager.CreateUser(user));
        }
    }
}
