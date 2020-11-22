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
using System.Windows.Navigation;
using System.Windows.Shapes;

// Dont use these Using directives or elese some Options like Cursor are not going to work
// Some Classes are in both directives
// using System.Windows.Forms;
// using Microsoft.Win32

namespace Packer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }

        /// <summary>
        /// Opens a file Dialog to choose a bmp-image to encode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chooseFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();

            fileDialog.Filter = "TTPACK Files (*.ttpack) | *.ttpack; | Bitmap Images (*.bmp) | *.bmp; | Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (fileDialog.ShowDialog() == true)
            {
                Values.sourceFileName = fileDialog.SafeFileName;
                Values.sourceFilePath = fileDialog.FileName;
                Values.sourceFileDirectory = System.IO.Path.GetDirectoryName(Values.sourceFilePath);

                Values.destFileName = Values.sourceFileName + ".ttpack";
                Values.destFileDirectory = Values.sourceFileDirectory;
                Values.destFilePath = Values.destFileDirectory + "\\" + Values.destFileName;

                //Values.header = "ttpack" + Values.marker + Values.sourceFileName;

                // A new Bitmap Image with a new Uniform Resource Identifier is requiered for Image.Source
                //previewImg.Source = new BitmapImage(new Uri(Values.sourceFilePath, UriKind.Absolute));

                // Destination Directory is Source Directory by default
                chooseDestination.Text = Values.destFileDirectory;
                dest.Visibility = Visibility.Visible;
            }

            // Shows appropriate Options
            // for example if File can be Encoded only show Encode Option
            if (Encoder.HasHeader())
            {
                // Show Decode Button, Hide Encode Button, Hide Image Preview
                decode.Visibility = Visibility.Visible;
                encode.Visibility = Visibility.Hidden;
                previewImg.Source = null;
            }
            else
            {
                // Hide Decode Button, Show Encode Button, Show Image Preview
                decode.Visibility = Visibility.Hidden;
                encode.Visibility = Visibility.Visible;
                previewImg.Source = new BitmapImage(new Uri(Values.sourceFilePath, UriKind.Absolute));
            }
        }

        /// <summary>
        /// Opens a dialog to choose a destination folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void destFile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Opens Dialog to choose a Folder
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            Values.destFileDirectory = dialog.SelectedPath;
            chooseDestination.Text = Values.destFileDirectory;
        }

        /// <summary>
        /// Indicates for the User that something is happending by starting/stopping a animation and disabeling/enableing the button
        /// Encodes or Decodes depending on which button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeEnCode_Click(object sender, RoutedEventArgs e)
        {
            // Sets Cursor to Wait to indicate that something is happening for User
            Window.Cursor = Cursors.Wait;

            // Starts progress Animation of Button and Circle
            MaterialDesignThemes.Wpf.ButtonProgressAssist.SetIsIndeterminate(encode, true);
            MaterialDesignThemes.Wpf.ButtonProgressAssist.SetIsIndicatorVisible(encode, true);
            progressBar.Visibility = Visibility.Visible;
            // Disables all elements so that no new operation can be done
            chooseFile.IsEnabled = false;
            chooseDestination.IsEnabled = false;
            encode.IsEnabled = false;
            decode.IsEnabled = false;
            previewImg.Opacity = .2;

            // Sets the sender object to a Button object so that the Name of the Button is readable
            var btn = sender as Button;
            // En- or Decodes depeding on the Button Name
            switch (btn.Name)
            {
                // Awaits Method asynchronously so that the window does not freeze
                case "encode":
                    await Task.Run(() => Encoder.Encode());
                    break;
                case "decode":
                    await Task.Run(() => Decoder.Decode());
                    break;
            }

            // Stops progress Animation of Button and Cricle
            progressBar.Visibility = Visibility.Hidden;
            MaterialDesignThemes.Wpf.ButtonProgressAssist.SetIsIndeterminate(encode, false);
            MaterialDesignThemes.Wpf.ButtonProgressAssist.SetIsIndicatorVisible(encode, false);
            // Enables all elements again
            chooseFile.IsEnabled = true;
            chooseDestination.IsEnabled = true;
            encode.IsEnabled = true;
            decode.IsEnabled = true;
            previewImg.Opacity = 1;

            // Sets Cursor back to default
            Window.Cursor = Cursors.Arrow;
        }
    }
}
