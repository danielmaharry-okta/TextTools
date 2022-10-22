namespace TextTools.CommandHandlers
{
	using System.Text.RegularExpressions;
	using TextTools.Enums;

	/// <summary>
	/// Creates a report of which files in a directory  contain the matches for a pattern.
	/// </summary>
	public class MatchLineCommandHandler : ReportCommandHandlerBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MatchLineCommandHandler" /> class
		/// </summary>
		public MatchLineCommandHandler()
		: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MatchLineCommandHandler" /> class
		/// </summary>
		/// <param name="coreOptions">The core report handler options</param>
		/// <param name="textRegex">The text regex to find in the files</param>
		public MatchLineCommandHandler(ReportCommandBaseOptions coreOptions, string textRegex) : base(coreOptions)
		{
			TextRegex = new Regex(textRegex);
		}

		/// <summary>
		/// Returns a text regular expression searched for in the files sent forward
		/// </summary>
		public Regex TextRegex { get; init; } = new Regex(string.Empty);

		/// <inheritdoc />
		protected override bool ValidateNonCoreOptions()
		{
			Console.WriteLine($"Text regex: {TextRegex.ToString()}");
			return !string.IsNullOrWhiteSpace(TextRegex.ToString());
		}

		/// <inheritdoc />
		protected override void BuildReport()
		{
			ReportFileName = "MatchList";
			SendToConsole("Running MatchList Report", ConsoleColor.Red);

			Worksheet matchlist = new Worksheet("matchlist");
			matchlist.Rows.Add(new List<string> { "file", "match" });

			foreach (var sourceFile in SourceDirectory.EnumerateFiles(FilePattern, RecurseDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
			{
				SendToConsole($"Pulling data from {sourceFile.Name}", ConsoleColor.Red);
				ExtractData(matchlist, sourceFile);
			}

			Reports.Add(matchlist);

			Worksheet counts = new Worksheet("matchcount");
			counts.Rows.Add(new List<string> { "match", "count" });
			// var groups = matchlist.Rows.GroupBy(row => row.Last()).Select(x => new
			// {
			// 	url = x.Key,
			// 	count = x.Count()
			// });

			counts.Rows.AddRange(matchlist.Rows.GroupBy(row => row.Last())
				.Select(x => new List<string> { x.Key, x.Count().ToString() }));

			Reports.Add(counts);
		}

		private void ExtractData(Worksheet ws, FileInfo sourceFile)
		{
			foreach (var line in sourceFile.AsStringList())
			{
				foreach (Match match in TextRegex.Matches(line))
				{
					ws.Rows.Add(new List<string> { sourceFile.FullName, match.Value });
				}
			}
		}
	}
}