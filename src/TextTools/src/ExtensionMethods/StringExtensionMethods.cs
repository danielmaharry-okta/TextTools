namespace TextTools
{
   using System;
   using System.Text.RegularExpressions;

   /// <summary>
   /// Set of useful string extension methods
   /// </summary>
   public static class StringExtensionMethods
   {
      /// <summary>
      /// Returns a safe filename by removing any invalid characters for that operating system's filenames
      /// </summary>
      /// <param name="filename">The filename to make safe</param>
      /// <returns>The safe version of the filename</returns>
      public static string AsSafeFileName(this string filename)
      {
			string safename = Regex.Replace(filename, $"[{string.Concat(Path.GetInvalidFileNameChars())}]", string.Empty);
         return safename.Replace(",", string.Empty).Replace(" ", "-").Replace("'", string.Empty);
      }


      /// <summary>
      /// Returns whether a string has any non-whitespace contents
      /// </summary>
      /// <param name="s"></param>
      /// <returns>true if it ahs contents, false if null</returns>
      public static bool HasValue(this string s) => !String.IsNullOrWhiteSpace(s);
   }
}