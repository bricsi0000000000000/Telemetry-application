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
        List<InputData> input_datas = new List<InputData>();

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

        public void AddInputData(InputData input_data)
        {
            input_datas.Add(input_data);
        }

        public InputData GetInputData(string attribute)
        {
            return input_datas.Find(attr => attr.InputDataName == attribute);
        }
    }
}
