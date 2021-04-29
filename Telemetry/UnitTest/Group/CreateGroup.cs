using DataLayer.Groups;
using NUnit.Framework;

namespace UnitTest
{
    [TestFixture]
    public partial class GroupTests
    {
        [Test]
        [TestCase(0, "proba_group", 0, "proba_group")]
        [TestCase(1, "proba group", 1, "proba group")]
        [TestCase(2, " proba group", 2, "proba group")]
        [TestCase(3, "proba group ", 3, "proba group")]
        [TestCase(4, " proba group ", 4, "proba group")]
        public void CreateGroup_TestGood(int id, string groupName, int expectedID, string expectedGroupName)
        {
            Group group = new Group(id, groupName);
            Assert.AreEqual(group.Name, expectedGroupName);
            Assert.AreEqual(group.ID, expectedID);
        }

        [Test]
        [TestCase("proba_group", "proba group")]
        [TestCase("proba group", "proba_group")]
        [TestCase(" proba group", " proba group")]
        [TestCase("proba group ", "proba group ")]
        [TestCase(" proba group ", " proba group ")]
        public void CreateGroup_TestBad(string groupName, string expectedGroupName)
        {
            Group group = new Group(0, groupName);
            Assert.AreNotEqual(group.Name, expectedGroupName);
        }
    }
}
