using Discord.WebSocket;
using JirumBot.CrawlManager;

namespace JirumBot
{
    public static class Constants
    {
        internal static DiscordSocketClient DiscordClient { get; set; }
        internal static Logger Logger { get; } = new();

        public static bool TestMode = false;
    }
}