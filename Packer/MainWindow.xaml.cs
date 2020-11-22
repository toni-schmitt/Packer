﻿using System;
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
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();

            //fileDialog.Filter = "Bitmap Images (*.bmp)|*.bmp";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if(fileDialog.ShowDialog() == true)
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

            if (Encoder.HasHeader())
            {
                decode.Visibility = Visibility.Visible;
                encode.Visibility = Visibility.Hidden;
                previewImg.Source = null;
            }
            else
            {
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
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            Values.destFileDirectory = dialog.SelectedPath;
            chooseDestination.Text = Values.destFileDirectory;
        }

        /// <summary>
        /// Runs on Encode Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void encode_Click(object sender, RoutedEventArgs e)
        {
            // Sets Cursor to Wait to indicate for User that something is happening
            Window.Cursor = Cursors.Wait;

            Encoder.Encode();

            Window.Cursor = Cursors.Arrow;
            MessageBox.Show("Done");
        }
    }
}
