using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Packer
{
    static class General
    {
        /// <summary>
        /// Checks if file has a Header
        /// </summary>
        /// <returns>true if file has header, false if file has no header</returns>
        public static bool HasHeader()
        {
            // Opening Streams
            FileStream fsRead = new FileStream(Values.source.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fsRead);

            // Write to variable bc Streams need to be closed before returning value
            byte[] header = br.ReadBytes(Values.header.Length);

            // Closing Streams
            fsRead.Flush();
            fsRead.Close();
            br.Close();

            // SequenceEqual compares two arrays and checks if the values are the same
            return Enumerable.SequenceEqual(header, Encoding.ASCII.GetBytes(Values.header));
        }


        /// <summary>
        /// Updates destination Values like destination-File-Path and so on
        /// </summary>
        public static void UpdateDestValues()
        {
            // Gets destination Name of TextBox of MainWindow
            // Current.Dispatcher.Invoke needs to be called cause UpdateDestValues is called from a new Thread
            // and GetDestName wants to read a value of the MainWindow wich is in a different Thread
            string destNameText = System.Windows.Application.Current.Dispatcher.Invoke(() => GetDestName()); 

            if (HasHeader()) 
                // Creates destination File Name 
                Values.destFileName = RemoveExtension(Values.ttpackExtension, destNameText) + Values.destFileName.Substring(Values.destFileName.LastIndexOf('.'));
            
            else
                // Extension entfernen
                Values.destFileName = RemoveExtension(Values.source.Extension, destNameText) + Values.ttpackExtension;

            Values.destFilePath = Values.destFileDirectory + "\\" + Values.destFileName;
        }

        /// <summary>
        /// Gets Text of a TextBox in MainWindow
        /// </summary>
        /// <returns>Returns Text of TextBox "destinationName" from the first Current MainWindow</returns>
        private static string GetDestName()
        {
            // Returns Text of TextBox "destinationName" from the first Current MainWindow
            return System.Windows.Application.Current.Windows.OfType<MainWindow>().First().destinationName.Text;
        }

        /// <summary>
        /// Removes extension from a string
        /// </summary>
        /// <param name="extension">extension to remove</param>
        /// <param name="toRemoveFrom">string to remove the extension from</param>
        /// <returns></returns>
        private static string RemoveExtension(string extension, string toRemoveFrom)
        {
            // If string has a '.'
            if (toRemoveFrom.LastIndexOf('.') > 0)
                // If Substring is the same as extension
                // Substring removes everything after the last '.'
                if (toRemoveFrom.Substring(toRemoveFrom.LastIndexOf('.')) == extension)
                    
                    // Returns everything in front of the last '.'
                    return toRemoveFrom.Substring(0, toRemoveFrom.LastIndexOf('.'));
            
            // Returns the inputted string
            return toRemoveFrom;
        }
    }
}
