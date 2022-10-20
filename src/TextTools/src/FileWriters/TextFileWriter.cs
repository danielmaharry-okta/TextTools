namespace TextTools.FileWriters
{
   using System.Text;

   /// <summary>
   /// Represents a writer that provides an easy way to create text-based files from utility output - text, StringBuilder, worksheets, xml
   /// </summary>
   public class TextFileWriter : CommonFileWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="TextFileWriter" /> class.
      /// </summary>
      public TextFileWriter()
      : base()
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="TextFileWriter" /> class.
      /// </summary>
      /// <param name="filename">The name of the file to write without file extension</param>
      public TextFileWriter(string filename)
      : base(filename)
      {
      }

      /// <summary>
      /// Writes the contents of a StringBuilder to a text file.
      /// </summary>
      /// <param name="sb">The <see cref="System.Text.StringBuilder" /> to write to the file</param>
      public void WriteToTextFile(StringBuilder sb)
      {
         WriteToTextFile(sb?.ToString() ?? string.Empty);
      }

      /// <summary>
      /// Writes the contents of a string to a text file.
      /// </summary>
      /// <param name="s">The <see cref="string" /> to write to the file</param>
      public void WriteToTextFile(string s)
      {
         WriteToTextFile(s, true);
      }

      /// <summary>
      /// Writes the contents of a string to a text file.
      /// </summary>
      /// <param name="s">The <see cref="string" /> to write to the file</param>
      /// <param name="includeDateSuffix"><c>true</c> if the file should end with current time to create unique files.true <c>false</c> if not</param>
      public void WriteToTextFile(string s, bool includeDateSuffix)
      {
         string finalFileLocation = GenerateOutputFileLocation(includeDateSuffix);
         File.WriteAllText(finalFileLocation, s, Encoding.UTF8);
         Console.WriteLine($"Written results to file {finalFileLocation}");
      }

      /// <summary>
      /// Writes the contents of an <see cref="System.Collections.Generic.IEnumerable{T}" /> of <see cref="System.Text.StringBuilder" /> to a text file.
      /// </summary>
      /// <param name="wordList">The <see cref="System.Collections.Generic.IEnumerable{T}" /> of <see cref="System.Text.StringBuilder" /> to write to the file</param>
      public void WriteToTextFile(IEnumerable<StringBuilder> wordList)
      {
         WriteToTextFile(wordList.Select(sb => sb?.ToString() ?? string.Empty));
      }

      /// <summary>
      /// Writes the contents of an <see cref="System.Collections.Generic.IEnumerable{T}" /> of strings to a text file.
      /// </summary>
      /// <param name="stringList">The <see cref="System.Collections.Generic.IEnumerable{T}" /> of strings to write to the file</param>
      public void WriteToTextFile(IEnumerable<string> stringList)
      {
         WriteToTextFile(stringList, true);
      }

      /// <summary>
      /// Writes the contents of an <see cref="System.Collections.Generic.IEnumerable{T}" /> of strings to a text file.
      /// </summary>
      /// <param name="stringList">The <see cref="System.Collections.Generic.IEnumerable{T}" /> of strings to write to the file</param>
      /// <param name="includeDateSuffix"><c>true</c> if the file should end with current time to create unique files.true <c>false</c> if not</param>
      public void WriteToTextFile(IEnumerable<string> stringList, bool includeDateSuffix)
      {
         string finalFileLocation = GenerateOutputFileLocation(includeDateSuffix);
         File.WriteAllLines(finalFileLocation, stringList, Encoding.UTF8);
         Console.WriteLine($"Written {stringList.Count()} words to {finalFileLocation}");
      }

      /// <summary>
      /// Writes the contents of a <see cref="Worksheet" /> to a text file.
      /// </summary>
      /// <param name="worksheet">The <see cref="Worksheet" /> to write to the file</param>
      public void WriteToTextFile(Worksheet worksheet)
      {
         WriteToTextFile(new List<Worksheet>() { worksheet });
      }

      /// <summary>
      /// Writes the contents of an <see cref="IEnumerable{T}" /> of <see cref="Worksheet" /> to a text file.
      /// </summary>
      /// <param name="worksheets">The <see cref="IEnumerable{T}" /> of <see cref="Worksheet" /> to write to the file</param>
      public void WriteToTextFile(IEnumerable<Worksheet> worksheets)
      {
         if (worksheets is null || worksheets.None())
         {
            Console.WriteLine("Attempted to write no worksheets to a file, so created nothing");
            return;
         }

         foreach (var worksheet in worksheets)
         {
            NewFileName = $"{NewFileName}-{worksheet.Name}";
            string finalFileLocation = GenerateOutputFileLocation();
            File.WriteAllLines(
					finalFileLocation, 
					worksheet.Rows.Select(row => row.AsCharacterSeparatedList('|',false)), 
					Encoding.UTF8);
            Console.WriteLine($"Written {worksheet.Rows.Count} rows to {finalFileLocation}");
         }
      }

      private string GenerateOutputFileLocation() => GenerateOutputFileLocation("txt");

      private string GenerateOutputFileLocation(
			bool includeDateSuffix) => GenerateOutputFileLocation("txt", includeDateSuffix);
   }
}
