using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Packer
{
    public static class UnitTest
    {
        // Directory where files are
        private static string _directory;
        public static string SetDirectory { set { _directory = value; } }

        // Colon
        private static readonly char _colon = ':';



        /// <summary>
        /// Starts the Unit Test
        /// </summary>
        public static void StartTest()
        {

            

            // Changes file name to random names
            RadnomFileNames();

            // The results of the test
            List<StringBuilder> results = new List<StringBuilder>();

            // First Encoding second Decoding
            for (int chooseOperation = 0; chooseOperation < 2; chooseOperation++)
            {
                // Files to en/decode
                FileInfo[] files = null;
                switch (chooseOperation)
                {
                    case 0: // Get File without ttpack extenstion
                        files = new DirectoryInfo(_directory).GetFiles();
                        break;
                    case 1: // Get Files with ttpack extesntion
                        files = new DirectoryInfo(_directory).GetFiles("*.ttpack");
                        break;
                }

                // For each file in UnitTest-directory
                foreach (FileInfo file in files)
                {
                    // Result of current test
                    StringBuilder result = new StringBuilder();

                    // Upadtes Values
                    Values.source = file;
                    Values.destinationDirectory = Values.source.DirectoryName;
                    System.Windows.Application.Current.Dispatcher.Invoke(() => System.Windows.Application.Current.Windows.OfType<MainWindow>().First().destinationName.Text = Values.source.Name);

                    // To measure time operation took
                    var timer = new System.Diagnostics.Stopwatch();
                    timer.Start();

                    switch (chooseOperation)
                    {
                        case 0:
                            Encoder.Encode();
                            result.Append("encoding").Append(_colon);
                            break;
                        case 1:
                            Decoder.Decode();
                            result.Append("decoding").Append(_colon);
                            break;
                    }

                    timer.Stop();

                    
                    long orgLength = file.Length;                                   // Length of original file
                    long newLength = new FileInfo(Values.destinationPath).Length;   // Length of new file (ttpack or decoded)
                    long differenceLenghts = orgLength - newLength;                 // Difference of lengths

                    // Append information of test  to result StringBuilder
                    result.Append(file.Name).Append(_colon)
                        .Append(file.Extension).Append(_colon)
                        .Append(string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}:{4:D3}", timer.Elapsed.Days, timer.Elapsed.Hours, timer.Elapsed.Minutes, timer.Elapsed.Seconds, timer.Elapsed.Milliseconds)).Append(_colon)
                        .Append(orgLength).Append(_colon)
                        .Append(newLength).Append(_colon)
                        .Append(differenceLenghts);

                    // Add result to results list
                    results.Add(result);
                }

                // Message Box
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    MessageBox mb = new MessageBox();
                    mb.text.Text = "Done with test" + chooseOperation;
                    mb.Show();
                });

            }

            // Message Box
            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                MessageBox mb = new MessageBox();
                mb.text.Text = "Writing result files now";
                mb.Show();
            });


            WriteResults(ref results);
            

        }


        /// <summary>
        /// Changes file-names to random name
        /// </summary>
        /// <param name="info"> FileInfo-Array with the files </param>
        private static void RadnomFileNames()
        {
            // Gets every file of directory as FielInfo
            FileInfo[] info = new DirectoryInfo(_directory).GetFiles();
            // A Random object for changing of file names
            Random rnd = new Random();
            foreach (FileInfo file in info)
            {
                int rndNbrsAmount = rnd.Next(4, 20);
                string rndNbr = "";
                for (int i = 0; i < rndNbrsAmount; i++)
                    rndNbr += rnd.Next(0, 10);


                string newFileName = "ut" + file.Name.ToArray()[0] + file.Name.ToArray()[1] + file.Name.ToArray()[2] + rndNbr + file.Extension;

                File.Move(file.FullName, Path.Combine(file.DirectoryName, newFileName));
            }
        }

        private static List<StringBuilder> CompareFiles()
        {
            // Compare Files
            List<StringBuilder> sameResultsList = new List<StringBuilder>();

            var orgFiles = new DirectoryInfo(_directory).EnumerateFiles().Where(f => !f.Extension.Equals(".ttpack") && Path.GetFileNameWithoutExtension(f.Name).Length > 8);
            var decodedFiles = new DirectoryInfo(_directory).EnumerateFiles().Where(f => Path.GetFileNameWithoutExtension(f.Name).Length == 8);


            if (orgFiles.Count() == decodedFiles.Count())
                for (int i = 0; i < orgFiles.Count(); i++)
                {
                    StringBuilder isSameResult = new StringBuilder();
                    isSameResult.Append(CompareFiles(orgFiles.ElementAt(i), decodedFiles.ElementAt(i))).Append(_colon)
                        .Append(orgFiles.ElementAt(i).Length).Append(_colon)
                        .Append(decodedFiles.ElementAt(i).Length);
                    sameResultsList.Add(isSameResult);
                }

            return sameResultsList;
        }

        private static bool CompareFiles(FileInfo file1, FileInfo file2)
        {
            FileStream fs1 = new FileStream(file1.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new BinaryReader(fs1);

            FileStream fs2 = new FileStream(file2.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br2 = new BinaryReader(fs2);

            byte[] fileOne = br1.ReadBytes((int)fs1.Length);
            byte[] fileTwo = br2.ReadBytes((int)fs2.Length);

            fs1.Close();
            br1.Close();

            fs2.Close();
            br2.Close();

            if (fileOne.Length != fileTwo.Length)
                return false;

            for (int i = 0; i < fileOne.Length && i < fileTwo.Length; i++)
                if (fileOne[i] != fileTwo[i])
                    return false;

            return true;
        }


        private static void WriteResults(ref List<StringBuilder> results)
        {
            Directory.CreateDirectory(Path.Combine(_directory, "results"));

            // Compare Files
            StringBuilder[] sameResultsTest = CompareFiles().ToArray();

            StreamWriter sw1 = new StreamWriter(Path.Combine(_directory, "results", "testResultsAreSame.txt"));

            foreach (StringBuilder line in sameResultsTest)
            {
                sw1.WriteLine(line.ToString());
            }

            sw1.Flush();
            sw1.Close();





            StringBuilder[] testResults = results.ToArray();

            StreamWriter sw = new StreamWriter(Path.Combine(_directory, "results", "testResults.txt"));

            foreach (StringBuilder line in testResults)
            {
                sw.WriteLine(line.ToString());
            }


            sw.Flush();
            sw.Close();




            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                MessageBox mb = new MessageBox();
                mb.text.Text = "Done with writing of result files";
                mb.Show();
            });
        }
    }
}
