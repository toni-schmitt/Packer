using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Packer
{
    static class Encoder
    {
        static FileStream fsRead;
        static BinaryReader br;

        static FileStream fsWrite;
        static BinaryWriter bw;
        /// <summary>
        /// Decreases size of a file throug encoding
        /// </summary>
        public static void Encode()
        {

            // Stream for reading Original File
            fsRead = new FileStream(Values.sourceFilePath, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fsRead);

            // Stream for writing Encoded File
            fsWrite = new FileStream(Values.destFilePath, FileMode.Create, FileAccess.Write);
            bw = new BinaryWriter(fsWrite);

            //Values.marker = SearchForMarker();

            // Writes header and sourceFileName at first pos in file
            bw.Write(Values.header);
            bw.Write(Values.sourceFileName);

            while (fsRead.Position < fsRead.Length)
            {
                // List for easy saving of same values
                List<byte> sameBytes = new List<byte>();

                // First value readed
                sameBytes.Add(br.ReadByte());

                // While first readed value is same as next readed value
                while(sameBytes[0] == br.ReadByte())
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


                if(sameBytes.Count > 3)
                {
                    // Writes in format
                    bw.Write((byte)Values.marker);
                    bw.Write(sameBytes.Count);
                    bw.Write(sameBytes[0]);
                } else
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

        private static char SearchForMarker()
        {
            byte[] allValues = br.ReadBytes((int)fsRead.Length);
            Array.Sort(allValues);

            Dictionary<byte, int> values = new Dictionary<byte, int>();

            int count = 1;
            byte[] specialChars =
            {
                33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,58,59,60,61,62,63,64,91,92,93,94,95,96,123,124,125,126
            };

            bool canBeAddedToList = false;

            for (int index = 0; index < allValues.Length; index++)
            {
                if (!canBeAddedToList)
                {
                    for (int specialChar = 0; specialChar < specialChars.Length; specialChar++)
                    {
                        if (allValues[index] == specialChars[specialChar])
                        {
                            canBeAddedToList = true;
                            break;
                        }
                    }
                }
                if (canBeAddedToList)
                {
                    if (allValues[index] == allValues[index + 1])
                    {
                        count++;
                    }
                    else
                    {
                        canBeAddedToList = false;
                        values.Add(allValues[index], count);
                        count = 1;
                    }
                }
            }

            byte mostUsedByte = values.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            return (char)mostUsedByte;
        }

        /// <summary>
        /// Checks if File has header
        /// </summary>
        /// <returns>true if file has header, false if file has no header</returns>
        public static bool HasHeader()
        {
            // Open Streams
            fsRead = new FileStream(Values.sourceFilePath, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fsRead);
            // Write to variable bc Streams need to be closed before returning value
            string read = br.ReadString();
            // Closing Streams
            fsRead.Flush();
            fsRead.Close();
            br.Close();
            // Check if variable matches Values.Header
            if (read == Values.header)
                return true;
            else
                return false;
        }
    }
}
