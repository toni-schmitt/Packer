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
        private static char _cMarker = '{';
        private static string _sFile;
        public static string GetFile
        {
            get { return _sFile; }
            set { _sFile = value; }
        }
        private static string _sNewFile = _sFile + ".ttpack";
        private static char _cMagicNbr;


        /// <summary>
        /// Decreases size of a file throug encoding
        /// </summary>
        /// <param name="filename">name of file to encode</param>
        public static void Encode(string filename)
        {
            _sFile = filename;
            // Stream for reading Original File
            FileStream fsRead = new FileStream(_sFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fsRead);

            // Stream for writing Encoded File
            FileStream fsWrite = new FileStream(_sNewFile, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fsWrite);

            
            while(fsRead.Position < fsRead.Length)
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
                    bw.Write((byte)_cMarker);
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
    }
}
