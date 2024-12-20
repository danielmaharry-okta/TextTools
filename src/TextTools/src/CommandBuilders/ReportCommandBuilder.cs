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
            getDefaultValue: () => new FileInfo(@"c:\code\navbar.json"),
            description: "Navbar file as json"
         );

         var conductorOption = new Option<FileInfo>(
            name: "-c",
            getDefaultValue: () => new FileInfo(@"c:\code\conductor.json"),
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
            getDefaultValue: () => new FileInfo(@"c:\code\pagelist.csv"),
            description: "Page list csv file saved from airtable"
         );
         pagelistOption.AddAlias("--pl");

         var contentTypesOption = new Option<FileInfo>(
            name: "--contenttypes",
            getDefaultValue: () => new FileInfo(@"c:\code\contenttypes.csv"),
            description: "Content types csv file saved from airtable"
         );
         contentTypesOption.AddAlias("--ct");

         var showSupplementalContentOption = new Option<bool>(
            name: "--showSupplementalContent",
            getDefaultValue: () => false,
            description: "Show supplemental content in the build"
         );
         showSupplementalContentOption.AddAlias("--ssc");

         var numberOfLevelsOption = new Option<int>(
            name: "--numberOfLevels",
            getDefaultValue: () => 6,
            description: "Number of nav levels to include in the build, from 1 to 6"
         );
         numberOfLevelsOption.AddAlias("--lvl");

         var targetUrlRootOption = new Option<string>(
            name: "--targetUrlRoot",
            getDefaultValue: () => "https://danielmaharry-okta.github.io/iaproto",
            description: "The root URL of the live prototype"
         );
         targetUrlRootOption.AddAlias("--root");

         var mainContentStubOption = new Option<string>(
            name: "--mainContentStub",
            getDefaultValue: () => string.Empty,
            description: "Sets a root folder name for all main content. Leave blank if root folder not required."
         );
         mainContentStubOption.AddAlias("--mcs");

         var supplementalContentStubOption = new Option<string>(
            name: "--supplementalContentStub",
            getDefaultValue: () => string.Empty,
            description: "Sets a root folder name for all supplemental content. Leave blank if root folder not required."
         );
         supplementalContentStubOption.AddAlias("--scs");

         iaProtoCommand.AddOption(pagelistOption);
         iaProtoCommand.AddOption(contentTypesOption);
         iaProtoCommand.AddOption(showSupplementalContentOption);
         iaProtoCommand.AddOption(numberOfLevelsOption);
         iaProtoCommand.AddOption(targetUrlRootOption);
         iaProtoCommand.AddOption(mainContentStubOption);
         iaProtoCommand.AddOption(supplementalContentStubOption);
         iaProtoCommand.SetHandler(
            (reportCommandBaseOptions, pagelistOption, contentTypesOption, showSupplementalContentOption,
             numberOfLevelsOption, targetUrlRootOption, mainContentStubOption, supplementalContentStubOption) =>
            {
               IaProtoCommandHandler ipHandler = new IaProtoCommandHandler(
                  reportCommandBaseOptions, pagelistOption, contentTypesOption, showSupplementalContentOption,
                  numberOfLevelsOption, targetUrlRootOption, mainContentStubOption, supplementalContentStubOption);
               ipHandler.Go();
            }, coreOptions, pagelistOption, contentTypesOption, showSupplementalContentOption,
            numberOfLevelsOption, targetUrlRootOption, mainContentStubOption, supplementalContentStubOption
         );

         return iaProtoCommand;
      }

      internal Command BuildBetaSiteSubCommand()
      {
         var betaSiteCommand = new Command("betasite", "Builds a skeleton vuepress beta site matching the IA spreadsheet");
         var coreOptions = CreateAndAddCoreReportCommandOptions(betaSiteCommand, "*.csv");
         var pagelistOption = new Option<FileInfo>(
            name: "--pagelist",
            getDefaultValue: () => new FileInfo(@"c:\code\pagelist.csv"),
            description: "Page list csv file saved from airtable"
         );
         pagelistOption.AddAlias("--pl");

         var contentTypesOption = new Option<FileInfo>(
            name: "--contenttypes",
            getDefaultValue: () => new FileInfo(@"c:\code\contenttypes.csv"),
            description: "Content types csv file saved from airtable"
         );
         contentTypesOption.AddAlias("--ct");

         var showSupplementalContentOption = new Option<bool>(
            name: "--showSupplementalContent",
            getDefaultValue: () => false,
            description: "Show supplemental content in the build"
         );
         showSupplementalContentOption.AddAlias("--ssc");

         var numberOfLevelsOption = new Option<int>(
            name: "--numberOfLevels",
            getDefaultValue: () => 6,
            description: "Number of nav levels to include in the build, from 1 to 6"
         );
         numberOfLevelsOption.AddAlias("--lvl");

         betaSiteCommand.AddOption(pagelistOption);
         betaSiteCommand.AddOption(contentTypesOption);
         betaSiteCommand.AddOption(showSupplementalContentOption);
         betaSiteCommand.AddOption(numberOfLevelsOption);
         betaSiteCommand.SetHandler(
            (reportCommandBaseOptions, pagelistOption, contentTypesOption, showSupplementalContentOption, numberOfLevelsOption) =>
            {
               BetaSiteCommandHandler bsHandler = new BetaSiteCommandHandler(
                  reportCommandBaseOptions, pagelistOption, contentTypesOption, showSupplementalContentOption, numberOfLevelsOption);
               bsHandler.Go();
            }, coreOptions, pagelistOption, contentTypesOption, showSupplementalContentOption, numberOfLevelsOption
         );

         return betaSiteCommand;
      }
   }
}