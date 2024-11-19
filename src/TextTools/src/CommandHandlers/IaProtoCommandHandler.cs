namespace TextTools.CommandHandlers
{
   using System;
   using System.Globalization;
   using System.Text;
   using System.Text.RegularExpressions;
   using System.Web;
   using CsvHelper;
   using TextTools.Enums;

   /// <summary>
   /// Builds a set of markdown files matching the IA spreadsheet for compilation in hugo
   /// </summary>
   public class IaProtoCommandHandler : ReportCommandHandlerBase
   {
      /// <summary>
      /// Initializes a new instance fo the <see cref="IaProtoCommandHandler" /> class
      /// </summary>
      public IaProtoCommandHandler() : base()
      {
      }

      /// <summary>
      /// Initializes a new instance fo the <see cref="IaProtoCommandHandler" /> class
      /// </summary>
      /// <param name="coreOptions">The core report handler options</param>
      /// <param name="pageListFile">The page list spreadsheet as a csv file</param>
      /// <param name="contentTypesFile">The content types list as a csv file</param>
      /// <param name="showSupplementalContent">Whether to include supplemental content in the build</param>
      /// <param name="numberOfLevels">The number of nav levels to include in the build</param>
      /// <param name="targetRootUrl">The root URL of the live prototype site</param>
      /// <param name="mainContentStub">The root folder name for all main content</param>
      /// <param name="supplementalContentStub">The root folder name for all main content</param>
      public IaProtoCommandHandler(ReportCommandBaseOptions coreOptions, FileInfo pageListFile, FileInfo contentTypesFile, bool showSupplementalContent, 
         int numberOfLevels, string targetRootUrl, string mainContentStub, string supplementalContentStub) : base(coreOptions)
      {
         PageListFileInfo = pageListFile;
         ContentTypeFileInfo = contentTypesFile;
         ShowSupplementalContent = showSupplementalContent;
         NumberOfLevels = numberOfLevels;
         TargetRootUrl = targetRootUrl;
         MainContentStub = mainContentStub;
         SupplementalContentStub = supplementalContentStub;
      }

      /// <summary>
      /// Gets or sets the page list file
      /// </summary>
      public FileInfo PageListFileInfo { get; set; } = new FileInfo(@"c:\temp\pagelist.csv");

      /// <summary>
      /// Gets or sets the content types file
      /// </summary>
      public FileInfo ContentTypeFileInfo { get; set; } = new FileInfo(@"c:\temp\contenttypes.csv");

      /// <summary>
      ///  Gets or sets whether supplemental content should be included in the build
      /// </summary>
      public bool ShowSupplementalContent { get; set; } = false;

      /// <summary>
      /// Gets or sets the number of nav levels to include in the build
      /// </summary>
      public int NumberOfLevels { get; set; } = 6;

      /// <summary>
      /// Gets or sets the root URL of the live prototype site
      /// </summary>
      public string TargetRootUrl { get; set; } = "https://danielmaharry-okta.github.io/iaproto";

      /// <summary>
      /// Gets or sets the root folder name for all main content. Leave empty if one is not required.
      /// </summary>
      public string MainContentStub { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the root folder name for all supplemental content. Leave empty if one is not required.
      /// </summary>
      public string SupplementalContentStub { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the current version number of the site
      /// </summary>
      public string VersionNumber { get; init; } = "v3.0b1";

      /// <summary>
      /// Gets or sets a list of all defined content types.
      /// </summary>
      public List<ContentType> ContentTypes { get; set; } = [];

      /// <summary>
      /// Gets or sets a list of all the pages listed in the IA spreadsheet.
      /// </summary>
      public List<PageListEntry> Pages { get; set; } = [];

      private DirectoryInfo ContentDirectory { get; set; } = new DirectoryInfo(@"c:\temp");

      private readonly string FeedbackFormURL = "https://docs.google.com/forms/d/e/1FAIpQLSdwubXc5pUELg1L5B7qKjyS1_vnfB-2kALIAJOqwoUQjVUHBA/viewform?usp=pp_url&entry.276608427=ID&entry.395545573=PAGE_URL&entry.549860695=VERSION_NUMBER";

      /// <inheritdoc />
      protected override bool ValidateNonCoreOptions()
      {
         if (!PageListFileInfo.Exists)
         {
            SendToConsole($"{PageListFileInfo.FullName} does not exist", ConsoleColor.Red);
            return false;
         }

         if (!ContentTypeFileInfo.Exists)
         {
            SendToConsole($"{ContentTypeFileInfo.FullName} does not exist", ConsoleColor.Red);
            return false;
         }

         if (NumberOfLevels < 1 || NumberOfLevels > 6)
         {
            SendToConsole($"Please set number of levels to generate to value between 1 and 6", ConsoleColor.Red);
            return false;
         }

         SendToConsole($"Show secondary content: {ShowSupplementalContent}", ConsoleColor.Yellow);
         SendToConsole($"Number of levels: {NumberOfLevels}", ConsoleColor.Yellow);
         SendToConsole($"Root URL: {TargetRootUrl}", ConsoleColor.Yellow);
         SendToConsole($"Main Content Stub: {MainContentStub}", ConsoleColor.Yellow);
         SendToConsole($"Supplementary Content Stub: {SupplementalContentStub}", ConsoleColor.Yellow);

         return true;
      }

      /// <inheritdoc />
      protected override void BuildReport()
      {
         ReportFileName = "IAProtoReport";
         SendToConsole("Build IA Prototype Site", ConsoleColor.Green);

         InitializeListsFromCsvFiles();

         var ContentDirectory = CreateOutputContentDirectory();

         // Add header row
         Worksheet fileReport = new("protobuild");
         fileReport.Rows.Add(["Weight", "File", "Title"]);

         SendToConsole("Building Pages", ConsoleColor.Blue);
         BuildPageSection(ContentDirectory, fileReport, Pages.Where(p => p.ContentRole == "Home page"), string.Empty);
         BuildPageSection(ContentDirectory, fileReport, Pages.Where(p => p.ContentRole == "Main content"), MainContentStub);

         if (ShowSupplementalContent)
         {
            BuildPageSection(ContentDirectory, fileReport, Pages.Where(p => p.ContentRole == "Supportive content"), SupplementalContentStub);
         }

         Reports.Add(fileReport);
         SendToConsole("Site built", ConsoleColor.Green);
      }

      private void BuildPageSection(DirectoryInfo ContentDirectory, Worksheet fileReport, IEnumerable<PageListEntry> pages, string navRootDirectory)
      {
         foreach (var p in pages.OrderBy(p => p.Weight))
         {
            string pageText = GeneratePageContents(p, navRootDirectory);
            FileInfo pageFile = p.GetAbsolutePageFilePath(ContentDirectory, navRootDirectory);

            try
            {
                  if (!Directory.Exists(pageFile.DirectoryName))
                  {
                     string dirName = string.IsNullOrWhiteSpace(pageFile.DirectoryName) ? "temp" : pageFile.DirectoryName;
                     Directory.CreateDirectory(dirName);
                  }

               if (pageFile.Exists)
               {
                  pageFile.Delete();
               }

               using (StreamWriter sw = pageFile.CreateText())
               {
                  sw.WriteLine(pageText);
               }
            }
            catch (Exception ex)
            {
               SendToConsole(ex.ToString(), ConsoleColor.Red);
            }

            fileReport.Rows.Add([p.Weight.ToString(), pageFile.FullName, p.Title]);
         }
      }

      private void InitializeListsFromCsvFiles()
      {
         // Read in content type list
         SendToConsole("Reading Content Types", ConsoleColor.Yellow);
         using (var reader = new StreamReader(ContentTypeFileInfo.FullName))
         using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
         {
            ContentTypes = csv.GetRecords<ContentType>().ToList();
         }
         SendToConsole($"Found {ContentTypes.Count} content types", ConsoleColor.Green);

         // Read in page list
         SendToConsole("Reading Pages", ConsoleColor.Yellow);
         using (var reader = new StreamReader(PageListFileInfo.FullName))
         using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
         {
            var pageList = csv.GetRecords<PageListEntry>().ToList();
            foreach (var pl in pageList)
            {
               if (
                  (NumberOfLevels == 1 && pl.Level2.HasValue()) ||
                  (NumberOfLevels == 2 && pl.Level3.HasValue()) ||
                  (NumberOfLevels == 3 && pl.Level4.HasValue()) ||
                  (NumberOfLevels == 4 && pl.Level5.HasValue()) ||
                  (NumberOfLevels == 5 && pl.Level6.HasValue())
               )
               {
                  continue;
               }

               pl.Weight = Pages.Count + (pl.ContentRole == "Supportive content" ? 1000 : 0);
               Pages.Add(pl);
            }
         }

         if (MainContentStub.HasValue())
         {
            Pages.Add(MainContentHomepage());
         }

         if (SupplementalContentStub.HasValue())
         {
            Pages.Add(SupportiveContentHomepage());
         }

         SendToConsole($"Found {Pages.Count} pages", ConsoleColor.Green);
      }

      private PageListEntry MainContentHomepage()
      {
         return new PageListEntry
         {
            ContentRole = "Main content",
            Title = MainContentStub,
            Weight = 1,
            DocDescription = "This area contains the main content of the site typically found in the main navigation of the DevDocs site.",
            InPhase1 = "yes",
            ContentType = "Not applicable"
         };
      }

      private PageListEntry SupportiveContentHomepage()
      {
         return new PageListEntry
         {
            ContentRole = "Supportive content",
            Title = SupplementalContentStub,
            Weight = 1000,
            DocDescription = "This area contains secondary content typically not found in the main navigation of the DevDocs site.",
            InPhase1 = "yes",
            ContentType = "Not applicable"
         };
      }

      private DirectoryInfo CreateOutputContentDirectory()
      {
         var contentDirectoryPath = Path.Combine(OutputDirectory.FullName, "Content");
         if (Directory.Exists(contentDirectoryPath))
         {
            Directory.Delete(contentDirectoryPath, true);
         }

         return Directory.CreateDirectory(contentDirectoryPath);
      }

      private string GeneratePageContents(PageListEntry p, string navRootDirectory)
      {
         StringBuilder contents = new();

         // header section
         contents.AppendLine("+++");
         contents.AppendLine($"title = '{p.GetNavTitle().Replace("'", "’")}'");
         contents.AppendLine($"date = {DateTime.Now.ToLongTimeString()}");
         contents.AppendLine($"weight = {p.Weight}");
         contents.AppendLine($"alwaysopen = false");
         contents.AppendLine("+++");
         contents.AppendLine();
         contents.AppendLine($"## {(p.ContentRole == "Home page" ? "developer.okta.com Navigation prototype " + VersionNumber : p.Title)}");
         contents.AppendLine();

         contents.AppendLine($"[Click here to give feedback about this page]({GenerateFeedbackURL(p.GetAbsolutePageFilePath(ContentDirectory, navRootDirectory).FullName, p.Id)})");
         contents.AppendLine();

         if (p.ContentRole == "Home page")
         {
            AddHomePageIntro(contents);
            return contents.ToString();
         }

         // AddIfItHasAValue(contents, "In Phase 1?", p.InPhase1);
         // AddIfItHasAValue(contents, "In Phase 2?", p.InPhase2);
         // AddIfItHasAValue(contents, "Related to help docs?", p.HelpDocsScope);
         contents.AppendLine();

         if (p.GroupDescription.HasValue())
         {
            contents.AppendLine("### Section Notes").AppendLine();
            contents.AppendLine(p.GroupDescription);
         }

         contents.AppendLine();
         contents.AppendLine("### About This Page").AppendLine();
         AddIfItHasAValue(contents, "Page ID", p.Id);
         AddIfItHasAValue(contents, "Is this a new page?", p.DocumentType);
         AddIfItHasAValue(contents, "Target personas", p.TargetPersonas.Replace(",", ", "));
         // AddIfItHasAValue(contents, "Dimensioned by", p.Dimensions, "No doc dimension");
         AddIfItHasAValue(contents, "Description", p.DocDescription);
         AddIfItHasAValue(contents, "Changes to be made", p.SuggestedChanges);
         AddIfItHasAValue(contents, "Links to original docs", p.ExistingLinks, string.Empty, true);
         AddIfItHasAValue(contents, "Why is this here?", p.Validation, string.Empty, true);
         AddIfItHasAValue(contents, "Page type", p.StructureType);
         contents.AppendLine($"**Content type:** {GetContentTypeLink(p.ContentType)}");
         contents.AppendLine();
         contents.AppendLine(GetContentTypeDescription(p.ContentType));

         return FindAndLinkInternalIDs(contents);
      }

      private string GenerateFeedbackURL(string absoluteFilePath, string pageID)
      {
         string absoluteUrl = absoluteFilePath.Replace(ContentDirectory.FullName, TargetRootUrl).Replace("\\", "/").Replace("_index.md", string.Empty);
         return FeedbackFormURL.Replace("PAGE_URL", HttpUtility.HtmlEncode(absoluteUrl).ToLowerInvariant()).Replace("VERSION_NUMBER", VersionNumber).Replace("ID", pageID);
      }

      // Finds any instance of the string ID: xxx and adds a link to the page with that internal ID number.
      private string FindAndLinkInternalIDs(StringBuilder contents)
      {
         string uncheckedText = contents.ToString();
         Regex IDstrings = new(@"ID\W?:\W?(\d+)");
         var matches = IDstrings.Matches(uncheckedText);

         foreach (Match match in matches)
         {
            string idString = match.Groups[0].Value;
            string pageID = match.Groups[1].Value;

            var targetPages = Pages.Where(p => p.Id == pageID);
            if (targetPages.Any())
            {
               var target = targetPages.First();
               string navRootDirectory = target.ContentRole == "Main content" ? MainContentStub : SupplementalContentStub;
               uncheckedText = uncheckedText.Replace(idString, $"[{idString}](/{target.GetRelativeFilePath(navRootDirectory).Replace("\\", "/")})");
            }
         }

         return uncheckedText;
      }

      private string GetContentTypeLink(string contentType)
      {
         var cts = ContentTypes.Where(ct => ct.Name == contentType);
         if (cts.None())
         {
            return "Unknown";
         }

         return $"[{cts.First().Name}]({cts.First().ConfluenceUrl})";
      }

      private string GetContentTypeDescription(string contentType)
      {
         var cts = ContentTypes.Where(ct => ct.Name == contentType);
         if (cts.None())
         {
            return "Unknown";
         }

         return cts.First().Description.Trim();
      }

      private void AddIfItHasAValue(StringBuilder contents, string name, string value, string nameToIgnore, bool contentOnNewLine)
      {
         if (string.IsNullOrWhiteSpace(value) || value.Trim() == nameToIgnore.Trim())
         {
            return;
         }

         string undashedContent = value.EndsWith("- ") ? value.Remove(value.Length - 2) : value;
         string unquotedContent = undashedContent.StartsWith("'") ? undashedContent.Substring(1) : undashedContent;

         contents.AppendLine($"**{name}**: ");

         if (contentOnNewLine)
         {
            contents.AppendLine();
         }

         contents.AppendLine($"{unquotedContent.Trim()}").AppendLine();
         return;
      }

      private void AddIfItHasAValue(StringBuilder contents, string name, string value)
      {
         AddIfItHasAValue(contents, name, value, string.Empty, false);
      }

      private void AddHomePageIntro(StringBuilder contents)
      {
         contents.AppendLine("**Please give us your feedback** on the redesigned navigation system for the developer documentation website.");
         contents.AppendLine();
         contents.AppendLine("The goal is a task-based navigation that makes it easier for developers to find the content they're looking for… the answer to their question. We've biased the design to developers inexperienced with Identity and Access Management.");
         contents.AppendLine();
        contents.AppendLine("**Please do NOT give us your feedback** on the design and visuals of this prototype or its lack of actual documentation. A new design is being worked on elsewhere and reworked content will appear in due course.");
         contents.AppendLine();
         contents.AppendLine("To leave feedback, click the large feedback link at the top of each page. On the Google form that appears, please share feedback on:");
         contents.AppendLine("* Navigation menu titles");
         contents.AppendLine("* The usefulness of the titles to finding your answer");
         contents.AppendLine("* Groupings of concepts at levels 1 and 2");
         contents.AppendLine("* Anything else that seems relevant");
         contents.AppendLine();
         contents.AppendLine("We'll randomly pick four people who gave feedback and award them 25 Oktappreciate points at the end of the feedback cycle on September 20!");
         contents.AppendLine();
         contents.AppendLine("Consider setting yourself a coding or conceptual question and then trying to find the answer by clicking through the navigation menu. As you click through the menu, the content on the right shows information about the menu item: who the page is for (Target personas), links to the current pages it maps to, and other information.");
         contents.AppendLine();
         contents.AppendLine("For example, can you find the page to answer these questions:");
         contents.AppendLine("* What are OAuth and OIDC in identity and access management?");
         contents.AppendLine("* What is the Okta Integration Network (OIN)?");
         contents.AppendLine("* How do I add Google Authenticator as an authentication factor to my web app?");
         contents.AppendLine("* How do I customize the sign-in widget?");
         contents.AppendLine("* How do I get started with the Okta Terraform provider?");
         contents.AppendLine("* How do I submit my app to the OIN?");
         contents.AppendLine("* How do I find a list of the endpoints for an API?");
         return;
      }
   }
}