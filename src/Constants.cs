using Discord.Commands;
using Discord.WebSocket;
using JirumBot.ChromeManagers;

namespace JirumBot
{
    public class Constants
    {
        public static QuasarManager QuasarJirumManager { get; } = new();
        public static QuasarManager QuasarJirumManager2 { get; } = new(); //장터
        public static CoolManager CoolJirumManager { get; } = new();
        public static CoolManager CoolJirumManager2 { get; } = new(); //장터
        internal static DiscordSocketClient DiscordClient { get; set; }
        internal static Logger Logger { get; } = new ();
    }
}