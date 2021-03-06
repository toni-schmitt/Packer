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

        // Variables for changing the language
        private bool engLangSelected = false;
        private readonly ResourceDictionary resourceDictionary = new ResourceDictionary();  // Language-XAML-File will be saved in ResourceDictionry


        public MainWindow()
        {
            InitializeComponent();
            Window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            // Sets Language on initialization to german
            SwitchLanguage(null, null);
        }


        /// <summary>
        /// Sets Window back to Startup-Layout
        /// </summary>
        private void StartupWindow()
        {
            // Puts Hiegh of middle Row back to 2
            midRow.Height = new GridLength(2, GridUnitType.Star);
            // Puts ChooseFile Button to middle
            chooseFileVB.SetValue(Grid.RowProperty, 6);

            // Hides all other elements
            destinationDirectory.Visibility = Visibility.Hidden;
            destinationName.Visibility = Visibility.Hidden;
            decode.Visibility = Visibility.Hidden;
            encode.Visibility = Visibility.Hidden;
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
            {// If OK in FileDialog was clicked


                Values.source = new System.IO.FileInfo(fileDialog.FileName);
                Values.destinationDirectory = Values.source.DirectoryName;

                

                // Destination Directory is Source Directory by default
                destinationName.Text = Values.source.Name;
                destinationDirectory.Text = Values.destinationDirectory;

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

            Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog openFileDialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Values.source.DirectoryName,
                Multiselect = false
            };


            if (openFileDialog.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                Values.destinationDirectory = openFileDialog.FileName;
                destinationDirectory.Text = Values.destinationDirectory;
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


            
            // En- or Decodes depeding on the Button Name
            switch ((sender as Button).Name)    // Sets the sender object to a Button object so that the Name of the Button is readable
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

            // Initializes new MessageBox Object
            MessageBox mb = new MessageBox(resourceDictionary);

            // Shows MessageBox
            mb.Show();

            // Awaits 1 Second
            await Task.Run(() => System.Threading.Thread.Sleep(1000));

            // Sets MainWindow back to StartupLayout
            StartupWindow();

        }

        /// <summary>
        /// Starts Unit Test (Obsolete)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartUnitTest_Click(object sender, RoutedEventArgs e)
        {
            Window.Visibility = Visibility.Hidden;
            UnitTestWindow ut = new UnitTestWindow();
            while ((bool)ut.ShowDialog());
            Window.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// Switches language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchLanguage(object sender, RoutedEventArgs e)
        {
            if (engLangSelected)
            {
                resourceDictionary.Source = new Uri("..\\Resources\\LangRes_en-EN.xaml", UriKind.Relative);
                engLangSelected = false;
            }
            else
            {
                resourceDictionary.Source = new Uri("..\\Resources\\LangRes_de-DE.xaml", UriKind.Relative);
                engLangSelected = true;
            }
            this.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
