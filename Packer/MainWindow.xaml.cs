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
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            // Only shows bmp images
            openFileDialog.Filter = "Bitmap Images (*.bmp)|*.bmp";
            // Opens folder MyPictures
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string imgSrc = "";
            if (openFileDialog.ShowDialog() == true)
            {
                // Sets chosen file name to encoder filename
                imgSrc = openFileDialog.FileName;
                // Sets chosen file name to filename for encoder
                Encoder.GetFile = imgSrc;
                // Make a Uniform Resource Identifier from the imgSrc-string
                Uri uri = new Uri(imgSrc, UriKind.Absolute);
                // Makes a BitmapImage (ImageSource) of the URI
                previewImg.Source = new BitmapImage(uri);
                if (imgSrc != "")
                    dest.Visibility = Visibility.Visible;

                // TO DO: AUTOMATICALLY CHECK IF SELECTED FILE HAS HEADER OR NOT!!!!!
                // IF FILE HAS HEADER ONLY SHOW DECODE OPTION!!!!!!
                // IF FILE HAS NO HEADER ONLY SHOW ENCODE OPTION!!!!!!
            }
        }

        private void destFile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                chooseDestination.Text = dialog.SelectedPath;
                
            }
        }
    }
}
