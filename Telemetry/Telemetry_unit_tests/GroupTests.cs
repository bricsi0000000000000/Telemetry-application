using DataLayer.Groups;
using NUnit.Framework;
using PresentationLayer.Groups;
using System;

namespace Telemetry_unit_tests
{
    [TestFixture]
    public class GroupTests
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
        [TestCase(0, "proba_group", 0, "proba group")]
        [TestCase(1, "proba group", 1, "proba_group")]
        [TestCase(2, " proba group", 2, " proba group")]
        [TestCase(3, "proba group ", 3, "proba group ")]
        [TestCase(4, " proba group ", 4, " proba group ")]
        public void CreateGroup_TestBad(int id, string groupName, int expectedID, string expectedGroupName)
        {
            Group group = new Group(id, groupName);
            Assert.AreNotEqual(group.Name, expectedGroupName);
        }

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

        [Test]
        public void InitGroup_TestGood()
        {
            GroupManager.InitGroups("../../../good_input_files/groups.json");

            Group group1 = new Group(0, "Gearbox");
            group1.AddAttribute("rev", "#FF4D4D4D", 1);
            group1.AddAttribute("gear", "#FF80FC05", 1);
            group1.AddAttribute("upInput", "#FF05FCF4", 1);
            group1.AddAttribute("upRqe", "#FF8005FC", 1);
            group1.AddAttribute("upGoing", "#FFFC0505", 1);
            group1.AddAttribute("shiftCnt", "#FFFC7C05", 1);
            group1.AddAttribute("neutral_In", "#FFFCE705", 1);

            Group group2 = GroupManager.GetGroup("Gearbox");
            Assert.AreEqual(group1.Name, group2.Name);
            for (int i = 0; i < group1.Attributes.Count; i++)
            {
                Assert.AreEqual(group1.Attributes[i].Name, group2.Attributes[i].Name);
                Assert.AreEqual(group1.Attributes[i].ColorText, group2.Attributes[i].ColorText);
            }
        }

        [Test]
        public void InitGroup_TestBad()
        {
            GroupManager.InitGroups("../../../good_input_files/groups.json");

            Group group1 = new Group(0, "Gaerbox");
            group1.AddAttribute("ver", "#FFF1B5B6", 1);
            group1.AddAttribute("gaer", "#FF5F1815", 1);
            group1.AddAttribute("downInput", "#FFFCE704", 1);
            group1.AddAttribute("downRqe", "#FF80FC03", 1);
            group1.AddAttribute("downGoing", "#FFBA8A83", 1);
            group1.AddAttribute("shtCnt", "#FF8005F2", 1);
            group1.AddAttribute("neutral_Out", "#FFFC0501", 1);

            var group2 = GroupManager.GetGroup("Gearbox");
            Assert.AreNotEqual(group1.Name, group2.Name);
            for (int i = 0; i < group1.Attributes.Count; i++)
            {
                Assert.AreNotEqual(group1.Attributes[i].Name, group2.Attributes[i].Name);
                Assert.AreNotEqual(group1.Attributes[i].ColorText, group2.Attributes[i].ColorText);
            }
        }

        [Test]
        [TestCase("Gearbox")]
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
            GroupManager.AddGroup(new Group(0, name));
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
            GroupManager.AddGroup(new Group(0, name));
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
