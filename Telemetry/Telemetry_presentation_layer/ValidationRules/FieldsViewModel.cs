using System;
using System.ComponentModel;

namespace Telemetry_presentation_layer.ValidationRules
{
    public class FieldsViewModel : INotifyPropertyChanged
    {
        private string? name;
        private string? formula;
        private string? driverlessHorizontalAxis;
        private string? driverlessC0refChannel;
        private string? driverlessYChannel;
        private int lineWidth;

        public string? Name
        {
            get => name;
            set => this.MutateVerbose(ref name, value, RaisePropertyChanged());
        }

        public string? Formula
        {
            get => formula;
            set => this.MutateVerbose(ref formula, value, RaisePropertyChanged());
        }

        public string? DriverlessHorizontalAxis
        {
            get => driverlessHorizontalAxis;
            set => this.MutateVerbose(ref driverlessHorizontalAxis, value, RaisePropertyChanged());
        }

        public string? DriverlessC0refChannel
        {
            get => driverlessC0refChannel;
            set => this.MutateVerbose(ref driverlessC0refChannel, value, RaisePropertyChanged());
        }

        public string? DriverlessYChannel
        {
            get => driverlessYChannel;
            set => this.MutateVerbose(ref driverlessYChannel, value, RaisePropertyChanged());
        }

        public int LineWidth
        {
            get => lineWidth;
            set => this.MutateVerbose(ref lineWidth, value, RaisePropertyChanged());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged() => args => PropertyChanged?.Invoke(this, args);
    }
}
