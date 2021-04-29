using NUnit.Framework;
using PresentationLayer.Groups;

namespace UnitTest
{
    [TestFixture]
    public partial class GroupTests
    {
        [Test]
        [TestCase("Gearbox")]
        [TestCase("EngineBase")]
        [TestCase("Gearbox")]
        public void GetGroup_TestGood(string name)
        {
            GroupManager.InitGroups(GOOD_GROUPS_PATH + "groups.json");

            Assert.IsNotNull(GroupManager.GetGroup(name));
        }
    }
}
