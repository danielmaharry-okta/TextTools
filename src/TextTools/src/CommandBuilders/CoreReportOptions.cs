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
		public bool RecurseDirectories { get; set; }

		/// <summary>
		/// Gets or sets the file pattern for files to use within source directory.
		/// </summary>
		public string FilePattern { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the output type for the result of the script. The default is a CSV file.
		/// </summary>
		public ReportFileType OutputType { get; set; } = ReportFileType.Csv;

		/// <summary>
		/// Gets or sets the directory to save the end report to.
		/// </summary>
		public DirectoryInfo OutputDirectory { get; set; } = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
	}

	public class ReportCommandBaseOptionsBinder : BinderBase<ReportCommandBaseOptions>
	{
		private readonly Option<DirectoryInfo> _sourceDirectoryOption;
		private readonly Option<bool> _recurseDirectoriesOption;
		private readonly Option<string> _filePatternOption;
		private readonly Option<ReportFileType> _outputTypeOption;
		private readonly Option<DirectoryInfo> _outputDirectoryOption;

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

		protected override ReportCommandBaseOptions GetBoundValue(BindingContext bindingContext) =>
			new ReportCommandBaseOptions
			{
				SourceDirectory = bindingContext.ParseResult.GetValueForOption<DirectoryInfo>(_sourceDirectoryOption),
				RecurseDirectories = bindingContext.ParseResult.GetValueForOption<bool>(_recurseDirectoriesOption),
				FilePattern = bindingContext.ParseResult.GetValueForOption<string>(_filePatternOption),
				OutputType = bindingContext.ParseResult.GetValueForOption<ReportFileType>(_outputTypeOption),
				OutputDirectory = bindingContext.ParseResult.GetValueForOption<DirectoryInfo>(_outputDirectoryOption)
			};
	}
}