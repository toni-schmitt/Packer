using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Packer
{
    static class UnitTest
    {
        private static readonly string exsDir = "C:\\Users\\tonis\\Desktop\\Packer-Test\\extensive"; // Direcotry Root
        private static readonly string orgDir = exsDir + "\\org"; // Directory with original files
        private static readonly string refDir = exsDir + "\\ref"; // Directory with reference files for later tests
        private static readonly string entDir = exsDir + "\\ent"; // Directory with unzipped files
        private static readonly string verDir = exsDir + "\\ver"; // Directory with zipped files

        // List for directorys where the UnitTest happens
        static readonly List<string> dirs = new List<string> {
            orgDir, verDir, entDir, refDir
        };

        /// <summary>
        /// Starts the UnitTest
        /// </summary>
        public static void StartTest()
        {

            // Stopwatch to measure time the Test took
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // Deletes old Files of verDir and entDir
            DelFiles();

            // Dictionarys for saving test-results
            Dictionary<string, string> orgFileInfo = new Dictionary<string, string>();
            Dictionary<string, string> verFileInfo = new Dictionary<string, string>();
            Dictionary<string, string> entFileInfo = new Dictionary<string, string>();
            Dictionary<string, bool> areSame = new Dictionary<string, bool>();

            for (int directory = 0; directory < dirs.Count; directory++)
            {
                // All Files in current directory
                FileInfo[] dirFiles = new DirectoryInfo(dirs[directory]).GetFiles();

                // For every file in dirFile
                for (int index = 0; index < dirFiles.Length; index++)
                {


                    switch (directory)
                    {

                        case 0: // Encode
                            orgFileInfo.Add(dirFiles[index].Name, dirFiles[index].Length.ToString());

                            UpdateValues(dirFiles[index], directory);
                            Encoder.Encode();

                            verFileInfo.Add(new DirectoryInfo(dirs[directory + 1]).GetFiles()[index].Name, new DirectoryInfo(dirs[directory + 1]).GetFiles()[index].Length.ToString());
                            break;


                        case 1: // Decode
                            UpdateValues(dirFiles[index], directory);
                            Decoder.Decode();

                            entFileInfo.Add(new DirectoryInfo(dirs[directory + 1]).GetFiles()[index].Name, new DirectoryInfo(dirs[directory + 1]).GetFiles()[index].Length.ToString());
                            break;


                        case 2: // Test
                            areSame.Add(dirFiles[index].Name, IsSameAs(dirFiles[index]));
                            break;

                    }

                }


            }

            stopwatch.Stop();

            string results = "Time needed for complete Test of " + areSame.Count + " Files: " + stopwatch.Elapsed + "\r\n";

            for (int i = 0; i < areSame.Count; i++)
            {
                // Writes results from Dictionarys to string, so that it can be saved
                results += "\r\nOriginal File Name & Size: \r\n";
                results += orgFileInfo.ElementAt(i).Key.ToString() + " " + orgFileInfo.ElementAt(i).Value.ToString();
                results += "\r\nZipped File Name & Size: \r\n";
                results += verFileInfo.ElementAt(i).Key.ToString() + " " + verFileInfo.ElementAt(i).Value.ToString();
                results += "\r\nUnzipped File Name & Size: \r\n";
                results += entFileInfo.ElementAt(i).Key.ToString() + " " + entFileInfo.ElementAt(i).Value.ToString();
                results += "\r\nIs Original File same as Reference file: \r\n";
                results += areSame.ElementAt(i).Key.ToString() + " " + areSame.ElementAt(i).Value.ToString();
                results += "\r\n\r\n";

            }

            // Saves string to text file
            StreamWriter sw = new StreamWriter(
                    exsDir + "\\results\\test_result__" +
                    System.DateTime.Now.ToString(new System.Globalization.CultureInfo("de-DE"))
                        .Replace(':', '-')
                        .Replace('.', '-')
                        .Replace(' ', '_') +
                    ".txt");
            sw.Write(results);
            sw.Close();

            // Shows results in a Message Box
            System.Windows.MessageBox.Show(
               results,
               "Test Results: ",
               System.Windows.MessageBoxButton.OK,
               System.Windows.MessageBoxImage.Information,
               System.Windows.MessageBoxResult.OK,
               System.Windows.MessageBoxOptions.ServiceNotification);

        }

        /// <summary>
        /// Delets old filed
        /// </summary>
        private static void DelFiles()
        {
            for (int i = 1; i <= 2; i++)
                foreach (FileInfo file in new DirectoryInfo(dirs[i]).GetFiles())
                    file.Delete();
        }

        /// <summary>
        /// Checks if decoded files and original files are the same
        /// </summary>
        /// <param name="checkFile"></param>
        /// <returns></returns>
        private static bool IsSameAs(FileInfo checkFile)
        {
            FileStream fsReadCheck = new FileStream(checkFile.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader brCheck = new BinaryReader(fsReadCheck);

            UpdateValues(checkFile, 2);

            FileStream fsReadRef = new FileStream(Values.destinationPath, FileMode.Open, FileAccess.Read);
            BinaryReader brRef = new BinaryReader(fsReadRef);

            while (fsReadCheck.Position < fsReadCheck.Length && fsReadRef.Position < fsReadRef.Length)
                if (brCheck.ReadByte() != brRef.ReadByte())
                    return false;
            return true;
        }


        /// <summary>
        /// Updates Values
        /// </summary>
        /// <param name="info"></param>
        /// <param name="directory"></param>
        private static void UpdateValues(FileInfo info, int directory)
        {
            Values.source = info;
            Values.destinationFileName = info.Name;
            Values.destinationDirectory = dirs[++directory] + "\\";
            Values.destinationPath = Values.destinationDirectory + Values.destinationFileName;
        }
    }
}
