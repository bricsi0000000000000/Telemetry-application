using LocigLayer.Texts;
using PresentationLayer.Extensions;
using System;
using System.IO;

namespace LogicLayer.Errors
{
    /// <summary>
    /// Show error messages.
    /// </summary>
    public static class ShowError
    {
        /// <summary>
        /// Shows error message in a <seealso cref="ErrorMessagePopUp"/> window.
        /// </summary>
        /// <param name="message">Error message.</param>
        public static void ShowErrorMessage(string message, string className)
        {
            WriteLog(message, className);
            var errorMessagePopUp = new ErrorMessagePopUp(message);
            errorMessagePopUp.ShowDialog();
        }

        private static void WriteLog(string message, string className)
        {
            using StreamWriter writer = new StreamWriter(TextManager.ErrorMessagesLogFileName.MakePath("logs"));

            writer.WriteLine($"[{DateTime.Now}]: {className}\t{message}");
        }
    }
}
