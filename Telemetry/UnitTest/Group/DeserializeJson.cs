using NUnit.Framework;
using PresentationLayer.Groups;
using System;

namespace UnitTest
{
    [TestFixture]
    public partial class GroupTests
    {
        [Test]
        [TestCase(GOOD_GROUPS_PATH + "groups.json")]
        public void DeserializeJson_AllGood(string fileName)
        {
            Assert.DoesNotThrow(() => GroupManager.InitGroups(fileName));
        }

        [Test]
        [TestCase(GOOD_GROUPS_PATH + "grouops.json")]
        public void DeserializeJson_FileNotFound(string fileName)
        {
            Assert.Throws<Exception>(() => GroupManager.InitGroups(fileName));
        }

        [Test]
        [TestCase(WRONG_GROUPS_PATH + "missing_curly_bracket.json")]
        [TestCase(WRONG_GROUPS_PATH + "wrong_groups/empty_name.json")]
        [TestCase(WRONG_GROUPS_PATH + "wrong_groups/null_name.json")]
        [TestCase(WRONG_GROUPS_PATH + "wrong_groups/null_customizable.json")]
        [TestCase(WRONG_GROUPS_PATH + "wrong_groups/null_driverless.json")]
        [TestCase(WRONG_GROUPS_PATH + "wrong_groups/null_attribute_name.json")]
        [TestCase(WRONG_GROUPS_PATH + "wrong_groups/null_attribute_color.json")]
        [TestCase(WRONG_GROUPS_PATH + "wrong_groups/empty.json")]
        public void DeserializeJson_MissingCurlyBrackets(string fileName)
        {
            Assert.Throws<Exception>(() => GroupManager.InitGroups(fileName));
        }
    }
}
