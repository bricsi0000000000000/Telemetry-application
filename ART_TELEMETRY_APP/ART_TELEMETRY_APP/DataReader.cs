using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using LiveCharts;

namespace ART_TELEMETRY_APP
{
    class DataReader
    {
        List<DataTable> data_tables = new List<DataTable>();

        public DataReader() { }
        public DataReader(string file_name)
        {
            List<Tuple<string, ChartValues<float>>> new_datas = new List<Tuple<string, ChartValues<float>>>();

            StreamReader read_file = new StreamReader(file_name);

            string[] attributes = read_file.ReadLine().Split(';');

            foreach (string item in attributes)
            {
                new_datas.Add(new Tuple<string, ChartValues<float>>(item, new ChartValues<float>()));
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

            foreach (Tuple<string, ChartValues<float>> data in new_datas)
            {
                data_tables.Add(new DataTable(data.Item1, data.Item2));
            }
        }

        internal List<DataTable> DataTables { get => data_tables; set => data_tables = value; }
    }
}
