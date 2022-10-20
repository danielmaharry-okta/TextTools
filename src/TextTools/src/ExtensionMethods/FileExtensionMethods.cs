namespace TextTools
{
   /// <summary>
   /// Set of useful file extension methods
   /// </summary>
   public static class ExtensionMethods
   {
      /// <summary>
      /// Returns a list of non-null, non-whitespace strings from a file
      /// </summary>
      /// <param name="sourceFile">The file to query</param>
      /// <returns>the list of strings</returns>
      public static List<string> AsStringList(this FileInfo sourceFile)
      {
         if (sourceFile == null)
         {
            throw new ArgumentNullException(nameof(sourceFile));
         }

         var strings = new List<string>();

         try
         {
            foreach (var line in File.ReadLines(sourceFile.FullName))
            {
               if (!string.IsNullOrWhiteSpace(line))
               {
                  strings.Add(line.Trim());
               }
            }
         }
         catch (UnauthorizedAccessException uAEx)
         {
            Console.WriteLine(uAEx.Message);
         }
         catch (PathTooLongException pathEx)
         {
            Console.WriteLine(pathEx.Message);
         }

         return strings;
      }
   }
}