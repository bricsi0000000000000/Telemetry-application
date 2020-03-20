using LiveCharts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    static class DataManager
    {
        static List<InputFile> datas = new List<InputFile>();
        static string active_file_name;

        public static void AddInputData(InputFile input_data)
        {
            input_data.InitDistances();
            datas.Add(input_data);
        }

        public static InputFile GetData(string file_name = "")
        {
            if (file_name.Equals(""))
            {
                return datas.Find(name => name.FileName == active_file_name);
            }
            else
            {
                return datas.Find(name => name.FileName == file_name);
            }
        }

        public static string ActiveFileName
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

        public static int DatasCount
        {
            get
            {
                return datas.Count;
            }
        }
    }
}
