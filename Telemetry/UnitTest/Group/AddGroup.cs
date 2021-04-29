using DataLayer.Groups;
using NUnit.Framework;
using PresentationLayer.Groups;

namespace UnitTest
{
    [TestFixture]
    public partial class GroupTests
    {
        [Test]
        [TestCase("group0", "group0")]
        [TestCase(" group1", "group1")]
        [TestCase("group2 ", "group2")]
        [TestCase(" group3 ", "group3")]
        [TestCase(" group_3 ", "group_3")]
        public void AddGroup_TestGood(string name, string expectedName)
        {
            GroupManager.AddGroup(new Group(0, name));
            Assert.IsNotNull(GroupManager.GetGroup(expectedName));
        }
    }
}
