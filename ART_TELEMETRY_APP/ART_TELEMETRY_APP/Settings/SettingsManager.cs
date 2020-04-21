using ART_TELEMETRY_APP.Pilots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.Settings
{
    static class SettingsManager
    {
        public static List<string> SelectedChannels = new List<string>();
        static List<GroupSettings_UC> groups_tabs_contents = new List<GroupSettings_UC>();
        public static DiagramsSettings_UC CustomTabSettings_UC;
        public static MapSettings_UC MapSettings_UC;
        static List<Diagrams_UI> diagrams_UCs = new List<Diagrams_UI>();
        public static GGDiagram_UC GGdiagram_UC = new GGDiagram_UC();

        public static void AddGroupSettingsUC(GroupSettings_UC group_settings_UC)
        {
            groups_tabs_contents.Add(group_settings_UC);
        }

        public static void UpdatePilotsInGroups()
        {
            foreach (GroupSettings_UC group_settings_UC in groups_tabs_contents)
            {
                group_settings_UC.InitPilots();
            }
        }

        public static void UpdateSelectedChannelsInGroups()
        {
            foreach (GroupSettings_UC group_settings_UC in groups_tabs_contents)
            {
                group_settings_UC.InitSelectedChannelsList();
            }
        }

        public static void ChangeGroupSettingUCName(string name, string new_name)
        {
            GetGroupSettingsUC(name).GroupName = new_name;
        }

        public static GroupSettings_UC GetGroupSettingsUC(string name)
        {
            return groups_tabs_contents.Find(n => n.GroupName == name);
        }

        public static List<GroupSettings_UC> GroupSettingsUCs
        {
            get
            {
                return groups_tabs_contents;
            }
        }

        public static bool SettingsIsOpen = false;

        public static void AddDiagramsUc(Diagrams_UI diagrams_UI)
        {
            if (GetDiagramsUc(diagrams_UI.Name) == null)
            {
                diagrams_UCs.Add(diagrams_UI);
            }
        }

        public static Diagrams_UI GetDiagramsUc(string name)
        {
            return diagrams_UCs.Find(n => n.Name == name);
        }
    }
}
