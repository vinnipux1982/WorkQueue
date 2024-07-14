namespace Common;

public static class StringExtensions
{
    public static bool Empty(this string s)
    {
        return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
    }

    public static bool NotEmpty(this string s)
    {
        return !s.Empty();
    }
}