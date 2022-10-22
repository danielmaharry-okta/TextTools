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
			return string.IsNullOrWhiteSpace(TextRegex.ToString());
		}

		/// <inheritdoc />
		protected override void BuildReport()
		{
			Console.WriteLine("MatchLine runs");
		}
	}
}