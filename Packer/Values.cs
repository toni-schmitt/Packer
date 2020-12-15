namespace Packer
{
    public static class Values
    {
        // Infos for source and destination file
        public static System.IO.FileInfo source;    // FileInfo for source file

        public static string destinationPath;          // Path of destination file
        public static string destinationDirectory;     // Directory of destination
        public static string destinationFileName;          // File Name of destination


        // Variable for marker 
        public static char marker;

        // Values for header
        public static string magicNbr = "ttpack";           // Magic Number of .ttpack-files
        public static string ttpackExtension = ".ttpack";   // File Extension of Encoded files
        public static int maxNameLength = 8;                // Max Length of original File in header
        public static char endOfHeader = '\0';              // End of header

        public static char dot = '.';
        public static char tilde = '~';
    }
}
