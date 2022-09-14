using System.Text;
using Discord;
using Discord.WebSocket;

namespace JirumBot.Tools;

public static class Utils
{
    public static string GetFullUserName(IUser? usr)
    {
        var sb = new StringBuilder(usr.Username);
        sb.Append("#").Append(usr.DiscriminatorValue.ToString("D4"));
        return sb.ToString();
    }

    public static bool IsPrivateMessage(SocketMessage msg)
    {
        return (msg.Channel.GetType() == typeof(SocketDMChannel));
    }
}