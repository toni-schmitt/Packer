﻿using System.Linq;
using System.Text;
using System.IO;

namespace Packer
{
    static class General
    {
        /// <summary>
        /// Checks if file has a Header
        /// </summary>
        /// <returns> true if file has header, false if file has no header </returns>
        public static bool HasHeader()
        {
            // Opening Streams
            FileStream fsRead = new FileStream(Values.source.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fsRead);

            // Write to variable bc Streams need to be closed before returning value
            byte[] header = br.ReadBytes(Values.magicNbr.Length);

            // Closing Streams
            fsRead.Flush();
            fsRead.Close();
            br.Close();

            // SequenceEqual compares two arrays and checks if the values are the same
            return Enumerable.SequenceEqual(header, Encoding.ASCII.GetBytes(Values.magicNbr));
        }


        /// <summary>
        /// Updates destination Values like destination-File-Path and so on
        /// </summary>
        /// <param name="hasHeader"> if file has header </param>
        public static void UpdateDestValues(bool hasHeader)
        {
            // Gets destination Name of TextBox of MainWindow
            // Current.Dispatcher.Invoke needs to be called cause UpdateDestValues is called from a new Thread
            // and GetDestName wants to read a value of the MainWindow wich is in a different Thread
            string destNameText = System.Windows.Application.Current.Dispatcher.Invoke(() => GetDestName());


            if (hasHeader && destNameText != Values.source.Name)
                // Creates destination File Name 
                if (Values.destinationFileName.LastIndexOf(Values.dot) > 0)
                    Values.destinationFileName = RemoveExtension(destNameText, Values.ttpackExtension) + Values.destinationFileName.Substring(Values.destinationFileName.LastIndexOf(Values.dot));
                else
                    Values.destinationFileName = RemoveExtension(destNameText, Values.ttpackExtension);

            else if (!hasHeader)
                // Extension entfernen
                Values.destinationFileName = RemoveExtension(destNameText, Values.source.Extension) + Values.ttpackExtension;


            Values.destinationPath = Values.destinationDirectory + "\\" + Values.destinationFileName;
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
        /// <param name="toRemoveFrom"> string to remove the extension from </param>
        /// <param name="extensionToRemove"> extension to remove </param>
        /// <returns> string without extension </returns>
        private static string RemoveExtension(string toRemoveFrom, string extensionToRemove)
        {
            // If string has a '.'
            if (toRemoveFrom.LastIndexOf(Values.dot) > 0)
                // If Substring is the same as extension
                // Substring removes everything after the last '.'
                if (toRemoveFrom.Substring(toRemoveFrom.LastIndexOf(Values.dot)) == extensionToRemove)

                    // Returns everything in front of the last '.'
                    return toRemoveFrom.Substring(0, toRemoveFrom.LastIndexOf(Values.dot));

            // Returns the inputted string
            return toRemoveFrom;
        }
    }
}
