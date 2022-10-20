namespace TextTools.CommandHandlers
{
	using System.Text.RegularExpressions;
	using TextTools.Enums;

	public class MatchLineCommandHandler : ReportCommandHandlerBase
	{
		public MatchLineCommandHandler()
		: base()
		{
		}

		public MatchLineCommandHandler(ReportCommandBaseOptions coreOptions, string textRegex) : base(coreOptions)
		{
			TextRegex = new Regex(textRegex);
		}

		public Regex TextRegex { get; init; } = new Regex(string.Empty);

		protected override bool ValidateNonCoreOptions()
		{
			Console.WriteLine($"Text regex: {TextRegex.ToString()}");
			return true;
		}

		protected override void BuildReport()
		{
			throw new NotImplementedException();
		}
	}
}