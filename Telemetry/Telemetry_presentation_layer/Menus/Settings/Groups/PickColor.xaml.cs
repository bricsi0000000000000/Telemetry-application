using System.Windows;

namespace Telemetry_presentation_layer.Menus.Settings.Groups
{
    /// <summary>
    /// Interaction logic for PickColor.xaml
    /// </summary>
    public partial class PickColor : Window
    {
        public PickColor()
        {
            InitializeComponent();
        }

        private void ChooseButtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
