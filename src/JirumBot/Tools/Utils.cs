using System.Text;
using Discord;

namespace JirumBot.Tools;

public static class Utils
{
    public static bool IsValidString(string? input) => input != null && !string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input);

    public static string GetFullUserName(IUser usr)
    {
        var sb = new StringBuilder(usr.Username);
        sb.Append("#").Append(usr.DiscriminatorValue.ToString("D4"));
        return sb.ToString();
    }
}