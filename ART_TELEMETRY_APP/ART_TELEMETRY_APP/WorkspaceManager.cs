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

        public Workspace GetWorkspace(string name)
        {
            return workspaces.Find(n => n.Name == name);
        }

        public void AddWorkspace(Workspace workspace)
        {
            workspaces.Add(workspace);
        }
    }
}
