namespace TextTools
{
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
      public static string AsSafeFileName(this string filename) =>
			Regex.Replace(
				filename, $"[{string.Concat(Path.GetInvalidFileNameChars())}]", string.Empty);
   }
}