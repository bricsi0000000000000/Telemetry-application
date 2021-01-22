using NUnit.Framework;
using System;
using System.IO;
using Telemetry_data_and_logic_layer.Groups;
using Telemetry_data_and_logic_layer.Texts;

namespace Telemetry_unit_tests
{
    [TestFixture]
    public class GroupTests
    {
        [Test]
        [TestCase("proba_group", "proba_group")]
        [TestCase("proba group", "proba group")]
        [TestCase(" proba group", "proba group")]
        [TestCase("proba group ", "proba group")]
        [TestCase(" proba group ", "proba group")]
        public void CreateGroup_TestGood(string groupName, string expectedGroupName)
        {
            Group group = new Group(groupName);
            Assert.AreEqual(group.Name, expectedGroupName);
        }

        [Test]
        [TestCase("proba_group", "proba group")]
        [TestCase("proba group", "proba_group")]
        [TestCase(" proba group", " proba group")]
        [TestCase("proba group ", "proba group ")]
        [TestCase(" proba group ", " proba group ")]
        public void CreateGroup_TestBad(string groupName, string expectedGroupName)
        {
            Group group = new Group(groupName);
            Assert.AreNotEqual(group.Name, expectedGroupName);
        }

        [Test]
        [TestCase("attribute", "#fc0505", "attribute", "#fc0505")]
        [TestCase(" attribute", "#fc0505", "attribute", "#fc0505")]
        [TestCase("attribute", " #fc0505", "attribute", "#fc0505")]
        [TestCase(" attribute", " #fc0505", "attribute", "#fc0505")]
        [TestCase("attribute ", " #fc0505", "attribute", "#fc0505")]
        [TestCase(" attribute ", " #fc0505", "attribute", "#fc0505")]
        public void AddAttributeToGroup_Test(string attributeName, string attributeColor, string expectedAttributeName, string expectedAttributeColor)
        {
            Group group = new Group("group");
            group.AddAttribute(attributeName, attributeColor);
            Assert.AreEqual(group.Attributes[0].Name, expectedAttributeName);
            Assert.AreEqual(group.Attributes[0].Color, expectedAttributeColor);
        }

        [Test]
        public void InitGroup_TestGood()
        {
            GroupManager.InitGroups("../../../good_input_files/groups.json");

            Group group1 = new Group("EngineMapping");
            group1.AddAttribute("rev", "#FFF1B5B5");
            group1.AddAttribute("ath", "#FF5F1818");
            group1.AddAttribute("lam", "#FFFCE705");
            group1.AddAttribute("ign_1", "#FF80FC05");
            group1.AddAttribute("ti_1", "#FFBA8A8A");
            group1.AddAttribute("gear", "#FF8005FC");
            group1.AddAttribute("speed", "#FFFC0505");

            var group2 = GroupManager.GetGroup("EngineMapping");
            Assert.AreEqual(group1.Name, group2.Name);
            for (int i = 0; i < group1.Attributes.Count; i++)
            {
                Assert.AreEqual(group1.Attributes[i].Name, group2.Attributes[i].Name);
                Assert.AreEqual(group1.Attributes[i].Color, group2.Attributes[i].Color);
            }
        }

        [Test]
        public void InitGroup_TestBad()
        {
            GroupManager.InitGroups("../../../good_input_files/groups.json");

            Group group1 = new Group("EngineMapoping");
            group1.AddAttribute("ver", "#FFF1B5B6");
            group1.AddAttribute("athh", "#FF5F1815");
            group1.AddAttribute("lame", "#FFFCE704");
            group1.AddAttribute("ign1", "#FF80FC03");
            group1.AddAttribute("ti1", "#FFBA8A83");
            group1.AddAttribute("geer", "#FF8005F2");
            group1.AddAttribute("spead", "#FFFC0501");

            var group2 = GroupManager.GetGroup("EngineMapping");
            Assert.AreNotEqual(group1.Name, group2.Name);
            for (int i = 0; i < group1.Attributes.Count; i++)
            {
                Assert.AreNotEqual(group1.Attributes[i].Name, group2.Attributes[i].Name);
                Assert.AreNotEqual(group1.Attributes[i].Color, group2.Attributes[i].Color);
            }
        }

        [Test]
        [TestCase("EngineMapping")]
        [TestCase("EngineBase")]
        [TestCase("Gearbox")]
        public void GetGroup_TestGood(string name)
        {
            GroupManager.InitGroups("../../../good_input_files/groups.json");

            Assert.IsNotNull(GroupManager.GetGroup(name));
        }

        [Test]
        [TestCase("group0", "group0")]
        [TestCase(" group1", "group1")]
        [TestCase("group2 ", "group2")]
        [TestCase(" group3 ", "group3")]
        [TestCase(" group_3 ", "group_3")]
        public void AddGroup_TestGood(string name, string expectedName)
        {
            GroupManager.AddGroup(new Group(name));
            Assert.IsNotNull(GroupManager.GetGroup(expectedName));
        }

        [Test]
        [TestCase("group00", "group00")]
        [TestCase(" group01", "group01")]
        [TestCase("group02 ", "group02")]
        [TestCase(" group03 ", "group03")]
        [TestCase(" group_03 ", "group_03")]
        public void RemoveGroup_TestGood(string name, string expectedName)
        {
            GroupManager.AddGroup(new Group(name));
            GroupManager.RemoveGroup(expectedName);
            Assert.IsNull(GroupManager.GetGroup(expectedName));
        }

        [Test]
        [TestCase("../../../good_input_files/groups.json")]
        public void DeserializeJson_AllGood(string fileName)
        {
            Assert.DoesNotThrow(() => GroupManager.InitGroups(fileName));
        }

        [Test]
        [TestCase("../../../good_input_files/groups.csv")]
        public void DeserializeJson_FileNotFound(string fileName)
        {
            Assert.Throws<Exception>(() => GroupManager.InitGroups(fileName));
        }

        [Test]
        [TestCase("../../../wrong_groups/missing_curly_bracket.json")]
        [TestCase("../../../wrong_groups/empty_name.json")]
        [TestCase("../../../wrong_groups/null_name.json")]
        [TestCase("../../../wrong_groups/null_customizable.json")]
        [TestCase("../../../wrong_groups/null_driverless.json")]
        [TestCase("../../../wrong_groups/null_attribute_name.json")]
        [TestCase("../../../wrong_groups/null_attribute_color.json")]
        [TestCase("../../../wrong_groups/empty.json")]
        public void DeserializeJson_MissingCurlyBrackets(string fileName)
        {
            Assert.Throws<Exception>(() => GroupManager.InitGroups(fileName));
        }
    }
}
