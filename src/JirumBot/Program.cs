using System.Diagnostics;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JirumBot.Data;
using JirumBot.Database;
using JirumBot.Database.Repositories;
using JirumBot.Services;
using JirumBot.Services.Crawl;

namespace JirumBot
{
    public class Program
    {
        private static DiscordSocketClient s_client;

        public static async Task Main(string[] args)
        {
            Console.Title = "JirumBot";
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = Environment.CurrentDirectory
            });
            builder.Configuration.AddEnvironmentVariables();
            builder.Services.AddOptions();

            builder.Services.Configure<BotSetting>(builder.Configuration.GetSection("BotSetting"));
            builder.Services.AddSingleton(builder.Configuration);

            builder.Services.AddSingleton<LoggingService>();
            builder.Services.AddSingleton<DocumentStoreLifecycle>();
            builder.Services.AddSingleton<UserRepository>();

            var discord = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = short.MaxValue,
                AlwaysDownloadDefaultStickers = true,
                AlwaysDownloadUsers = true,
                AlwaysResolveStickers = true,
                GatewayIntents = GatewayIntents.All & ~GatewayIntents.GuildPresences & ~GatewayIntents.GuildInvites & ~GatewayIntents.GuildScheduledEvents
            });
            s_client = discord;
            builder.Services.AddSingleton(discord);
            builder.Services.AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Error,
                DefaultRunMode = Discord.Commands.RunMode.Async,
                CaseSensitiveCommands = false,
                IgnoreExtraArgs = true
            }));
            builder.Services.AddSingleton(new InteractionService(discord, new InteractionServiceConfig
            {
                DefaultRunMode = Discord.Interactions.RunMode.Async,
                ExitOnMissingModalField = true,
                EnableAutocompleteHandlers = true,
                LogLevel = LogSeverity.Error,
                UseCompiledLambda = true
            }));

            builder.Services.AddSingleton<StartupService>();

            builder.Services.AddSingleton<ClienService>();
            builder.Services.AddSingleton<CoolService>();
            builder.Services.AddSingleton<FmService>();
            builder.Services.AddSingleton<MeecoService>();
            builder.Services.AddSingleton<PpomppuService>();
            builder.Services.AddSingleton<QuasarService>();
            builder.Services.AddSingleton<RuliService>();

            var app = builder.Build();
            var startup = app.Services.GetService<StartupService>();
            if (startup == null)
            {
                Console.WriteLine("startup null");
                return;
            }

            await Task.Factory.StartNew(app.Services.GetService<ClienService>().MainTask);
            await Task.Factory.StartNew(app.Services.GetService<CoolService>().MainTask);
            await Task.Factory.StartNew(app.Services.GetService<FmService>().MainTask);
            await Task.Factory.StartNew(app.Services.GetService<MeecoService>().MainTask);
            await Task.Factory.StartNew(app.Services.GetService<PpomppuService>().MainTask);
            await Task.Factory.StartNew(app.Services.GetService<QuasarService>().MainTask);
            await Task.Factory.StartNew(app.Services.GetService<RuliService>().MainTask);

            await startup.StartAsync();
            await app.RunAsync();
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs e)
        {
            StopService();
            Process.GetCurrentProcess().Kill(true);
        }

        private static void StopService()
        {
            try
            {
                s_client.LogoutAsync().Wait();
                s_client.StopAsync().Wait();
                foreach (var process in Process.GetProcessesByName("chromedriver")) process.Kill(true);
            }
            catch
            {
                //ignored
            }
        }
    }
}