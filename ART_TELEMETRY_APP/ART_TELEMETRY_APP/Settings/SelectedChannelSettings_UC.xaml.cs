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

namespace ART_TELEMETRY_APP.Settings
{
    /// <summary>
    /// Interaction logic for SelectedChannelSettings_UC.xaml
    /// </summary>
    public partial class SelectedChannelSettings_UC : UserControl
    {
        public SelectedChannelSettings_UC(string attribute)
        {
            InitializeComponent();

            this.attribute = attribute;

            Random r = new Random();
            strokeThicknessTxtbox.Text = r.Next(0, 10).ToString();
        }

        private void strokeThicknessTxtbox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void strokeColorColorpickerPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        string attribute;

        public string Attribute
        {
            get
            {
                return attribute;
            }
        }
    }
}
