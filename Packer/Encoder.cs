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

            while (fsRead.Position < fsRead.Length)
            {
                // Creating List for saving same values and adding Value of File at Position
                List<byte> sameBytes = new List<byte>
                {
                    // First value readed
                    br.ReadByte()
                };

                // While first readed value is same as next readed value
                while (sameBytes[0] == br.ReadByte())
                {
                    // Position must be decreased by 1 bc ReadByte() increases it automatically
                    fsRead.Position--;
                    sameBytes.Add(br.ReadByte());

                    // Stop while loop if end of file is reached
                    if (fsRead.Position == fsRead.Length)
                        break;
                }

                // Decreasing Position only if not at end
                // Else main-while-loop would still be going
                if (fsRead.Position != fsRead.Length)
                    fsRead.Position--;


                if(sameBytes.Count > 3 || sameBytes[0] == Values.marker)
                {
                    // Writes in format
                    bw.Write((byte)Values.marker);
                    bw.Write(sameBytes.Count);
                    bw.Write(sameBytes[0]);
                }
                else
                    // Writes normal
                    for(int count = 0; count < sameBytes.Count; count++)
                        bw.Write(sameBytes[count]);
                
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
