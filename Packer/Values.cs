namespace Packer
{
    public static class Values
    {
        // FileInfo for source File
        public static System.IO.FileInfo source;

        // Values for destination file
        public static string destFilePath;
        public static string destFileDirectory;
        public static string destFileName;

        // Marker used
        public static char marker;

        // Values for header
        public static string header = "ttpack";
        public static string ttpackExtension = ".ttpack";
    }
}
