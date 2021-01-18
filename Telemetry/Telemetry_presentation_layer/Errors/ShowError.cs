using MaterialDesignThemes.Wpf;
using System;

namespace Telemetry_presentation_layer.Errors
{
    /// <summary>
    /// Show error messages.
    /// </summary>
    public static class ShowError
    {
        /// <summary>
        /// Show error message in <paramref name="snackbar"/>.
        /// </summary>
        /// <param name="snackbar"><seealso cref="Snackbar"/>, where the error message will be shown.</param>
        /// <param name="message">Error message.</param>
        /// <param name="time">How many seconds will the snackbar shown.</param>
        public static void ShowErrorMessage(ref Snackbar snackbar, string message, double time = 3)
        {
            snackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }
    }
}
