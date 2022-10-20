namespace TextTools.CommandBuilders
{
   using System.CommandLine;
   using TextTools.Enums;

   /// <summary>
   /// Represents core command-line options for each utility.
   /// </summary>
   public abstract class CommandBuilderBase
   {
      /// <summary>
      /// Gets an option to set the source directory for the command
      /// </summary>
      protected Option<DirectoryInfo> SourceDirectoryOption { get; } = new Option<DirectoryInfo>(
         new string[] { "-d", "--sourceDirectory" },
         "Source directory"
      );

      /// <summary>
      /// Gets an option to recurse through all directories
      /// </summary>
      protected Option<bool> RecurseDirectoriesOption { get; } = new Option<bool>(
         new string[] { "-r", "--recurseDirectories" },
         getDefaultValue: () => false,
         "Add this flag to process all files in directory specified with -d AND all its child directories."
      );

      /// <summary>
      /// Gets an option to specify the type of file returned by the command
      /// </summary>
      protected Option<ReportFileType> OutputTypeOption { get; } = new Option<ReportFileType>(
         new string[] { "-t", "--outputType" },
         getDefaultValue: () => ReportFileType.Csv,
         "Sets the type of file output by the script. Options are csv, txt."
      );

      /// <summary>
      /// Gets an option to set the output directory for new files created by the command
      /// </summary>
      protected Option<DirectoryInfo> OutputDirectoryOption { get; } = new Option<DirectoryInfo>(
         new string[] { "-o", "--outputDirectory" },
         getDefaultValue: () => new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)),
         "Output directory for new files."
      ).ExistingOnly();

      /// <summary>
      /// Returns an option to set a regex for the files in the source directory
      /// </summary>
      /// <param name="pattern">A default value for the file regex</param>
      /// <returns>An option to set a regex for the files in the source directory</returns>
      protected Option<string> FilePatternOption(string pattern)
      {
         return new Option<string>(
            new string[] { "-f", "--filePattern" },
            getDefaultValue: () => pattern,
            $"File pattern for files to use within source directory. Default is {pattern}"
         );
      }

      /// <summary>
      /// Adds the five core options for a report command - source directory, output type, recurse directory, file pattern, and output directory
      /// </summary>
      /// <param name="subCommand">The <see cref="System.CommandLine.Command" /> to add the options to.</param>
      /// <param name="pattern">A file pattern <see cref="System.CommandLine.Option{T}" /></param>
      protected ReportCommandBaseOptionsBinder CreateAndAddCoreReportCommandOptions(Command subCommand, string pattern)
      {
         subCommand.AddOption(SourceDirectoryOption);
         subCommand.AddOption(OutputTypeOption);
         subCommand.AddOption(RecurseDirectoriesOption);
         subCommand.AddOption(FilePatternOption(pattern));
         subCommand.AddOption(OutputDirectoryOption);
			return new ReportCommandBaseOptionsBinder(SourceDirectoryOption, RecurseDirectoriesOption,
						FilePatternOption(pattern), OutputTypeOption, OutputDirectoryOption);
      }
   }
}