using NUnit.Framework;
using DataLayer.Groups;

namespace UnitTest
{
    [TestFixture]
    public class AttributeTests
    {
        [Test]
        [TestCase(0, "attribute", "#fc0505", 1, 0, "attribute", "#fc0505", 1)]
        [TestCase(1, " attribute", "#fc0505", 1, 1, "attribute", "#fc0505", 1)]
        [TestCase(2, "attribute", " #fc0505", 1, 2, "attribute", "#fc0505", 1)]
        [TestCase(3, " attribute", " #fc0505", 1, 3, "attribute", "#fc0505", 1)]
        [TestCase(4, "attribute ", " #fc0505", 1, 4, "attribute", "#fc0505", 1)]
        [TestCase(5, " attribute ", " #fc0505", 1, 5, "attribute", "#fc0505", 1)]
        [TestCase(6, "attribute", "#fc0505 ", 1, 6, "attribute", "#fc0505", 1)]
        [TestCase(7, "attribute", " #fc0505 ", 1, 7, "attribute", "#fc0505", 1)]
        [TestCase(8, " attribute ", " #fc0505 ", 1, 8, "attribute", "#fc0505", 1)]
        public void CreateAttribute_TestGood(int id, string name, string colorText, int lineWidth, int expectedID, string expectedName, string expectedColorText, int expectedintLineWidth)
        {
            Attribute attribute = new Attribute(id, name, colorText, lineWidth);
            Assert.AreEqual(expectedID, attribute.ID);
            Assert.AreEqual(expectedName, attribute.Name);
            Assert.AreEqual(expectedColorText, attribute.ColorText);
            Assert.AreEqual(expectedintLineWidth, attribute.LineWidth);
        }
    }
}
