using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Packer
{
    /// <summary>
    /// Interaktionslogik für UnitTestWindow.xaml
    /// </summary>
    public partial class UnitTestWindow : Window
    {
        public UnitTestWindow()
        {
            InitializeComponent();
            // Center Window
            windowUnitTest.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }


        /// <summary>
        /// Sets the destination directory where the files of the Unit Test are
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickDestDir(object sender, MouseButtonEventArgs e)
        {
            // Initializes an CommonOpenFileDialog for choosing a folder
            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog openFileDialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                UnitTest.SetDirectory = openFileDialog.FileName;
                destinationDirectory.Text = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Starts UnitTest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ClickStartTest(object sender, RoutedEventArgs e)
        {
            // Disables buttons so that user knows Unit Test is in progress
            destinationDirectory.IsEnabled = false;
            startTest.IsEnabled = false;
            analyze.IsEnabled = false;

            // Starts Unit Test as background Thread
            await Task.Run(() => UnitTest.StartTest());

            // Enables buttons so that user knows UnitTest is done
            destinationDirectory.IsEnabled = true;
            startTest.IsEnabled = true;
            analyze.IsEnabled = true;
        }
    }
}
