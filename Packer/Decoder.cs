using System.Linq;
using System.Text;
using System.IO;

namespace Packer
{
    public static class Decoder
    {

        private static long fsReadLength;

        /// <summary>
        /// Decodes the File
        /// </summary>
        public static void Decode()
        {

            // Stream for Reading
            FileStream fsRead = new FileStream(Values.source.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fsRead);

            // Saves Length of FileStream to a Variable
            // Makes the program run faster
            fsReadLength = fsRead.Length;

            // Read File Name out of Header
            ReadFileName(fsRead, br);

            General.UpdateDestValues(true);

            // Stream for writing
            FileStream fsWrite = new FileStream(Values.destinationPath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fsWrite);

            
            // While not at end of File
            while (fsRead.Position < fsReadLength)
            {
                // Variable for Character and the count of the Character
                int count = 1;
                byte character = br.ReadByte();

                
                if (character == Values.marker)
                {// If Marker found
                    // Read Count of Character
                    count = System.Convert.ToInt32(br.ReadByte());
                    // Read Character
                    character = br.ReadByte();
                }

                // Write Characters to new file
                for (int index = 0; index < count; index++)
                    bw.Write(character);
            }


            // Flushes all Streams
            fsRead.Flush();
            fsWrite.Flush();

            // Closes all Streams
            fsRead.Close();
            br.Close();
            fsWrite.Close();
            bw.Close();


        }

        /// <summary>
        /// Reads Marker and FileName out of the Header
        /// </summary>
        /// <param name="fsRead"></param>
        /// <param name="br"></param>
        private static void ReadFileName(FileStream fsRead, BinaryReader br)
        {
            // Set Position to 0 to ensure it reads at start
            fsRead.Position = 0;

            // Checking if File to read has Header
            // SequenceEqual checks if values of two arrays are the same
            if (Enumerable.SequenceEqual(br.ReadBytes(Values.magicNbr.Length), Encoding.ASCII.GetBytes(Values.magicNbr))) 
            {
                // Read marker out of Header
                Values.marker = (char)br.ReadByte();
                // Read original Name out of Header
                Values.destinationFileName = Encoding.ASCII.GetString(br.ReadBytes(Values.maxNameLength));
                
                // Gets Extension
                while (br.ReadByte() != (byte)Values.endOfHeader) // While not at end of header
                {
                    fsRead.Position--;
                    Values.destinationFileName += (char)br.ReadByte();
                } 

            }
        }
    }
}
