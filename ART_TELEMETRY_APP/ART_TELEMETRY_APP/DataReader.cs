using LiveCharts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    class DataReader
    {
        public DataReader(string file_name, ComboBox inputdatas_cmb_box)
        {
            readFile(file_name);
        }

        private void readFile(string file_name)
        {
            Trace.WriteLine("reading file");

            List<Tuple<string, ChartValues<float>>> new_datas = new List<Tuple<string, ChartValues<float>>>();

            StreamReader read_file = new StreamReader(file_name, Encoding.Default);

            string[] attributes = read_file.ReadLine().Split(';');
            foreach (string attribute in attributes)
            {
                new_datas.Add(new Tuple<string, ChartValues<float>>(attribute, new ChartValues<float>()));
            }

            while (!read_file.EndOfStream)
            {
                string[] row = read_file.ReadLine().Split(';');
                for (int i = 0; i < new_datas.Count; i++)
                {
                    if (row[i] != "")
                    {
                        new_datas[i].Item2.Add(float.Parse(row[i]));
                    }
                    else
                    {
                        new_datas[i].Item2.Add(float.NaN);
                    }
                }
            }

            read_file.Close();

            Trace.WriteLine("reading file has ended");

            Datas.Instance.AddInputData(new InputData(file_name.Split('\\').Last(), new_datas));
        }
    }
}
