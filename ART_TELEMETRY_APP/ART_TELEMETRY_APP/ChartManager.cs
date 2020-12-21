using ART_TELEMETRY_APP.Datas.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Laps.Classes;
using ART_TELEMETRY_APP.Laps.UserControls;
using ART_TELEMETRY_APP.Settings.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP
{
    public static class ChartManager
    {
        public static List<string> CursorChannelNames = new List<string>();
        public static List<double> CursorChannelData = new List<double>();

        public static void UpdateCharts()
        {
            foreach (var lapsContent in ((Diagrams)MenuManager.GetTab(TextManager.DiagramsMenuName).Content).Tabs)
            {
                if (!(lapsContent.Content is LapsContent))
                    continue;

                var selectedChannels = ((LapsContent)lapsContent.Content).Group.Attributes;

                for (int i = 0; i < InputFileManager.InputFiles.Count; i++)
                {
                    if (!InputFileManager.InputFiles[i].IsSelected)
                        continue;

                    var selectedLaps = new List<Lap>();

                    foreach (var lap in InputFileManager.InputFiles[i].Laps)
                    {
                        if (ChartsSelectedData.SelectedLaps.Contains(lap.Index))
                        {
                            selectedLaps.Add(lap);
                        }
                    }

                    /*foreach (var channel in InputFileManager.InputFiles[i].Channels)
                    {
                        if (selectedChannels.Contains(channel.Name))
                        {
                            //var chart = ((LapsContent)lapsContent.Content).GetChart(((LapsContent)lapsContent.Content).Group.Name + channel.ChannelName);
                            ((LapsContent)lapsContent.Content).UpdateCursorData();
                            //  chart.RenderPlot(chart.MouseX);
                        }
                    }*/
                }
            }
        }
    }
}
