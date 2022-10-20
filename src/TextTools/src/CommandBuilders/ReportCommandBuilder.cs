namespace TextTools.CommandBuilders
{
	using System.CommandLine;
	using TextTools.CommandHandlers;
	using TextTools.Enums;

	class ReportCommandBuilder : CommandBuilderBase
	{
		public ReportCommandBuilder()
		{
		}

		public Command BuildMatchListSubCommand()
		{
			var matchListCommand = new Command("matchlist", "Builds a report of all matches to a regex in all files in a directory");
         var coreOptions = CreateAndAddCoreReportCommandOptions(matchListCommand, "*.txt");
			var regexOption = new Option<string>(
					name: "--regex",
					description: "The regex to search for in the files"
				)
			{ IsRequired = true };

			matchListCommand.AddOption(regexOption);

			matchListCommand.SetHandler(
				(reportCommandBaseOptions, regex) =>
				{
					MatchLineCommandHandler mlHandler = new MatchLineCommandHandler(reportCommandBaseOptions, regex);
					mlHandler.Go();
				}, coreOptions, regexOption
			);

			return matchListCommand;
		}
	}
}