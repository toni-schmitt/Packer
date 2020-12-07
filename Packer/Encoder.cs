using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Packer
{
    static class Encoder
    {

        /// <summary>
        /// Decreases size of a file throug encoding
        /// </summary>
        public static void Encode()
        {
            
            // Updates Destination Values
            General.UpdateDestValues();

            // Stream for reading Original File
            FileStream fsRead = new FileStream(Values.source.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fsRead);

            // Stream for writing Encoded File
            FileStream fsWrite = new FileStream(Values.destFilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fsWrite);

            // Searches least used value in File and sets it to Marker
            Values.marker = (char)SearchForMarker(fsRead, br);

            // Writes Header to File
            WriteHeader(bw);


            // While not at end of file
            while (fsRead.Position < fsRead.Length)
            {
                // First Byte will  be used for comparision
                byte firstByte = br.ReadByte();
                // Count of the same Bytes after First Byte
                int sameCount = 1;


                // While not at the second to last Position of file
                // So that last byte can also be read
                if (fsRead.Position < fsRead.Length - 1)
                {
                    // While next read byte is the same as first read byte
                    while (fsRead.Position != fsRead.Length && firstByte == br.ReadByte())
                        sameCount++;

                    // Decreases Position by 1, bc last iteration of while-loop increases position
                    if (fsRead.Position != fsRead.Length)
                        fsRead.Position--;
                }


                // Writes Byte in Format if there are more then 3 same bytes next to each other
                // Or the first read Byte is the marker
                if (sameCount > 3 || firstByte == (byte)Values.marker)
                {
                    // Writes marker as byte
                    bw.Write((byte)Values.marker);
                    // Writes count of bytes as Int32
                    bw.Write(sameCount);
                    // Writes byte
                    bw.Write(firstByte);
                }
                // Else writes all Bytes one after another
                else
                    for (int count = 0; count < sameCount; count++)
                        bw.Write(firstByte);

            }

            // Closes all Streams
            fsRead.Flush();
            fsRead.Close();
            br.Close();

            fsWrite.Flush();
            fsWrite.Close();
            bw.Close();


        }
        private static void WriteHeader(BinaryWriter bw)
        {
            bw.Write(Encoding.ASCII.GetBytes(Values.header));
            bw.Write((byte)Values.marker);
            bw.Write(Values.source.Name.Length);
            bw.Write(Encoding.ASCII.GetBytes(Values.source.Name));
        }



        private static int SearchForMarker(FileStream fsRead, BinaryReader br)
        {
            int[] ascii = new int[256];
            while (fsRead.Position < fsRead.Length)
                ascii[br.ReadByte()]++;
            fsRead.Position = 0;
            return Array.IndexOf(ascii, ascii.Min());
        }

        
    }
}
