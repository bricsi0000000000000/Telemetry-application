using DataLayer.Groups;
using NUnit.Framework;
using PresentationLayer.Groups;
using System;

namespace UnitTest
{
    [TestFixture]
    public partial class GroupTests
    {
        [Test]
        public void InitGroup_TestGood()
        {
            GroupManager.InitGroups(GOOD_GROUPS_PATH + "groups.json");

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
            GroupManager.InitGroups(GOOD_GROUPS_PATH + "groups.json");

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
    }
}
