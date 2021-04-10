using NUnit.Framework;
using DataLayer.Groups;

namespace Telemetry_unit_tests
{
    [TestFixture]
    public class AttributeTests
    {
        [Test]
        [TestCase("attribute", "#fc0505", "attribute", "#fc0505")]
        [TestCase(" attribute", "#fc0505", "attribute", "#fc0505")]
        [TestCase("attribute", " #fc0505", "attribute", "#fc0505")]
        [TestCase(" attribute", " #fc0505", "attribute", "#fc0505")]
        [TestCase("attribute ", " #fc0505", "attribute", "#fc0505")]
        [TestCase(" attribute ", " #fc0505", "attribute", "#fc0505")]
        [TestCase("attribute", "#fc0505 ", "attribute", "#fc0505")]
        [TestCase("attribute", " #fc0505 ", "attribute", "#fc0505")]
        [TestCase(" attribute ", " #fc0505 ", "attribute", "#fc0505")]
        public void CreateAttribute_TestGood(string name, string color, string expectedName, string expectedColor)
        {
            Attribute attribute = new Attribute(name, color);
            Assert.AreEqual(attribute.Name, expectedName);
            Assert.AreEqual(attribute.Color, expectedColor);
        }
    }
}
