using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Telemetry_presentation_layer.ValidationRules
{
    public class FieldsViewModel : INotifyPropertyChanged
    {
        private string? name;
        private string? formula;

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

        public event PropertyChangedEventHandler? PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged() => args => PropertyChanged?.Invoke(this, args);
    }
}
