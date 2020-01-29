using LiveCharts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    class InputData
    {
        string input_data_name;
        List<Tuple<string, ChartValues<float>>> datas = new List<Tuple<string, ChartValues<float>>>();

        public InputData(string input_data_name, List<Tuple<string, ChartValues<float>>> datas)
        {
            this.input_data_name = input_data_name;
            this.datas = datas;
        }

        public string InputDataName
        {
            get
            {
                return this.input_data_name;
            }
        }

        public List<Tuple<string, ChartValues<float>>> Datas
        {
            get
            {
                return this.datas;
            }
        }
    }
}
