namespace ROP;
/// <summary>
/// Extension methods for IEnumerable.
/// </summary>
public static class IEnumerableUtils
{
    /// <summary>
    /// Shorthand for string.Join(separator, strings)
    /// </summary>
    public static string JoinStrings(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);
}