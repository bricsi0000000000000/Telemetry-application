using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void BuildWorkspace(string name)
        {
            
        }
    }
}
