using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class WorkspaceManager
    {
        #region instance
        private static WorkspaceManager instance = null;
        private WorkspaceManager() { }

        public static WorkspaceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WorkspaceManager();
                }
                return instance;
            }
        }
        #endregion

        List<Workspace> workspaces = new List<Workspace>();
        string active_workspace;

        public string ActiveWorkspace
        {
            get
            {
                return active_workspace;
            }
            set
            {
                active_workspace = value;
            }
        }

        public Workspace GetWorkspace(string name = "")
        {
            if (name.Equals(""))
            {
                return workspaces.Find(n => n.Name == active_workspace);
            }
            return workspaces.Find(n => n.Name == name);
        }

        public void AddWorkspace(Workspace workspace)
        {
            workspaces.Add(workspace);
            active_workspace = workspace.Name;
        }
    }
}
