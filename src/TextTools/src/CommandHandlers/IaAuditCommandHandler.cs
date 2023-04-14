namespace TextTools.CommandHandlers
{
   using System;
   using System.Text;
   using System.Text.Json;
   using TextTools.Enums;

   /// <summary>
   /// Creates a report for the IA Audt fo developer.okta.com
   /// </summary>
   public class IaAuditCommandHandler : ReportCommandHandlerBase
   {
      /// <summary>
      /// Initializes a new instance fo the <see cref="IaAuditCommandHandler" /> class
      /// </summary>
      public IaAuditCommandHandler() : base()
      {
      }

      /// <summary>
      /// Initializes a new instance fo the <see cref="IaAuditCommandHandler" /> class
      /// </summary>
		/// <param name="coreOptions">The core report handler options</param>
		/// <param name="navbarFile">The navbar as a json file</param>
		/// <param name="conductorFile">The conductor as a json file</param>
      public IaAuditCommandHandler(ReportCommandBaseOptions coreOptions, FileInfo navbarFile, FileInfo conductorFile) : base(coreOptions)
      {
         NavbarFileInfo = navbarFile;
         ConductorFileInfo = conductorFile;
      }

      /// <summary>
      /// Gets or sets the directory containing navbar.json
      /// </summary>
      public FileInfo NavbarFileInfo { get; set; } = new FileInfo(@"c:\temp\navbar.json");

      /// <summary>
      /// Gets or sets the directory containing conductor.json
      /// </summary>
      public FileInfo ConductorFileInfo { get; set; } = new FileInfo(@"c:\temp\conductor.json");


      /// <summary>
      /// Gets or sets the contents of the left nav bar fiel navbar.const.js
      /// </summary>
      public List<NavbarEntry> LeftNav { get; set; } = new List<NavbarEntry>();

      /// <summary>
      /// Gets or sets the redirects set up in the conductor file
      /// </summary>
      public List<SiteRedirect> SiteRedirects { get; set; } = new List<SiteRedirect>();

      /// <inheritdoc />
      protected override bool ValidateNonCoreOptions()
      {
         if (!NavbarFileInfo.Exists)
         {
            SendToConsole($"{NavbarFileInfo.FullName} does not exist", ConsoleColor.Red);
            return false;
         }

         if (!ConductorFileInfo.Exists)
         {
            SendToConsole($"{ConductorFileInfo.FullName} does not exist", ConsoleColor.Red);
            return false;
         }

         return true;
      }

      /// <inheritdoc />
      protected override void BuildReport()
      {
         ReportFileName = "IAFileReport";
         SendToConsole("Running IA File Report", ConsoleColor.Green);

         Worksheet fileReport = new Worksheet("filereport");
         SiteRedirects = ReadRedirectsFromConductorFile(ConductorFileInfo);
         LeftNav = ReadNavbarFromFile(NavbarFileInfo);

         // Add header row
         fileReport.Rows.Add(new List<string> { "Directory", "Article Title", "Variant", "In Left Nav?", "Count of Redirects From This Doc", "URLs redirected to from this Doc", "Count of redirects To This Doc", "URLs redirecting to this doc" });

         foreach (var sourceDirectory in SourceDirectory.EnumerateDirectories("*", RecurseDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
         {
            // SendToConsole($"Pulling data from {sourceDirectory.FullName}", ConsoleColor.DarkYellow);
            ExtractData(fileReport, sourceDirectory);
         }

         Reports.Add(fileReport);
         SendToConsole("Report Complete", ConsoleColor.Green);
      }

      private void ExtractData(Worksheet fileReport, DirectoryInfo currentDirectory)
      {
         var filesInDirectory = currentDirectory.GetFiles();

         // No article here if there is no index.md, so return nothing
         if (filesInDirectory.None(fi => fi.Name == "index.md"))
         {
            return;
         }

         var relativePath = Path.GetRelativePath(SourceDirectory.FullName, currentDirectory.FullName);
         var indexmd = filesInDirectory.Single(fi => fi.Name == "index.md");
         var articleTitle = GetArticleTitle(indexmd);
         var articleVariants = GetArticleVariants(currentDirectory, indexmd);
         string breadcrumb = string.Empty;
         var listedInLeftNav = IsInNavbar(relativePath, out breadcrumb);
         var redirectFromCount = CountRedirectsFrom(relativePath);
         var redirectFromUrls = GetRedirectFromUrls(relativePath);
         var redirectToCount = CountRedirectsTo(relativePath);
         var redirectToUrls = GetRedirectToUrls(relativePath);

         if (articleVariants.Count == 1)
         {
            if (!relativePath.EndsWith("main"))
            {
               fileReport.Rows.Add(new List<string> {
                  relativePath,
                  articleTitle,
                  articleVariants.Single(),
                  breadcrumb,
                  redirectFromCount,
                  redirectFromUrls.AsStringSeparatedList(Environment.NewLine, false),
                  redirectToCount,
                  redirectToUrls.AsStringSeparatedList(Environment.NewLine, false)
                  });
            }
         }
         else
         {
            foreach (var variant in articleVariants)
            {
               fileReport.Rows.Add(new List<string> {
                  relativePath,
                  articleTitle,
                  variant,
                  "n/a",
                  redirectFromCount,
                  redirectFromUrls.AsStringSeparatedList(Environment.NewLine, false),
                  redirectToCount,
                  redirectToUrls.AsStringSeparatedList(Environment.NewLine, false)
                  });
            }
         }
      }

      private List<SiteRedirect> ReadRedirectsFromConductorFile(FileInfo conductorFile)
      {
         var options = new JsonSerializerOptions
         {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true

         };
         string jsonString = File.ReadAllText(conductorFile.FullName);
         ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(jsonString);
         var entries = JsonSerializer.Deserialize<List<SiteRedirect>>(jsonReadOnlySpan, options)!;
         SendToConsole($"Found {entries.Count} redirects", ConsoleColor.Green);
         return entries;
      }

      private List<NavbarEntry> ReadNavbarFromFile(FileInfo navbarFile)
      {
         var options = new JsonSerializerOptions
         {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
         };

         string jsonString = File.ReadAllText(navbarFile.FullName);
         ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(jsonString);
         var entries = JsonSerializer.Deserialize<List<NavbarEntry>>(jsonReadOnlySpan, options)!;
         SendToConsole($"Found {entries.Count} main navbar entries: {entries.Select(e => e.Title).AsCsv(true)}", ConsoleColor.Green);

         // Clean entries up a bit
         foreach (var entry in entries)
         {
            CleanNavbarEntry(entry, entry.Path, entry.Breadcrumb);
         }

         File.WriteAllText(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "navbar_generated.json"),
            JsonSerializer.Serialize(entries, options)
         );

         return entries;
      }

      private void CleanNavbarEntry(NavbarEntry entry, string rootPath, string parentBreadcrumb)
      {
         if (String.IsNullOrWhiteSpace(entry.Path))
         {
            if (String.IsNullOrWhiteSpace(entry.GuideName))
            {
               entry.Path = string.Empty;
            }
            else
            {
               entry.Path = Path.Combine(rootPath, entry.GuideName ?? string.Empty);
            }
         }

         entry.Path = entry.Path.Replace("/", @"\");
         entry.Path = entry.Path.Trim().Trim('\\');

         if (entry.Path.EndsWith("main"))
         {
            entry.Path = entry.Path.Remove(entry.Path.Length - 4);
         }

         if (string.IsNullOrWhiteSpace(parentBreadcrumb))
         {
            entry.Breadcrumb = entry.Title;
         }
         else
         {
            entry.Breadcrumb = $"{parentBreadcrumb} > {entry.Title}";
         }

         foreach (var subEntry in entry.SubLinks)
         {
            CleanNavbarEntry(subEntry, rootPath, entry.Breadcrumb);
         }
      }

      private string GetArticleTitle(FileInfo indexmd)
      {
         var titleLine = indexmd.AsStringList().FirstOrDefault(line => line.StartsWith("title:", StringComparison.OrdinalIgnoreCase));
         return String.IsNullOrWhiteSpace(titleLine) ? "unknown" : titleLine.Substring(6).Trim();
      }

      private List<string> GetArticleVariants(DirectoryInfo currentDirectory, FileInfo indexmd)
      {
         var articleVariants = new List<string>();

         // if there are no StackSnippets in index.md, there are no variants, so return List.
         if (indexmd.AsStringList().Any(line => line.Contains("StackSnippet", StringComparison.OrdinalIgnoreCase)))
         {
            articleVariants.AddRange(currentDirectory.EnumerateDirectories().Select(dir => dir.Name));
         }
         else
         {
            articleVariants.Add("none");
         }

         return articleVariants;
      }

      private bool IsInNavbar(string relativePath, out string breadcrumb)
      {
         breadcrumb = string.Empty;
         string entrybreadcrumb = string.Empty;
         foreach (var entry in LeftNav)
         {
            if (IsInNavbar(entry, relativePath, out entrybreadcrumb))
            {
               breadcrumb = entrybreadcrumb;
               return true;
            }
         }

         return false;
      }

      private bool IsInNavbar(NavbarEntry entry, string relativePath, out string entrybreadcrumb)
      {
         entrybreadcrumb = string.Empty;
         if (relativePath == entry.Path)
         {
            entrybreadcrumb = entry.Breadcrumb;
            return true;
         }

         string subentrybreadcrumb = string.Empty;
         foreach (var subEntry in entry.SubLinks)
         {
            if (IsInNavbar(subEntry, relativePath, out subentrybreadcrumb))
            {
               entrybreadcrumb = subentrybreadcrumb;
               return true;
            }
         }

         return false;
      }

      private string CountRedirectsFrom(string relativePath) => SiteRedirects.Count(sr => sr.From == relativePath).ToString();

      private string CountRedirectsTo(string relativePath) => SiteRedirects.Count(sr => sr.To == relativePath).ToString();

      private IEnumerable<string> GetRedirectFromUrls(string relativePath) => SiteRedirects.Where(sr => sr.From == relativePath).Select(sr => sr.To);

      private IEnumerable<string> GetRedirectToUrls(string relativePath) => SiteRedirects.Where(sr => sr.To == relativePath).Select(sr => sr.From);

   }
}