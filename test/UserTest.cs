using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable MethodHasAsyncOverload

namespace EPS.Extensions.B2CGraphUtil.Test
{
    [TestFixture]
    public class UserTest: TestBase
    {
        protected UserRepo repo;
        [OneTimeSetUp]
        public void Setup()
        {
            repo = new UserRepo(Config);
        }

        [Test]
        [Order(2)]
        public async Task UserExistsTest()
        {
            Assert.IsTrue(await repo.Exists($"fred.flintstone@{Tenant}"));
        }

        [Test]
        [Order(1)]
        public async Task CreateUserTest()
        {
            var user = await repo.AddUser("fred", "flintstone", "fred flintstone","my pretty good password!01");
            var userId = user.Id;
            TestContext.Out.WriteLine($"User {userId} created.");
            Assert.IsNotNull(userId);
            await repo.DeleteUser(userId);
            TestContext.Out.WriteLine($"User {userId} deleted.");
        }

        [Test]
        [Order(3)]
        public async Task CheckUserMembershipTest()
        {
            var dir = await repo.MemberOf("bd618a82-0d63-423b-9a91-442d37fd6fc2");
            Assert.True(dir.Count > 0);
        }

        [Test]
        [Order(4)]
        public async Task create_a_large_amt_of_users_and_list_them()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < 2000; i++)
            {
                var first = Unique.Unique.Generate(15, 0);
                var last = Unique.Unique.Generate(15,0);
                var disp = first + " " + last;
                var pwd = Unique.Unique.Generate(15, 3);

                tasks.Add(repo.AddUser(first,last,disp,pwd));
                Thread.Sleep(50);
            }

            Task.WaitAll(tasks.ToArray());
            var users = await repo.GetAllUsers();

            foreach (var user in users)
            {
                await repo.DeleteUser(user.Id);
            }
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            var tasks = new List<Task>();
            var users = await repo.GetAllUsers();
            foreach (var user in users)
            {
                tasks.Add(repo.DeleteUser(user.Id));
                Thread.Sleep(25);
            }

            Task.WaitAll(tasks.ToArray());
        }

    }
}
