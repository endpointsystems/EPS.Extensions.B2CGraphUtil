using System.Threading.Tasks;
using Microsoft.Graph;
using NUnit.Framework;

namespace EPS.Extensions.B2CGraphUtil.Test
{
    [TestFixture]
    public class GroupsTest: TestBase
    {

        protected GroupsRepo repo;

        [OneTimeSetUp]
        public void Setup()
        {
            repo = new GroupsRepo(Config);
        }
        [Test]
        public async Task CreateAndDeleteGroupTest()
        {
            var g = await repo.CreateGroup("test");
            await repo.DeleteGroup(g.Id);
        }

        [Test]
        public async Task GetAllGroupsTest()
        {
            var list = await repo.GetAllGroups();
            Assert.True(list.Count > 0);
        }

    }
}
