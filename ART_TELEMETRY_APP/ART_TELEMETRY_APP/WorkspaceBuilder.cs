using Dragablz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    class WorkspaceBuilder
    {
        #region instance
        private static WorkspaceBuilder instance = null;
        private WorkspaceBuilder() { }

        public static WorkspaceBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WorkspaceBuilder();
                }
                return instance;
            }
        }
        #endregion

        public void BuildWorkspace(TabablzControl workspaces, string name = "")
        {
            TabItem item = new TabItem();
            item.Header = WorkspaceManager.Instance.GetWorkspace(name).Name;
            workspaces.Items.Add(item);

            /*TabManager.Instance.AddTab(new Tab(name));
            TabBuilder.Instance.BuildTab(name, workspaces);

            TabManager.Instance.AddTab(new Tab(string.Format("{0}_settings", name)));*/
        }
    }
}
