using System;
using System.Collections.Generic;
using System.Text;

namespace Telemetry_data_and_logic_layer.Exceptions
{
    public class ErrorException : Exception
    {
        public ErrorException(string message) : base(message) { }
    }
}
