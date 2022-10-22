namespace TextTools.CommandHandlers
{
   using TextTools.Enums;
   using TextTools.FileWriters;

   /// <summary>
   /// Abstract base class for all report generating command handlers.
   /// </summary>
   public abstract class ReportCommandHandlerBase : CommandHandlerBase
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="ReportCommandHandlerBase" /> class.
      /// </summary>
      protected ReportCommandHandlerBase()
      {
         FilePattern = "*.txt";
         FileTypeToReturn = ReportFileType.Csv;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ReportCommandHandlerBase" /> class.
      /// </summary>
      /// <param name="coreOptions">The source, output type, recurse and file pattern options for this command</param>
      protected ReportCommandHandlerBase(ReportCommandBaseOptions coreOptions)
      {
         if (coreOptions == null)
         {
            throw new ArgumentNullException(nameof(coreOptions));
         }

         SourceDirectory = coreOptions.SourceDirectory;
         FilePattern = coreOptions.FilePattern;
         RecurseDirectories = coreOptions.RecurseDirectories;
         FileTypeToReturn = coreOptions.OutputType == ReportFileType.Unspecified ? ReportFileType.Csv : coreOptions.OutputType;
         OutputDirectory = coreOptions.OutputDirectory;
      }

      /// <summary>
      /// Gets or sets the source directory to find the files to process
      /// </summary>
      protected DirectoryInfo SourceDirectory { get; set; } = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

		/// <summary>
		/// Gets or sets a value indicating whether the command should look recursively through all subfolders of the source directory or not
		/// </summary>
		/// <value><c>true</c> to set recursive search. <c>false</c> to find files only in the one folder</value>
		protected bool RecurseDirectories { get; set; } = false;

		/// <summary>
		/// Gets or sets the file pattern for files to use within source directory.
		/// </summary>
		protected string FilePattern { get; set; } = "*.txt";

		/// <summary>
		/// Gets or sets the type of file to save the report as
		/// </summary>
		/// <value>A value of the <see cref="ReportFileType" /> enum</value>
		protected ReportFileType FileTypeToReturn { get; set; } = ReportFileType.Csv;

      /// <summary>
      /// Gets or sets the directory to save the reports to
      /// </summary>
      protected DirectoryInfo OutputDirectory { get; } = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

      /// <summary>
      /// Gets the set of Worksheets that will be created by the report
      /// </summary>
      /// <returns>A <see cref="System.Collections.Generic.List{T}" /> of <see cref="Worksheet" /> for the report to create</returns>
      protected List<Worksheet> Reports { get; } = new List<Worksheet>();

      /// <summary>
      /// Gets or sets the file name of the reports to be created
      /// </summary>
      /// <value>The fie name of the reports to be created</value>
      protected string ReportFileName { get; set; } = "NewReport";

      /// <inheritdoc/>
      protected override void RunCommand()
      {
         BuildReport();
         WriteReport();
      }

      /// <inheritdoc/>
      protected override bool ValidateOptions()
      {
         Console.WriteLine($"File pattern : {FilePattern}");
         Console.WriteLine($"Check child directories : {RecurseDirectories}");
         Console.WriteLine($"Output file type : {FileTypeToReturn}");
			Console.WriteLine($"Source Directory : {SourceDirectory.FullName}");
			var sourceFiles = SourceDirectory.EnumerateFiles(FilePattern, RecurseDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			Console.WriteLine($"Count of files : {sourceFiles.Count<FileInfo>()} files");
         Console.WriteLine($"Output Directory : {OutputDirectory.FullName}");
         return ValidateNonCoreOptions();
      }

      /// <summary>
      /// Parses and validates as necessary the values of any non core report command parameters
      /// </summary>
      /// <returns><c>true</c> if all parameters are valid, <c>false</c> otherwise</returns>
      protected abstract bool ValidateNonCoreOptions();

      /// <summary>
      /// Builds the worksheets that make up the report.
      /// </summary>
      protected abstract void BuildReport();

      private void WriteReport()
      {
         switch (FileTypeToReturn)
         {
            case ReportFileType.Csv:
               CsvFileWriter cfw = new CsvFileWriter(ReportFileName);
               cfw.NewFileLocation = OutputDirectory.FullName;
               cfw.WriteToCsvFile(Reports);
               break;
            default:
               TextFileWriter tfw = new TextFileWriter(ReportFileName);
               tfw.NewFileLocation = OutputDirectory.FullName;
               tfw.WriteToTextFile(Reports);
               break;
         }
      }
   }
}