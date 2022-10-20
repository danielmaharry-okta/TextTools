namespace TextTools.Enums
{
    /// <summary>
    /// Enum for different report file types
    /// </summary>
    public enum ReportFileType
    {
        /// <summary>output type is not specified</summary>
        Unspecified = 0,

        /// <summary>command should return a text file</summary>
        Txt,

        /// <summary>command should return a csv file</summary>
        Csv,
    }
}