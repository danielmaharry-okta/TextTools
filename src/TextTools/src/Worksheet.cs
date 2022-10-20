namespace TextTools
{
    /// <summary>
    /// Represents a simple worksheet to send to an Excel file
    /// </summary>
    public class Worksheet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Worksheet" /> class.
        /// </summary>
        public Worksheet()
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="Worksheet" /> class.
        /// </summary>
        /// <param name="sheetName">The name of the worksheet</param>
        public Worksheet(string sheetName)
        {
            Name = sheetName;
        }

        /// <summary>
        /// Gets or sets the name of the worksheet
        /// </summary>
        /// <value>The name of the worksheet</value>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets a <see cref="System.Collections.Generic.List{T}" /> of <see cref="System.Collections.Generic.List{T}" /> of <see cref="string" /> representing the rows in the worksheet
        /// </summary>
        /// <returns>The rows in the worksheet</returns>
        public List<List<string>> Rows { get; } = new List<List<string>>();
    }
}