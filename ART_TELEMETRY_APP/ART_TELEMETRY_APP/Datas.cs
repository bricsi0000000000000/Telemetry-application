using LiveCharts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class Datas
    {
        List<Data> datas = new List<Data>();
        string active_file_name;

        #region instance
        private static Datas instance = null;
        private Datas() { }

        public static Datas Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Datas();
                }
                return instance;
            }
        }
        #endregion

        public void AddInputData(Data input_data)
        {
            datas.Add(input_data);
        }

        public Data GetData(string file_name = "")
        {
            if (file_name == "")
            {
                return datas.Find(name => name.FileName == active_file_name);
            }
            else
            {
                return datas.Find(name => name.FileName == file_name);
            }
        }

        public string ActiveFileName
        {
            get
            {
                return active_file_name;
            }
            set
            {
                active_file_name = value;
            }
        }

        public int DatasCount
        {
            get
            {
                return datas.Count;
            }
        }
    }
}
