namespace TextTools.CommandHandlers
{
   using System;
   using System.Globalization;
   using System.Text;
   using System.Text.RegularExpressions;
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
      /// <param name="mainContentOnly">Whether to include only main content in the build</param>
      /// <param name="numberOfLevels">The number of nav levels to include in the build</param>
      public IaProtoCommandHandler(ReportCommandBaseOptions coreOptions, FileInfo pageListFile, FileInfo contentTypesFile, bool mainContentOnly, int numberOfLevels) : base(coreOptions)
      {
         PageListFileInfo = pageListFile;
         ContentTypeFileInfo = contentTypesFile;
         MainContentOnly = mainContentOnly;
         NumberOfLevels = numberOfLevels;
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
      ///  Gets or sets whether only main content should be included in the build
      /// </summary>
      public bool MainContentOnly { get; set; } = false;

      /// <summary>
      /// Gets or sets the number of nav levels to include in the build
      /// </summary>
      public int NumberOfLevels { get; set; } = 6;

      /// <summary>
      /// Gets or sets a list of all defined content types.
      /// </summary>
      public List<ContentType> ContentTypes { get; set; } = [];

      /// <summary>
      /// Gets or sets a list of all the pages listed in the IA spreadsheet.
      /// </summary>
      public List<PageListEntry> Pages { get; set; } = [];

      private DirectoryInfo ContentDirectory { get; set; } = new DirectoryInfo(@"c:\temp");

      private bool StubMainContent = true;

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

         SendToConsole($"Exclude secondary content: {MainContentOnly}", ConsoleColor.Yellow);

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
         BuildPageSection(ContentDirectory, fileReport, Pages.Where(p => p.ContentRole == "Home page"), false);
         BuildPageSection(ContentDirectory, fileReport, Pages.Where(p => p.ContentRole == "Main content"), StubMainContent);

         if (!MainContentOnly)
         {
            BuildPageSection(ContentDirectory, fileReport, Pages.Where(p => p.ContentRole == "Supportive content"), true);
         }

         Reports.Add(fileReport);
         SendToConsole("Site built", ConsoleColor.Green);
      }

