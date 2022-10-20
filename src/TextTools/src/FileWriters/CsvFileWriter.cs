namespace TextTools.FileWriters
{
   using System.IO;
   using System.Globalization;
   using CsvHelper;

   /// <summary>
   /// Represents a writer that provides an easy way to create Csv files using the basic worksheet class.
   /// </summary>
   public class CsvFileWriter : CommonFileWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="CsvFileWriter" /> class.
      /// </summary>
      public CsvFileWriter()
      : base()
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CsvFileWriter" /> class.
      /// </summary>
      /// <param name="filename">The name of the file to write without file extension</param>
      public CsvFileWriter(string filename)
      : base(filename)
      {
      }

      /// <summary>
      /// Writes the contents of a <see cref="Worksheet" /> to a Csv file
      /// </summary>
      /// <param name="worksheet">The <see cref="Worksheet" /> to write to the Csv file</param>
      public void WriteToCsvFile(Worksheet worksheet) => WriteToCsvFile(new List<Worksheet>() { worksheet });

      /// <summary>
      /// Writes the contents of a list of <see cref="Worksheet" />s to a Csv file
      /// </summary>
      /// <param name="worksheets">The <see cref="System.Collections.Generic.List{T}" /> of  <see cref="Worksheet" /> to write to the Csv file</param>
      public void WriteToCsvFile(List<Worksheet> worksheets)
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

            using (var writer = new StreamWriter(finalFileLocation))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
               foreach (var row in worksheet.Rows)
               {
                  foreach (var field in row)
                  {
                     csv.WriteField(field);
                  }

                  csv.NextRecord();
               }

               writer.Flush();
            }

            Console.WriteLine($"Written {worksheet.Rows.Count} rows to {finalFileLocation}");
         }
      }

      private string GenerateOutputFileLocation() => GenerateOutputFileLocation("csv");
   }
}
