using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Packer
{
    static class Encoder
    {
        // Variable for saving Lenght of FileStream
        private static long fsReadLength;

        /// <summary>
        /// Decreases size of a file throug encoding
        /// </summary>
        public static void Encode()
        {

            // Updates Destination Values
            General.UpdateDestValues(false);

            // Stream for reading Original File
            FileStream fsRead = new FileStream(Values.source.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fsRead);

            // Saves Length to variable for optimization 
            // else getter of FileStream object needs to be called often
            fsReadLength = fsRead.Length;

            // Stream for writing Encoded File
            FileStream fsWrite = new FileStream(Values.destinationPath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fsWrite);

            // Searches least used value in File and sets it to Marker
            Values.marker = (char)SearchForMarker(fsRead, br);

            // Writes Header to File
            WriteHeader(bw);


            // While not at end of file
            while (fsRead.Position != fsReadLength)
            {
                // First Byte will  be used for comparision
                byte firstByte = br.ReadByte();
                // Count of the same Bytes after First Byte
                int sameCount = 1;


                // While not at the second to last Position of file
                // So that last byte can also be read
                if (fsRead.Position < fsReadLength)
                {
                    // While next read byte is the same as first read byte
                    while (fsRead.Position != fsReadLength && firstByte == br.ReadByte() && sameCount < 255)
                        sameCount++;

                    // Decreases Position by 1
                    if (fsRead.Position != fsReadLength)
                        fsRead.Position--;
                }


                // Writes Byte in Format if there are more then 3 same bytes next to each other
                // Or the first read Byte is the marker
                if (sameCount > 3 || firstByte == (byte)Values.marker)
                {
                    // Writes marker as byte
                    bw.Write((byte)Values.marker);
                    // Writes count of bytes as Int32
                    bw.Write((byte)sameCount);
                    // Writes byte
                    bw.Write(firstByte);
                }
                // Else writes all Bytes one after another
                else
                    for (int count = 0; count < sameCount; count++)
                        bw.Write(firstByte);

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
        /// Writes header in file
        /// </summary>
        /// <param name="bw"> BinaryWriter to write header to </param>
        private static void WriteHeader(BinaryWriter bw)
        {
            // Write header to file
            bw.Write(Encoding.ASCII.GetBytes(Values.magicNbr));         // Magic Number for ttpack-identification
            bw.Write((byte)Values.marker);                              // Used marker
            bw.Write(Encoding.ASCII.GetBytes(CreateFileName()));        // Sutiable File-Name (8 byte long) for header
            bw.Write(Encoding.ASCII.GetBytes(Values.source.Extension)); // Extension of original file
            bw.Write((byte)Values.endOfHeader);                         // End of header
        }


        /// <summary>
        /// Creates sutiable file name for header (8 byte long)
        /// </summary>
        /// <returns> new file name to write to header </returns>
        private static string CreateFileName()
        {
            // Gets last Index of dot character in SourceName
            int lastIndexOfDot = Values.source.Name.LastIndexOf(Values.dot);
            // StringBuilder for source Name that will later be added to header
            StringBuilder sourceNameSub = new StringBuilder(Values.source.Name);


            // Removes extension
            if (lastIndexOfDot > 0)
                if (Values.source.Name.Substring(lastIndexOfDot) == Values.source.Extension) // If substring that starts at last Index of dot is extension
                    // Remove Extension
                    sourceNameSub.Remove(lastIndexOfDot, Values.source.Extension.Length);


            // Ensures length of name is exactly 8
            if (sourceNameSub.Length < Values.maxNameLength) // If sourceName without extension is lower then 8
                // Replace empty chars with '-'
                sourceNameSub.Append(Values.tilde, (Values.maxNameLength - sourceNameSub.Length));
            else
                // Remove everything from 8
                sourceNameSub.Remove(Values.maxNameLength, (sourceNameSub.Length - Values.maxNameLength)).Append(Values.tilde);

            return sourceNameSub.ToString();
        }


        /// <summary>
        /// Searches for least used value to use as marker
        /// </summary>
        /// <param name="fsRead"> FileStream to read marker from </param>
        /// <param name="br"> BinaryReder of FileStream </param>
        /// <returns> Index of least used value </returns>
        private static int SearchForMarker(FileStream fsRead, BinaryReader br)
        {
            // Array with every ASCII-Sign as Index
            int[] ascii = new int[256];

            while (fsRead.Position < fsReadLength)
                ascii[br.ReadByte()]++;

            // Resets Position
            fsRead.Position = 0;

            return Array.IndexOf(ascii, ascii.Min());
        }


    }
}
