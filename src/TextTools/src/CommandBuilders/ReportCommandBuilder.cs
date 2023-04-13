namespace TextTools.CommandBuilders
{
   using System;
   using System.CommandLine;
   using TextTools.CommandHandlers;

   class ReportCommandBuilder : CommandBuilderBase
   {
      public ReportCommandBuilder()
      {
      }

      public Command BuildMatchListSubCommand()
      {
         var matchListCommand = new Command("matchlist", "Builds a report of all matches to a regex in all files in a directory");
         var coreOptions = CreateAndAddCoreReportCommandOptions(matchListCommand);
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

      internal Command BuildIAAuditSubCommand()
      {
         var iaAuditCommand = new Command("iaaudit", "Builds a list of docs in dev.okta");
         var coreOptions = CreateAndAddCoreReportCommandOptions(iaAuditCommand, "*.md");
         var navbarOption = new Option<FileInfo>(
            name: "-n",
            getDefaultValue: () => new FileInfo(@"c:\temp\navbar.json"),
            description: "Navbar file as json"
         );

         var conductorOption = new Option<FileInfo>(
            name: "-c",
            getDefaultValue: () => new FileInfo(@"c:\temp\conductor.json"),
            description: "Conductor file as json"
         );

         iaAuditCommand.AddOption(navbarOption);
         iaAuditCommand.AddOption(conductorOption);
         iaAuditCommand.SetHandler(
            (reportCommandBaseOptions, navbarOption, conductorOption) =>
            {
               IaAuditCommandHandler iaHandler = new IaAuditCommandHandler(reportCommandBaseOptions, navbarOption, conductorOption);
               iaHandler.Go();
            }, coreOptions, navbarOption, conductorOption
         );

         return iaAuditCommand;
      }
   }
}