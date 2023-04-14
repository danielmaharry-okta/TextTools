namespace TextTools
{
   /// <summary>
   /// Represents a single entry in the navbar
   /// </summary>
   public class NavbarEntry
   {
      /// <summary>
      /// Gets or sets the title of the entry
      /// </summary>
      public string Title { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the relative path of the entry
      /// </summary>
      public string Path { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the guidename of the entry
      /// </summary>
      public string GuideName { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the breadcrumb trail to the page
      /// </summary>
      public string Breadcrumb { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the set of sublinks in the navbar
      /// </summary>
      public List<NavbarEntry> SubLinks { get; set; } = new List<NavbarEntry>();
   }

   /// <summary>
   /// Represents a single redirect in a condutor file
   /// </summary>
   public class SiteRedirect
   {
      /// <summary>
      /// Gets or sets a path being redirected from
      /// </summary>
      public string From { get; set; } = string.Empty;

      /// <summary>
      /// Gets or sets the target path fro the redirection
      /// </summary>
      public string To { get; set; } = string.Empty;
   }
}