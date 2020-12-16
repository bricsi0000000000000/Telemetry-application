using MaterialDesignThemes.Wpf;
using System;

namespace ART_TELEMETRY_APP.Errors.Classes
{
    public static class ShowError
    {
        public static void ShowErrorMessage(ref Snackbar snackbar, string message, double time = 3)
        {
            snackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }

        public static void ShowErrorMessage(Snackbar snackbar, string message, double time = 3)
        {
            snackbar.MessageQueue.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(time));
        }
    }
}
