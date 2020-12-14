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
        public static string _directory;
        public static string SetDirectory { set { _directory = value; } }

        private static readonly char _colon = ':';



        public static void StartTest()
        {
            // Encode or Decode or Encode and Decode
            // Get Files
            // Do it for every file
            // Save time

            // First File of Dir = source
            // destDir = sourceDir
            // destName.Text = source.Name

            // UPDATEDESTVALUES:
            //  ENCODING:
            //      destFileName = destName.Text + .ttpack
            //  DECODING:
            //      destFileName = read from header
            //  destPath = destDir + destFileName


            FileInfo[] info = new DirectoryInfo(_directory).GetFiles();
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


            List<StringBuilder> results = new List<StringBuilder>();

            for (int chooseOperation = 0; chooseOperation < 2; chooseOperation++)
            {
                FileInfo[] files = null;
                switch (chooseOperation)
                {
                    case 0:
                        files = new DirectoryInfo(_directory).GetFiles();
                        break;
                    case 1:
                        files = new DirectoryInfo(_directory).GetFiles("*.ttpack");
                        break;
                }

                foreach (FileInfo file in files)
                {
                    StringBuilder result = new StringBuilder();

                    Values.source = file;
                    Values.destinationDirectory = Values.source.DirectoryName;
                    System.Windows.Application.Current.Dispatcher.Invoke(() => System.Windows.Application.Current.Windows.OfType<MainWindow>().First().destinationName.Text = Values.source.Name);

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


                    long orgLength = file.Length;
                    long newLength = new FileInfo(Values.destinationPath).Length;
                    long differenceLenghts = orgLength - newLength;

                    result.Append(file.Name).Append(_colon)
                        .Append(file.Extension).Append(_colon)
                        .Append(string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}:{4:D3}", timer.Elapsed.Days, timer.Elapsed.Hours, timer.Elapsed.Minutes, timer.Elapsed.Seconds, timer.Elapsed.Milliseconds)).Append(_colon)
                        .Append(orgLength).Append(_colon)
                        .Append(newLength).Append(_colon)
                        .Append(differenceLenghts);

                    results.Add(result);

                    

                    
                }

                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    MessageBox mb = new MessageBox();
                    mb.text.Text = "Done with test" + chooseOperation;
                    mb.Show();
                });

            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                MessageBox mb = new MessageBox();
                mb.text.Text = "Writing result files now";
                mb.Show();
            });

            List<StringBuilder> sameResultsList = new List<StringBuilder>();

            var orgFiles = new DirectoryInfo(_directory).EnumerateFiles().Where(f => !f.Extension.Equals(".ttpack") && Path.GetFileNameWithoutExtension(f.Name).Length > 8);
            var decodedFiles = new DirectoryInfo(_directory).EnumerateFiles().Where(f => Path.GetFileNameWithoutExtension(f.Name).Length == 8);

            //var orgFiles = new DirectoryInfo(_directory).GetFiles("????????*.*")/*.Where(file => !file.Name.EndsWith(".ttpack"))*/;
            //var decodedFiles = new DirectoryInfo(_directory).GetFiles("????????.*")/*.Where(file => !file.Name.EndsWith(".ttpack"))*/;

            if (orgFiles.Count() == decodedFiles.Count())

                for (int i = 0; i < orgFiles.Count(); i++)
                {
                    StringBuilder isSameResult = new StringBuilder();
                    isSameResult.Append(CompareFiles(orgFiles.ElementAt(i), decodedFiles.ElementAt(i))).Append(_colon)
                        .Append(orgFiles.ElementAt(i).Length).Append(_colon)
                        .Append(decodedFiles.ElementAt(i).Length);
                    sameResultsList.Add(isSameResult);
                }


            Directory.CreateDirectory(Path.Combine(_directory, "results"));

            StringBuilder[] testResults = results.ToArray();

            StreamWriter sw = new StreamWriter(Path.Combine(_directory, "results", "testResults.txt"));

            foreach (StringBuilder line in testResults)
            {
                sw.WriteLine(line.ToString());
            }
            

            sw.Flush();
            sw.Close();
            

            StringBuilder[] sameResultsTest = sameResultsList.ToArray();

            StreamWriter sw1 = new StreamWriter(Path.Combine(_directory, "results", "testResultsAreSame.txt"));

            foreach (StringBuilder line in sameResultsTest)
            {
                sw1.WriteLine(line.ToString());
            }

            sw1.Flush();
            sw1.Close();

            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                MessageBox mb = new MessageBox();
                mb.text.Text = "Done with writing of result files";
                mb.Show();
            });

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

            for (int i = 0; i < fileOne.Length && i < fileTwo.Length; i++)
                if (fileOne[i] != fileTwo[i])
                    return false;

            return true;
        }
    }
}
