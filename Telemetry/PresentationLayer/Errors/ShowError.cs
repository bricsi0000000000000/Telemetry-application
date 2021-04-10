using MaterialDesignThemes.Wpf;
using System;

namespace PresentationLayer.Errors
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
        public static void ShowErrorMessage(string message)
        {
            var errorMessagePopUp = new ErrorMessagePopUp(message);
            errorMessagePopUp.ShowDialog();
        }
    }
}
