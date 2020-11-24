using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packer
{
    public static class Values
    {
        public static string sourceFilePath;
        public static string sourceFileDirectory;
        public static string sourceFileName;

        public static string destFilePath;
        public static string destFileDirectory;
        public static string destFileName;

        public static char marker = '{';

        public static string header = "ttpack" + marker;
    }
}
