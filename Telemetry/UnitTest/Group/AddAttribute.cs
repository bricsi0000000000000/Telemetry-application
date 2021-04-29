using DataLayer.Groups;
using NUnit.Framework;

namespace UnitTest
{
    [TestFixture]
    public partial class GroupTests
    {
        [Test]
        [TestCase("attribute", "#fc0505", 1, "attribute", "#fc0505", 1)]
        [TestCase(" attribute", "#fc0505", 1, "attribute", "#fc0505", 1)]
        [TestCase("attribute", " #fc0505", 1, "attribute", "#fc0505", 1)]
        [TestCase(" attribute", " #fc0505", 1, "attribute", "#fc0505", 1)]
        [TestCase("attribute ", " #fc0505", 1, "attribute", "#fc0505", 1)]
        [TestCase(" attribute ", " #fc0505", 1, "attribute", "#fc0505", 1)]
        [TestCase("attribute", "#fc0505 ", 1, "attribute", "#fc0505", 1)]
        [TestCase("attribute", " #fc0505 ", 1, "attribute", "#fc0505", 1)]
        [TestCase(" attribute ", " #fc0505 ", 1, "attribute", "#fc0505", 1)]
        public void AddAttributeToGroup_Test(string name, string colorText, int lineWidth, string expectedName, string expectedColorText, int expectedintLineWidth)
        {
            Group group = new Group(0, "group");
            group.AddAttribute(name, colorText, lineWidth);
            Assert.AreEqual(group.Attributes[0].Name, expectedName);
            Assert.AreEqual(group.Attributes[0].ColorText, expectedColorText);
            Assert.AreEqual(group.Attributes[0].LineWidth, expectedintLineWidth);
        }
    }
}
