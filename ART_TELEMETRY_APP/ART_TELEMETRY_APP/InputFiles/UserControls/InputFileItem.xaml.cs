using ART_TELEMETRY_APP.Drivers.Classes;
using ART_TELEMETRY_APP.InputFiles.Classes;
using ART_TELEMETRY_APP.Settings.Classes;
using ART_TELEMETRY_APP.Settings.UserControls;
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

namespace ART_TELEMETRY_APP.InputFiles.UserControls
{
    /// <summary>
    /// Interaction logic for InputFileItem.xaml
    /// </summary>
    public partial class InputFileItem : UserControl
    {
        private readonly BrushConverter converter = new BrushConverter();
        private readonly InputFile inputFile;

        public InputFileItem(InputFile inputFile)
        {
            InitializeComponent();

            InputFileNameLbl.Content = inputFile.FileName;
            this.inputFile = inputFile;
            ChangeState();
        }

        private void ChangeState()
        {
            SelectInputFileIcon.Kind = inputFile.IsSelected ? MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarked : MaterialDesignThemes.Wpf.PackIconKind.CheckboxBlankOutline;
            SelectInputFileIcon.Foreground = inputFile.IsSelected ? (Brush)converter.ConvertFromString("#FFE21B1B") : Brushes.White;
        }

        private void SelectInputFileBtn_Click(object sender, RoutedEventArgs e)
        {
            inputFile.IsSelected = !inputFile.IsSelected;
            ChangeState();

            if (inputFile.IsSelected)
            {
                if (!DriverManager.GetDriver(inputFile.DriverName).IsSelected)
                {
                    DriverManager.GetDriver(inputFile.DriverName).IsSelected = true;
                    ((SelectDriversAndInputFiles)MenuManager.GetTab(TextManager.DiagramsSettingsMenuName).Content).InitDriversItems();

                    //TODO update diagrams
                }
            }

            ((SelectDriversAndInputFiles)MenuManager.GetTab(TextManager.DiagramsSettingsMenuName).Content).InitLapItems();
            ((SelectDriversAndInputFiles)MenuManager.GetTab(TextManager.DiagramsSettingsMenuName).Content).InitSelectedChannels();
        }
    }
}
