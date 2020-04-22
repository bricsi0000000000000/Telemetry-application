using ART_TELEMETRY_APP.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ART_TELEMETRY_APP
{
    /// <summary>
    /// Interaction logic for DiagramsGroupTabs_UI.xaml
    /// </summary>
    public partial class DiagramsGroupTabs_UI : UserControl
    {
        public DiagramsGroupTabs_UI()
        {
            InitializeComponent();

            InitGroupTabs();
        }

        public void InitGroupTabs()
        {
           /* foreach (Group group in GroupManager.Groups)
            {
                TabItem item = new TabItem();
                item.Header = group.Name;
                Diagrams_UI diagrams_IU = new Diagrams_UI(group);
                diagrams_IU.Name = group.Name;
                SettingsManager.AddDiagramsUc(diagrams_IU);
                item.Content = diagrams_IU;
                groups_tabcontrol.Items.Add(item);
                item.IsSelected = true;

                item = new TabItem();
                item.Header = "G-G diagram";
              //  GGDiagram_UC gg_diagram_UC = new GGDiagram_UC(group);
                gg_diagram_UC.Name = string.Format("gg_{0}", group.Name);
                SettingsManager.GGdiagram_UC = gg_diagram_UC;
                item.Content = gg_diagram_UC;
                groups_tabcontrol.Items.Add(item);

                item = new TabItem();
                item.Header = "Lap report";
                groups_tabcontrol.Items.Add(item);
                //item.IsSelected = true;
            }*/
        }
    }
}
