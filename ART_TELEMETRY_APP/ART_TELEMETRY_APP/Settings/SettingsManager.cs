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
        static List<GroupSettings_UC> group_settings_UCs = new List<GroupSettings_UC>();
        public static void AddGroupSettingsUC(GroupSettings_UC group_settings_UC)
        {
            group_settings_UCs.Add(group_settings_UC);
        }

        public static void UpdatePilotsTabs(Pilot active_pilot = null)
        {
            foreach (GroupSettings_UC group_settings_UC in group_settings_UCs)
            {
               // group_settings_UC.InitPilotsTabs(active_pilot);
            }
        }

        public static void ChangeGroupSettingUCName(string name, string new_name)
        {
            GetGroupSettingsUC(name).GroupName = new_name;
        }

        public static GroupSettings_UC GetGroupSettingsUC(string name)
        {
            return group_settings_UCs.Find(n => n.GroupName == name);
        }

        public static List<GroupSettings_UC> GroupSettingsUCs
        {
            get
            {
                return group_settings_UCs;
            }
        }
    }
}
