namespace TextTools.CommandBuilders
{
   using System;
   using System.CommandLine;
   using System.Reflection.Metadata.Ecma335;
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

      internal Command BuildIAProtoSubCommand()
      {
         var iaProtoCommand = new Command("iaproto", "Builds a set of markdown files matching the IA spreadsheet for compilation in hugo");
         var coreOptions = CreateAndAddCoreReportCommandOptions(iaProtoCommand, "*.csv");
         var pagelistOption = new Option<FileInfo>(
            name: "--pagelist",
            getDefaultValue: () => new FileInfo(@"c:\temp\pagelist.csv"),
            description: "Page list csv file saved from airtable"
         );
         pagelistOption.AddAlias("--pl");

         var contentTypesOption = new Option<FileInfo>(
            name: "--contenttypes",
            getDefaultValue: () => new FileInfo(@"c:\temp\contenttypes.csv"),
            description: "Content types csv file saved from airtable"
         );
         contentTypesOption.AddAlias("--ct");

         var mainContentOnlyOption = new Option<bool>(
            name: "--mainContentOnly",
            getDefaultValue: () => false,
            description: "Show main content only in the build"
         );
         mainContentOnlyOption.AddAlias("--mco");

         var numberOfLevelsOption = new Option<int>(
            name: "--numberOfLevels",
            getDefaultValue: () => 6,
            description: "Number of nav levels to include in the build, from 1 to 6"
         );
         numberOfLevelsOption.AddAlias("--lvl");

         iaProtoCommand.AddOption(pagelistOption);
         iaProtoCommand.AddOption(contentTypesOption);
         iaProtoCommand.AddOption(mainContentOnlyOption);
         iaProtoCommand.AddOption(numberOfLevelsOption);
         iaProtoCommand.SetHandler(
            (reportCommandBaseOptions, pagelistOption, contentTypesOption, mainContentOnlyOption, numberOfLevelsOption) =>
            {
               IaProtoCommandHandler ipHandler = new IaProtoCommandHandler(reportCommandBaseOptions, pagelistOption, contentTypesOption, mainContentOnlyOption, numberOfLevelsOption);
               ipHandler.Go();
            }, coreOptions, pagelistOption, contentTypesOption, mainContentOnlyOption, numberOfLevelsOption
         );

         return iaProtoCommand;
      }
   }
}