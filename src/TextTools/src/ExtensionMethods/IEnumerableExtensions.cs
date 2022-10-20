namespace TextTools
{
    /// <summary>
    /// Contains IEnumerable extension methods
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Determines whether a sequence <paramref name="source"/> contains no elements
        /// </summary>
        /// <param name="source">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to check for emptiness.</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns><c>true</c> if the source sequence is null or contains no elements; otherwise <c>false</c></returns>
        public static bool None<T>(this IEnumerable<T> source) => source == null || !source.Any();

        /// <summary>
        /// Determines whether a sequence <paramref name="source"/> contains no elements matching a <paramref name="predicate" />
        /// </summary>
        /// <param name="source">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to check for emptiness.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns><c>true</c> if the source sequence is null or contains no elements; otherwise <c>false</c></returns>
        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate) => source == null || !source.Any(predicate);

        /// <summary>
        /// Generates a comma-separated string from a list of string values <paramref name="source"/>
        /// </summary>
        /// <param name="source">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to write out as a comma-separated string.</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>A comma-separated string</returns>
        public static string AsCsv<T>(this IEnumerable<T> source) => source.AsCharacterSeparatedList(',', false);

        /// <summary>
        /// Generates a comma-separated string from a list of string values
        /// </summary>
        /// <param name="source">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to write out as a comma-separated string.</param>
        /// <param name="includeSpace"><c>true</c> if list should be comma+space separated. <c>false</c> if comma-separated only</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>A comma-separated string</returns>
        public static string AsCsv<T>(this IEnumerable<T> source, bool includeSpace) => source.AsCharacterSeparatedList(',', includeSpace);

        /// <summary>
        /// Generates a character-separated string from a list of string values
        /// </summary>
        /// <param name="source">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to write out as a character-separated string.</param>
        /// <param name="character">The separation character to use</param>
        /// <param name="includeSpace"><c>true</c> if list should be character+space separated. <c>false</c> if comma-separated only</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>A comma-separated string</returns>
        public static string AsCharacterSeparatedList<T>(this IEnumerable<T> source, char character, bool includeSpace) => source.AsStringSeparatedList(character.ToString(), includeSpace);

        /// <summary>
        /// Generates a string-separated string from a list of string values
        /// </summary>
        /// <param name="source">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to write out as a character-separated string.</param>
        /// <param name="separator">The separation string to use</param>
        /// <param name="includeSpace"><c>true</c> if list should be character+space separated. <c>false</c> if comma-separated only</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>A comma-separated string</returns>
        public static string AsStringSeparatedList<T>(this IEnumerable<T> source, string separator, bool includeSpace)
        {
            return string.Join($"{separator}{(includeSpace ? " " : string.Empty)}", source.Select(item => item?.ToString() ?? "null"));
        }

        /// <summary>
        /// Generates a numbered list string from a list of string values <paramref name="source"/>
        /// </summary>
        /// <param name="source">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to write out as a numbered list.</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>A numbered list as a string</returns>
        public static string AsNumberedList<T>(this IEnumerable<T> source) => source.AsNumberedList(string.Empty, ". ", Environment.NewLine);

        /// <summary>
        /// Generates a numbered list string from a list of string values <paramref name="source"/>
        /// </summary>
        /// <param name="source">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to write out as a numbered list.</param>
        /// <param name="numberPrefix">The characters to add before the number in the list</param>
        /// <param name="numberSuffix">The characters to add between the number and the string</param>
        /// <param name="joinCharacter">The join character between each item in the list</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>A numbered list as a string</returns>
        public static string AsNumberedList<T>(this IEnumerable<T> source, string numberPrefix, string numberSuffix, string joinCharacter)
        {
            if (source == null || source.None())
            {
                return string.Empty;
            }

            if (source.Count() == 1)
            {
                return source.AsCsv();
            }

            return string.Join(joinCharacter, source.Select((x, n) => $"{numberPrefix}{n + 1}{numberSuffix}{x}"));
        }

        /// <summary>
        /// Generates a bulleted list string from a list of string values <paramref name="source"/>
        /// </summary>
        /// <param name="source">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to write out as a bulleted list.</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>A bulleted list as a string</returns>
        public static string AsBulletedList<T>(this IEnumerable<T> source) => source.AsBulletedList('•');

        /// <summary>
        /// Generates a bulleted list string from a list of string values <paramref name="source"/>
        /// </summary>
        /// <param name="source">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to write out as a bulleted list.</param>
        /// <param name="character">The bullet character to use</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>A bulleted list as a string</returns>
        public static string AsBulletedList<T>(this IEnumerable<T> source, char character)
        {
            if (source == null || source.None())
            {
                return string.Empty;
            }

            return string.Join(' ', source.Select(s => s != null ? $"{character} {s.ToString()}" : $"{character} null"));
        }

        /// <summary>
        /// Determines whether an item is the first in an ordered collection of items
        /// </summary>
        /// <param name="item">The item that may or may not be first in <paramref name="items" /></param>
        /// <param name="items">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to check against</param>
        /// <typeparam name="T">The type of the elements of <paramref name="items" /></typeparam>
        /// <returns><c>true</c> if the item is the first object in the target collection; otherwise <c>false</c></returns>
        public static bool IsFirstIn<T>(this T item, IEnumerable<T> items)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (Enumerable.Any(items))
            {
                return item.Equals(Enumerable.First(items));
            }

            return false;
        }

        /// <summary>
        /// Determines whether an item is the last in an ordered collection of items
        /// </summary>
        /// <param name="item">The item that may or may not be last in <paramref name="items" /></param>
        /// <param name="items">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to check against</param>
        /// <typeparam name="T">The type of the elements of <paramref name="items" /></typeparam>
        /// <returns><c>true</c> if the item is the last object in the target collection; otherwise <c>false</c></returns>
        public static bool IsLastIn<T>(this T item, IEnumerable<T> items)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (Enumerable.Any(items))
            {
                return item.Equals(Enumerable.Last(items));
            }

            return false;
        }

        /// <summary>
        /// Returns the next item in a collection <paramref name="items" /> after <paramref name="current" />
        /// </summary>
        /// <param name="items">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to check against</param>
        /// <param name="current">The current item in <paramref name="items" /></param>
        /// <typeparam name="T">The type of the elements of <paramref name="items" /></typeparam>
        /// <returns>The next item after the current one in the list or default(T)</returns>
        public static T ElementAfter<T>(this IEnumerable<T> items, T current)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }

            if (!items.Contains(current))
            {
                throw new ArgumentOutOfRangeException(nameof(current), "Item cannot be found in enumerable");
            }

            return items.SkipWhile(x => !current.Equals(x)).Skip(1).FirstOrDefault();
        }

        /// <summary>
        /// Returns the previous item in a collection <paramref name="items" /> before <paramref name="current" />
        /// </summary>
        /// <param name="items">The <see cref="System.Collections.Generic.IEnumerable{T}" /> to check against</param>
        /// <param name="current">The current item in <paramref name="items" /></param>
        /// <typeparam name="T">The type of the elements of <paramref name="items" /></typeparam>
        /// <returns>The previous item before the current one in the list or default(T)</returns>
        public static T ElementBefore<T>(this IEnumerable<T> items, T current)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }

            if (!items.Contains(current))
            {
                throw new ArgumentOutOfRangeException(nameof(current), "Item cannot be found in enumerable");
            }

            return items.TakeWhile(x => !current.Equals(x)).LastOrDefault();
        }
    }
}