      private void BuildPageSection(DirectoryInfo ContentDirectory, Worksheet fileReport, IEnumerable<PageListEntry> pages, bool hideUnderContentRoleStub)
      {
         foreach (var p in pages.OrderBy(p => p.Weight))
         {
            string pageText = GeneratePageContents(p);
            FileInfo pageFile = p.GetAbsolutePageFilePath(ContentDirectory, hideUnderContentRoleStub);

            try
            {
               if (!Directory.Exists(pageFile.DirectoryName))
               {
                  Directory.CreateDirectory(pageFile.DirectoryName);
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

         if (StubMainContent)
         {
            Pages.Add(MainContentStub());
         }
         Pages.Add(SupportiveContentStub());
         SendToConsole($"Found {Pages.Count} pages", ConsoleColor.Green);
      }

      private PageListEntry MainContentStub()
      {
         return new PageListEntry
         {
            ContentRole = "Main content",
            Level1 = "Main content",
            Title = "Main Content",
            Weight = 1,
            DocDescription = "This area contains the main content of the site typically found in the main navigation of the DevDocs site.",
            InPhase1 = "yes",
            IsStub = true
         };
      }

      private PageListEntry SupportiveContentStub()
      {
         return new PageListEntry
         {
            ContentRole = "Supportive content",
            Level1 = "Supportive content",
            Title = "Supportive Content",
            Weight = 1000,
            DocDescription = "This area contains secondary content typically not found in the main navigation of the DevDocs site.",
            InPhase1 = "yes",
            IsStub = true
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

      private string GeneratePageContents(PageListEntry p)
      {
         StringBuilder contents = new();

         // header section
         contents.AppendLine("+++");
         contents.AppendLine($"title = '{p.GetNavTitle().Replace("'", string.Empty)}'");
         contents.AppendLine($"date = {DateTime.Now.ToLongTimeString()}");
         contents.AppendLine($"weight = {p.Weight}");
         contents.AppendLine($"alwaysopen = false");
         contents.AppendLine("+++");
         contents.AppendLine();
         contents.AppendLine($"## {p.Title}");
         contents.AppendLine();

         if (p.ContentRole == "Home page")
         {
            AddHomePageIntro(contents);
            return contents.ToString();
         }

         AddIfItHasAValue(contents, "In Phase 1?", p.InPhase1);
         AddIfItHasAValue(contents, "In Phase 2?", p.InPhase2);
         AddIfItHasAValue(contents, "Related to help docs?", p.HelpDocsScope);
         contents.AppendLine();

         if (p.GroupDescription.HasValue())
         {
            contents.AppendLine("### Group Notes").AppendLine();
            contents.AppendLine($"**Group description**: {p.GroupDescription}");
         }

         contents.AppendLine();
         contents.AppendLine("### Content Notes").AppendLine();
         AddIfItHasAValue(contents, "New or existing doc?", p.DocumentType);
         AddIfItHasAValue(contents, "Target personas", p.TargetPersonas);
         AddIfItHasAValue(contents, "Dimensioned by", p.Dimensions, "No doc dimension");
         AddIfItHasAValue(contents, "Description", p.DocDescription);
         AddIfItHasAValue(contents, "Changes to be made", p.SuggestedChanges);
         AddIfItHasAValue(contents, "Links to original docs", p.ExistingLinks);
         AddIfItHasAValue(contents, "Validation", p.Validation);
         contents.AppendLine($"**Content type: {p.ContentType.Trim()}**").AppendLine();
         contents.AppendLine(GetContentTypeDescription(p.ContentType));
         contents.AppendLine();

         return FindAndLinkInternalIDs(contents);
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
               uncheckedText = uncheckedText.Replace(idString, $"[{idString}](/{targetPages.First().GetRelativeFilePath(StubMainContent).Replace("\\", "/")})");
            }
         }

         return uncheckedText;
      }

      private string? GetContentTypeDescription(string contentType)
      {
         var cts = ContentTypes.Where(ct => ct.Name == contentType);
         if (cts.Any())
         {
            return cts.First().Description;
         }

         return "Unknown";
      }

      private void AddIfItHasAValue(StringBuilder contents, string name, string value, string nameToIgnore)
      {
         if (string.IsNullOrWhiteSpace(value) || value.Trim() == nameToIgnore.Trim())
         {
            return;
         }

         string undashedContent = value.EndsWith("- ") ? value.Remove(value.Length - 2) : value;

         contents.AppendLine($"**{name}**: {undashedContent.Trim()}").AppendLine();
         return;
      }

      private void AddIfItHasAValue(StringBuilder contents, string name, string value)
      {
         AddIfItHasAValue(contents, name, value, string.Empty);
      }

      private void AddHomePageIntro(StringBuilder contents)
      {
         contents.AppendLine("Welcome to our Developer Documentation Information Architecture (IA) Prototype! We're on a mission to elevate your experience by unveiling a redesigned structure that makes finding and utilizing our resources more intuitive.");
         contents.AppendLine();
         contents.AppendLine("This prototype website serves as a visual guide to our proposed new IA groupings, showcasing how documents are categorized and how you can navigate through them. As you explore, you'll encounter placeholders for both existing and newly proposed documentation, complete with details on content type, document titles, user research validation, and more. While the actual documents are not hosted here, links to the existing content on our current developer site are provided for your reference.");
         contents.AppendLine();
         contents.AppendLine("Here's what we need from you:");
         contents.AppendLine("* Dive into the navigation menu to explore the proposed groupings and item placements.");
         contents.AppendLine("* Reflect on the clarity and intuitiveness of these new groupings and the ease of navigation. Consider how the new structure aligns with your expectations and needs.");
         contents.AppendLine("* Share your invaluable feedback using the [IA Prototype v2 feedback spreadsheet](https://docs.google.com/spreadsheets/d/1wsnbNcJdPsxHszTYzDEPYj4ZUEyJs_YGtRgUdsSruZU/edit#gid=375236245). Your insights on the new IA, document placements, and any additional attributes are crucial for refining our documentation site.");
         contents.AppendLine();
         contents.AppendLine("Your feedback is pivotal in crafting a documentation site that is informative, easily navigable, and intuitive. We thank you in advance for your time and thoughtful contributions.");
         return;
      }
   }
}