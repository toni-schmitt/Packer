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
using System.Windows.Shapes;

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
            windowUnitTest.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void ClickDestDir(object sender, MouseButtonEventArgs e)
        {
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

        private async void ClickStartTest(object sender, RoutedEventArgs e)
        {
            destinationDirectory.IsEnabled = false;
            startTest.IsEnabled = false;
            analyze.IsEnabled = false;

            await Task.Run(() => UnitTest.StartTest());

            destinationDirectory.IsEnabled = true;
            startTest.IsEnabled = true;
            analyze.IsEnabled = true;
        }
    }
}
