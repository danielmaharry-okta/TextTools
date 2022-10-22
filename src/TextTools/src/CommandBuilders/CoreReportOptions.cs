namespace TextTools.Enums
{
	using System.CommandLine;
	using System.CommandLine.Binding;

	/// <summary>
	/// Complex type holding all standard options for report generating commands
	/// </summary>
	public class ReportCommandBaseOptions
	{
		/// <summary>
		/// Gets or sets the source directory to find the files to process
		/// </summary>
		public DirectoryInfo SourceDirectory { get; set; } = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

		/// <summary>
		/// Gets or sets a value indicating whether the command should look recursively through all subfolders of the source directory or not
		/// </summary>
		/// <value><c>true</c> to set recursive search. <c>false</c> to find files only in the one folder</value>
		public bool RecurseDirectories { get; set; } = false;

		/// <summary>
		/// Gets or sets the file pattern for files to use within source directory.
		/// </summary>
		public string FilePattern { get; set; } = "*.*";

		/// <summary>
		/// Gets or sets the output type for the result of the script. The default is a CSV file.
		/// </summary>
		public ReportFileType OutputType { get; set; } = ReportFileType.Csv;

		/// <summary>
		/// Gets or sets the directory to save the end report to.
		/// </summary>
		public DirectoryInfo OutputDirectory { get; set; } = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
	}

	/// <summary>
	/// Binder class for parsing report command base options from the command line
	/// </summary>
	public class ReportCommandBaseOptionsBinder : BinderBase<ReportCommandBaseOptions>
	{
		private readonly Option<DirectoryInfo> _sourceDirectoryOption;
		private readonly Option<bool> _recurseDirectoriesOption;
		private readonly Option<string> _filePatternOption;
		private readonly Option<ReportFileType> _outputTypeOption;
		private readonly Option<DirectoryInfo> _outputDirectoryOption;

		/// <summary>
		/// Creates an instance of the <see cref="ReportCommandBaseOptionsBinder" /> class
		/// </summary>
		/// <param name="sourceDirectoryOption">The source directory option</param>
		/// <param name="recurseDirectoriesOption">The recurse directories option</param>
		/// <param name="filePatternOption">The file pattern option</param>
		/// <param name="outputTypeOption">The output type option</param>
		/// <param name="outputDirectoryOption">The output directory option</param>
		public ReportCommandBaseOptionsBinder(
			Option<DirectoryInfo> sourceDirectoryOption,
			Option<bool> recurseDirectoriesOption,
			Option<string> filePatternOption,
			Option<ReportFileType> outputTypeOption,
			Option<DirectoryInfo> outputDirectoryOption
			)
		{
			_sourceDirectoryOption = sourceDirectoryOption;
			_recurseDirectoriesOption = recurseDirectoriesOption;
			_filePatternOption = filePatternOption;
			_outputTypeOption = outputTypeOption;
			_outputDirectoryOption = outputDirectoryOption;
		}

		/// <summary>
		/// Binds the Binder class to actual values for the ReportCommandBaseOptions class
		/// </summary>
		/// <param name="bindingContext">The binding context</param>
		/// <returns>A new <see cref="ReportCommandBaseOptions" /> class</returns>
		protected override ReportCommandBaseOptions GetBoundValue(BindingContext bindingContext)
		{
			var sourceDirectory = bindingContext.ParseResult.GetValueForOption<DirectoryInfo>(_sourceDirectoryOption);
			var filePattern = bindingContext.ParseResult.GetValueForOption<string>(_filePatternOption);
			var outputDirectory = bindingContext.ParseResult.GetValueForOption<DirectoryInfo>(_outputDirectoryOption);

			var options = new ReportCommandBaseOptions();
			if (sourceDirectory is not null)
			{
				options.SourceDirectory = sourceDirectory;
			}

			if (outputDirectory is not null)
			{
				options.OutputDirectory = outputDirectory;
			}

			if (!string.IsNullOrWhiteSpace(filePattern))
			{
				options.FilePattern = filePattern;
			}

			options.RecurseDirectories = bindingContext.ParseResult.GetValueForOption<bool>(_recurseDirectoriesOption);
			options.OutputType = bindingContext.ParseResult.GetValueForOption<ReportFileType>(_outputTypeOption);

			return options;
		}
	}
}