﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private bool chngLang = false;

        public MainWindow()
        {
            InitializeComponent();
            Window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SwitchLanguage(null, null);
        }


        /// <summary>
        /// Opens a file Dialog to choose a bmp-image to encode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            // Createa a FileDialog 
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                // Set the Filter of the FileDialog
                Filter = "All Files (*.*) | *.*; | TTPACK Files (*.ttpack) | *.ttpack; | Bitmap Images (*.bmp) | *.bmp; | Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            // Show FileDialog
            if (fileDialog.ShowDialog() == true)
            {

                Values.source = new System.IO.FileInfo(fileDialog.FileName);

                Values.destFileDirectory = Values.source.DirectoryName;


                // Destination Directory is Source Directory by default
                destinationName.Text = Values.source.Name;
                destinationDirectory.Text = Values.destFileDirectory;

                // Puts the Button to top position
                midRow.Height = new GridLength(0.25, GridUnitType.Star);
                chooseFileVB.SetValue(Grid.RowProperty, 3);
                chooseFile.Content = new DynamicResourceExtension("chooseFileNew").ProvideValue(null);

                // Enables elements 
                destinationDirectory.Visibility = Visibility.Visible;
                destinationName.Visibility = Visibility.Visible;
                
                // Shows appropriate Options
                // for example if File can be Encoded only show Encode Option
                if (General.HasHeader())
                {
                    // Show Decode Button, Hide Encode Button, Hide Image Preview
                    decode.Visibility = Visibility.Visible;
                    encode.Visibility = Visibility.Hidden;
                }
                else
                {
                    // Hide Decode Button, Show Encode Button, Show Image Preview
                    decode.Visibility = Visibility.Hidden;
                    encode.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Opens a dialog to choose a destination folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DestFolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Opens Dialog to choose a Folder
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {// If File is selected (User clicked on OK)
                // Updates destination Values
                Values.destFileDirectory = dialog.SelectedPath;
                destinationDirectory.Text = Values.destFileDirectory;
            }
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
            destinationDirectory.IsEnabled = false;
            destinationName.IsEnabled = false;
            encode.IsEnabled = false;
            decode.IsEnabled = false;

            General.UpdateDestValues();

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
            destinationDirectory.IsEnabled = true;
            destinationName.IsEnabled = true;
            encode.IsEnabled = true;
            decode.IsEnabled = true;

            // Sets Cursor back to default
            Window.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Starts Unit Test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartUnitTest_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => UnitTest.StartTest());
        }

        private void SwitchLanguage(object sender, RoutedEventArgs e)
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            if (chngLang)
            {
                resourceDictionary.Source = new Uri("..\\Resources\\LangRes_en-EN.xaml", UriKind.Relative);
                chngLang = false;
            } 
            else
            {
                resourceDictionary.Source = new Uri("..\\Resources\\LangRes_de-DE.xaml", UriKind.Relative);
                chngLang = true;
            }
            this.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
