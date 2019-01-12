

using System.Windows;

namespace Microsoft.ProjectOxford.Face
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="App"/> class from being created
        /// </summary>
        private App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Show unhandled exception in message box
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is FaceAPIException)
            {
                var ex = e.Exception as FaceAPIException;
                MessageBox.Show(ex.ErrorMessage, "Face API Calling Error", MessageBoxButton.OK);
            }
            else
            {
                if (e.Exception.InnerException != null)
                {
                    MessageBox.Show(e.Exception.InnerException.ToString(), "Error", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButton.OK);
                }
            }

            e.Handled = true;
        }

        #endregion Methods
    }
}