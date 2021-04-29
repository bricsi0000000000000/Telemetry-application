using DataLayer.Groups;
using NUnit.Framework;
using PresentationLayer.Groups;

namespace UnitTest
{
    [TestFixture]
    public partial class GroupTests
    {
        [Test]
        [TestCase("group00", "group00")]
        [TestCase(" group01", "group01")]
        [TestCase("group02 ", "group02")]
        [TestCase(" group03 ", "group03")]
        [TestCase(" group_03 ", "group_03")]
        public void RemoveGroup_TestGood(string name, string expectedName)
        {
            GroupManager.AddGroup(new Group(0, name));
            GroupManager.RemoveGroup(expectedName);
            Assert.IsNull(GroupManager.GetGroup(expectedName));
        }
    }
}
