namespace TextTools
{
   using System;
   using CsvHelper.Configuration.Attributes;

   /// <summary>
   /// Represents a row in lib\pagelist.csv
   /// </summary>
   public class PageListEntry
   {
      /// <summary>
      /// Gets or sets the page ID
      /// </summary>
      [Name("Id")]
      public string Id { get; set; } = "0";

      /// <summary>
      /// Gets or sets the current draft status of the page in the spreadsheet
      /// </summary>
      [Name("Edit status")]
      public string EditStatus { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the person dealing with this page in the IA
      /// </summary>
      [Name("Owner")]
      public string Owner { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the role of the content in the IA. Homepage, primary, secondary
      /// </summary>
      [Name("Group type")]
      public string ContentRole { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the page's level 1 value
      /// </summary>
      [Name("Level 1")]
      public string Level1 { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the page's level 2 value
      /// </summary>
      [Name("Level 2")]
      public string Level2 { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the page's level 3 value
      /// </summary>
      [Name("Level 3")]
      public string Level3 { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the page's level 4 value
      /// </summary>
      [Name("Level 4")]
      public string Level4 { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the page's level 5 value
      /// </summary>
      [Name("Level 5")]
      public string Level5 { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the page's level 6 value
      /// </summary>
      [Name("Level 6")]
      public string Level6 { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the article title
      /// </summary>
      [Name("Article Title")]
      public string Title { get; set; } = "Article Title";

      /// <summary>
      /// Gets or sets the page's content type
      /// </summary>
      [Name("Content Type")]
      public string ContentType { get; set; } = "Unknown";

      /// <summary>
      /// Gets or sets the page's keywords
      /// </summary>
      [Name("Keywords")]
      public string Keywords { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets whether the page will be included in phase 1
      /// </summary>
      [Name("Included in phase 1?")]
      public string InPhase1 { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets whether the page will be included in phase 2
      /// </summary>
      [Name("Included in phase 2")]
      public string InPhase2 { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the help docs scope for the page
      /// </summary>
      [Name("Help docs scope (needs further discussion)")]
      public string HelpDocsScope { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the page's document type
      /// </summary>
      [Name("Document change type")]
      public string DocumentType { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the suggested changes for the page
      /// </summary>
      [Name("Changes to existing doc (if applicable)")]
      public string SuggestedChanges { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the links to existing content on DevDocs for this page
      /// </summary>
      [Name("Existing content link (if applicable)")]
      public string ExistingLinks { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets any ways that this page will need to be dimensioned.
      /// For example, does it need to be written multiple times for different languages, JS frameworks, auth flows etc.
      /// </summary>
      [Name("Content dimensions")]
      public string Dimensions { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the page's group description
      /// </summary>
      [Name("Group description (if applicable)")]
      public string GroupDescription { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the page's doc description
      /// </summary>
      [Name("Doc description (if applicable)")]
      public string DocDescription { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets any user research validation for the decisions made about this document
      /// </summary>
      [Name("User research validation")]
      public string Validation { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the target personas for the page.
      /// </summary>
      [Name("Primary targetted personas")]
      public string TargetPersonas { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the page's weight - it's position in the left hand menu
      /// </summary>
      [Ignore]
      public int Weight { get; set; } = 0;

      /// <summary>
      /// Gets or sets whether the page is an artifically added stub for the nav
      /// </summary>
      [Ignore]
      public bool IsStub { get; set; } = false;

      /// <summary>
      /// Calculates the absolute file path for the page
      /// </summary>
      /// <param name="baseDirectory"></param>
      /// <param name="navRootDirectory">The name of an additional directory to place the file inside.</param>
      /// <returns>The absolute file path for the page</returns>
      public FileInfo GetAbsolutePageFilePath(DirectoryInfo baseDirectory, string navRootDirectory)
      {
         return new FileInfo(Path.Combine(baseDirectory.FullName, GetRelativeFilePath(navRootDirectory)));
      }

      /// <summary>
      /// Calculates the relative file path for the page
      /// </summary>
      /// <param name="navRootDirectory">The name of an additional directory to place the file inside.</param>
      /// <returns>The path of the file relative to the root of the content</returns>
      public string GetRelativeFilePath(string navRootDirectory)
      {
         return Path.Combine(navRootDirectory.Trim(), Level1.Trim(), Level2.Trim(), Level3.Trim(), Level4.Trim(), Level5.Trim(), Level6.Trim(), "_index.md").AsSafeFileName();
      }

      /// <summary>
      /// Gets the title of the page for the navigation bar
      /// </summary>
      public string GetNavTitle()
      {
         return new string[] { Level6, Level5, Level4, Level3, Level2, Level1 }.FirstOrDefault(s => s.HasValue()) ?? Title;
      }
   }

   /// <summary>
   /// Represents a row in lib\contenttypes.csv
   /// </summary>
   public class ContentType
   {
      /// <summary>
      /// Gets or sets the name of the content type
      /// </summary>
      [Name("Content Type")]
      public string Name { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the description of the content type
      /// </summary>
      [Name("Description")]
      public string Description { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets any external links for examples of the content type
      /// </summary>
      [Name("External links")]
      public string ExternalLinks { get; set; } = string.Empty;
   }
}