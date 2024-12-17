namespace TextTools.CommandHandlers
{
   using System;
   using System.Globalization;
   using System.Text;
   using System.Text.Json;
   using System.Text.Json.Serialization.Metadata;
   using System.Text.RegularExpressions;
   using System.Web;
   using CsvHelper;
   using TextTools.Enums;

   /// <summary>
   /// Builds a set of markdown files matching the IA spreadsheet for compilation in hugo
   /// </summary>
   public class BetaSiteCommandHandler : ReportCommandHandlerBase
   {
      /// <summary>
      /// Initializes a new instance fo the <see cref="BetaSiteCommandHandler" /> class
      /// </summary>
      public BetaSiteCommandHandler() : base()
      {
      }

      /// <summary>
      /// Initializes a new instance fo the <see cref="BetaSiteCommandHandler" /> class
      /// </summary>
      /// <param name="coreOptions">The core report handler options</param>
      /// <param name="pageListFile">The page list spreadsheet as a csv file</param>
      /// <param name="contentTypesFile">The content types list as a csv file</param>
      /// <param name="showSupplementalContent">Whether to include supplemental content in the build</param>
      /// <param name="numberOfLevels">The number of nav levels to include in the build</param>
      public BetaSiteCommandHandler(ReportCommandBaseOptions coreOptions, FileInfo pageListFile, FileInfo contentTypesFile, bool showSupplementalContent,
         int numberOfLevels) : base(coreOptions)
      {
         PageListFileInfo = pageListFile;
         ContentTypeFileInfo = contentTypesFile;
         ShowSupplementalContent = showSupplementalContent;
         NumberOfLevels = numberOfLevels;
      }

      /// <summary>
      /// Gets or sets the page list file
      /// </summary>
      public FileInfo PageListFileInfo { get; set; } = new FileInfo(@"c:\code\pagelist.csv");

      /// <summary>
      /// Gets or sets the content types file
      /// </summary>
      public FileInfo ContentTypeFileInfo { get; set; } = new FileInfo(@"c:\code\contenttypes.csv");

      /// <summary>
      ///  Gets or sets whether supplemental content should be included in the build
      /// </summary>
      public bool ShowSupplementalContent { get; set; } = false;

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

      private DirectoryInfo ContentDirectory { get; set; } = new DirectoryInfo(@"c:\code");

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
         return true;
      }

      /// <inheritdoc />
      protected override void BuildReport()
      {
         ReportFileName = "BetaSiteReport";
         SendToConsole("Build Beta Site Pages", ConsoleColor.Green);

         InitializeListsFromCsvFiles();

         var ContentDirectory = CreateOutputContentDirectory();

         // Add header row
         Worksheet fileReport = new("betabuild");
         fileReport.Rows.Add(["Weight", "File", "Title"]);

         SendToConsole("Building Pages", ConsoleColor.Blue);
         BuildPageSection(ContentDirectory, fileReport, Pages.Where(p => p.ContentRole == "Home page"), string.Empty);
         BuildPageSection(ContentDirectory, fileReport, Pages.Where(p => p.ContentRole == "Main content"), string.Empty);
         BuildNavbarSection(ContentDirectory, fileReport, Pages.Where(p => p.ContentRole == "Main content"));

         Reports.Add(fileReport);
         SendToConsole("Site built", ConsoleColor.Green);
      }

      private void BuildNavbarSection(DirectoryInfo ContentDirectory, Worksheet fileReport, IEnumerable<PageListEntry> pages)
      {
         var rootEntries = new List<NavbarEntry>();
         foreach (var p in pages.OrderBy(p => p.DocOrder))
         {
            // if doc is level2 or deeper, path is level1 parent + navtitle
            string relativePath = string.Empty;
            if (p.Level2.Trim().HasValue())
            {
               relativePath = $"/{p.Level1.Trim().AsSafeFileName()}/{p.GetNavTitle().AsSafeFileName()}/";
            }
            else
            {
               relativePath = $"/{p.Level1.Trim().AsSafeFileName()}/";
            }

            // if doc is level1, create new navbarsection
            if (string.IsNullOrWhiteSpace(p.Level2))
            {
               rootEntries.Add(
                  new NavbarEntry { Title = p.GetNavTitle(), Path = relativePath.Replace("(", string.Empty).Replace(")", string.Empty) }
               );

               continue;
            }

            // if doc is level2 or deeper, find parent node and add sublink
            NavbarEntry parentNode = FindParentNode(p, rootEntries);
            parentNode.SubLinks.Add(
               new NavbarEntry { Title = p.GetNavTitle(), Path = relativePath.Replace("(", string.Empty).Replace(")", string.Empty) }
            );
         }

         StringBuilder jsonAsText = new StringBuilder();
         var options = new JsonSerializerOptions
         {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
               Modifiers = { SerializeAsNavbarConst }
            }
         };

         foreach (var rootEntry in rootEntries)
         {
            jsonAsText.Append($"export const {rootEntry.Title.ToLowerInvariant()} = [ {JsonSerializer.Serialize(rootEntry, options)} ];");
            jsonAsText.AppendLine().AppendLine();
         }

         File.WriteAllText(Path.Combine(ContentDirectory.FullName, "navbar.const.js"), jsonAsText.ToString());
         fileReport.Rows.Add(["0", "navbar.const.js", "n/a"]);
         SendToConsole("navbar file built", ConsoleColor.Green);
      }

      static void SerializeAsNavbarConst(JsonTypeInfo typeInfo)
      {
         if (typeInfo.Type != typeof(NavbarEntry))
            return;

         foreach (JsonPropertyInfo propertyInfo in typeInfo.Properties)
         {
            if (propertyInfo.PropertyType == typeof(string))
            {
               propertyInfo.ShouldSerialize = static (obj, value) =>
               {
                  if (value is null)
                  {
                     return false;
                  }

                  return !string.IsNullOrWhiteSpace(value.ToString());
               };
            }

            if (propertyInfo.PropertyType == typeof(List<NavbarEntry>))
            {
               propertyInfo.ShouldSerialize = static (obj, value) =>
               {
                  if (value is null)
                  {
                     return false;
                  }

                  return ((List<NavbarEntry>)value).Count > 0;
               };
            }
         }
      }

      private NavbarEntry FindParentNode(PageListEntry p, List<NavbarEntry> rootNodes)
      {
         var l1Node = rootNodes.First(rn => rn.Title == p.Level1);

         // if level 2 node, return l1 parent
         if (string.IsNullOrWhiteSpace(p.Level3))
         {
            return l1Node;
         }

         var l2Node = l1Node.SubLinks.First(cn => cn.Title == p.Level2);

         // if level 3 node, return l2 parent
         if (string.IsNullOrWhiteSpace(p.Level4))
         {
            return l2Node;
         }

         var l3Node = l2Node.SubLinks.First(cn => cn.Title == p.Level3);

         // if level 4 node, return l3 parent
         if (string.IsNullOrWhiteSpace(p.Level5))
         {
            return l3Node;
         }

         var l4Node = l3Node.SubLinks.First(cn => cn.Title == p.Level4);

         // if level 5 node, return l4 parent
         if (string.IsNullOrWhiteSpace(p.Level6))
         {
            return l4Node;
         }

         // is level 6 node, so return l5 parent
         return l4Node.SubLinks.First(cn => cn.Title == p.Level5);
      }

      private void BuildPageSection(DirectoryInfo ContentDirectory, Worksheet fileReport, IEnumerable<PageListEntry> pages, string navRootDirectory)
      {
         foreach (var p in pages.OrderBy(p => p.DocOrder))
         {
            string pageText = GeneratePageContents(p, navRootDirectory);
            FileInfo pageFile = GetFileLocation(p, ContentDirectory);

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

            fileReport.Rows.Add([p.DocOrder.ToString(), pageFile.FullName, p.Title]);
         }
      }

      private FileInfo GetFileLocation(PageListEntry p, DirectoryInfo baseDirectory)
      {
         string relativePath = Path.Combine(
            p.Level2.Trim().HasValue() ? p.Level1.Trim() : string.Empty,
            p.GetNavTitle(),
            "index.md"
         ).AsSafeFileName().Replace("(", string.Empty).Replace(")", string.Empty);
         return new FileInfo(Path.Combine(baseDirectory.FullName, relativePath));
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

               Pages.Add(pl);
            }
         }

         SendToConsole($"Found {Pages.Count} pages", ConsoleColor.Green);
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
         contents.AppendLine("---");
         contents.AppendLine($"title: {p.GetNavTitle()}");
         contents.AppendLine("---");
         contents.AppendLine();

         if (p.ContentRole == "Home page")
         {
            AddHomePageIntro(contents);
            return contents.ToString();
         }

         contents.AppendLine();

         if (p.GroupDescription.HasValue())
         {
            contents.AppendLine("### Section Notes").AppendLine();
            contents.AppendLine(p.GroupDescription);
         }

         contents.AppendLine();
         contents.AppendLine("### About This Page").AppendLine();
         AddIfItHasAValue(contents, "Description", p.DocDescription);
         AddIfItHasAValue(contents, "Links to original docs", p.ExistingLinks, string.Empty, true);
         AddIfItHasAValue(contents, "Why is this here?", p.Validation, string.Empty, true);
         AddIfItHasAValue(contents, "Page type", p.StructureType);
         AddIfItHasAValue(contents, "Content type", p.ContentType);
         contents.AppendLine();
         AddIfItHasAValue(contents, "Article Structure", GetContentTypeStructure(p.ContentType));
         return contents.ToString();
      }

      private string GetContentTypeStructure(string contentType)
      {
         var cts = ContentTypes.Where(ct => ct.Name == contentType);
         if (cts.None())
         {
            return "Unknown";
         }

         return cts.First().Structure.Trim();
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
         contents.AppendLine("## Start your Workforce Identity journey");
         contents.AppendLine("Welcome! Start with Learn if you're new to Workforce Identity Cloud, or find the step in your journey and follow the links to browse docs.");
         contents.AppendLine();
         contents.AppendLine("## Learn");
         contents.AppendLine("### Understand the basics of identity");
         contents.AppendLine("Learn the key concepts you need for creating identity and access management (IAM) solutions for WIC.");
         contents.AppendLine();
         contents.AppendLine("* Understand IAM");
         contents.AppendLine("* How WIC works");
         contents.AppendLine("* Choose an authentication protocol");
         contents.AppendLine("* Get a developer org");
         contents.AppendLine();
         contents.AppendLine("## Build");
         contents.AppendLine("### Connect with APIs and SDKs");
         contents.AppendLine("* Build apps and services that interact directly with WIC for a completely integrated experience.");
         contents.AppendLine();
         contents.AppendLine("* Explore reference APIs");
         contents.AppendLine("* Explore our SDKs");
         contents.AppendLine("* Explore embedded authentication use cases");
         contents.AppendLine();
         contents.AppendLine("## Authenticate");
         contents.AppendLine("### Define how your applications and APIs verify the identity of a user or device.");
         contents.AppendLine();
         contents.AppendLine("* Start with redirect authentication");
         contents.AppendLine("* Set up multifactor authentication");
         contents.AppendLine("* Use an external IdP");
         contents.AppendLine();
         contents.AppendLine("## Brand and customize");
         contents.AppendLine("### Tailor your IAM tools with your organization's brand and give users a consistent, familiar experience");
         contents.AppendLine();
         contents.AppendLine("* Add a custom domain");
         contents.AppendLine("* Style email notifications");
         contents.AppendLine("* Customize the sign-in page");
         contents.AppendLine();
         contents.AppendLine("## Publish");
         contents.AppendLine("### Join the Okta Integration Network");
         contents.AppendLine("Promote your OIDC, SAML, SCIM, or API service integration to thousands of customers and grow your business with the Okta Integration Network (OIN)");
         contents.AppendLine();
         contents.AppendLine("* Learn about the OIN");
         contents.AppendLine("* Go to the Okta Integration Network");
         return;
      }
   }
}