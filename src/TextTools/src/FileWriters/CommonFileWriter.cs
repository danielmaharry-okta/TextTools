namespace TextTools.FileWriters
{
    /// <summary>
    /// An abstract class that provides an easy way to write files from utility output - text, StringBuilder, xml files
    /// </summary>
    public abstract class CommonFileWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonFileWriter" /> class.
        /// </summary>
        public CommonFileWriter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonFileWriter" /> class.
        /// </summary>
        /// <param name="filename">The name of the file to write without file extension</param>
        public CommonFileWriter(string filename)
        {
            NewFileName = filename;
        }

        /// <summary>
        /// Gets or sets the name of the file to write - no file extension required
        /// </summary>
        public string NewFileName { get; set; } = "UnnamedFile";

        /// <summary>
        /// Gets or sets the folder to save file in - by default the desktop folder
        /// </summary>
        public string NewFileLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        /// <summary>
        /// Returns the name of the file to be created
        /// </summary>
        /// <param name="fileExtension">The file extension of the file to br created</param>
        /// <returns>The name of the file to be created</returns>
        internal string GenerateOutputFileLocation(string fileExtension)
        {
            return GenerateOutputFileLocation(fileExtension, true);
        }

        /// <summary>
        /// Returns the name of the file to be created
        /// </summary>
        /// <param name="fileExtension">The file extension of the file to be created</param>
        /// <param name="includeDateSuffix"><c>true</c> if the file should end with current time to create unique files.true <c>false</c> if not</param>
        /// <returns>The name of the file to be created</returns>
        internal string GenerateOutputFileLocation(string fileExtension, bool includeDateSuffix)
        {
            string dateSuffix = $"-{DateTime.Now.ToString("HHmmss")}";
            string newFileName = $"{NewFileName}{(includeDateSuffix ? dateSuffix : string.Empty)}.{fileExtension}";
            return Path.Combine(NewFileLocation, newFileName.AsSafeFileName());
        }
    }
}