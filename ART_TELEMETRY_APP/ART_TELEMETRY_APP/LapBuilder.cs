using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ART_TELEMETRY_APP
{
    class LapBuilder
    {
        #region instance
        private static LapBuilder instance = null;
        private LapBuilder() { }

        public static LapBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LapBuilder();
                }
                return instance;
            }
        }
        #endregion

        BackgroundWorker worker;
        ColorZone diagram_calculate_laps;
        Grid charts_grid;
        ColorZone diagram_nothing;
        Label act_lap_lbl;

        public void Build(ColorZone diagram_nothing, ColorZone diagram_calculate_laps, Grid charts_grid, Label act_lap_lbl)
        {
            this.diagram_calculate_laps = diagram_calculate_laps;
            this.charts_grid = charts_grid;
            this.diagram_nothing = diagram_nothing;
            this.act_lap_lbl = act_lap_lbl;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += calculateLaps;
            worker.ProgressChanged += workerProgressChanged;
            worker.RunWorkerCompleted += workerCompleted;
            worker.RunWorkerAsync(10000);

            diagram_nothing.Visibility = Visibility.Hidden;
            diagram_calculate_laps.Visibility = Visibility.Visible;
           
        }

        private void calculateLaps(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < Groups.Instance.GroupsCount; i++)
            {
                foreach (string attribute in Groups.Instance.GetGroups[i].Attributes)
                {
                    Datas.Instance.GetData().MakeDatasInLaps(attribute);
                }
            }
        }

        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void workerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            diagram_calculate_laps.Visibility = Visibility.Hidden;

            ChartBuilder.Instance.Build(charts_grid, diagram_nothing);
            act_lap_lbl.Content = string.Format("{0}/{1}", 1, Datas.Instance.GetData().Laps.Count);
        }
    }
}
