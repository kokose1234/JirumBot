//  Copyright 2021 Jonguk Kim
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FluentScheduler;
using JirumBot.Command;
using JirumBot.CrawlManager;
using JirumBot.Data;
using JirumBot.Database;
using JirumBot.Database.Repositories;
using JirumBot.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace JirumBot
{
    internal static class Program
    {
        public static CommandService Service { get; private set; }
        private static bool s_initialized;

        private static async Task Main(string[] args)
        {
            Console.Title = "JirumBot";
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;

            await MainAsync();
        }

        private static async Task MainAsync()
        {
            ServiceProviderFactory.ServiceProvider = ConfigureServices();
            var discordSocketClient = ServiceProviderFactory.ServiceProvider.GetRequiredService<DiscordSocketClient>();

            discordSocketClient.Log += Log;
            JobManager.JobException += info => Constants.Logger.GetExceptionLogger().Error(info.Exception, "작업 실행중 예외 발생");
            Constants.DiscordClient = ServiceProviderFactory.ServiceProvider.GetRequiredService<DiscordSocketClient>();
            Service = ServiceProviderFactory.ServiceProvider.GetRequiredService<CommandService>();
            ServiceProviderFactory.ServiceProvider.GetRequiredService<CommandService>().Log += Log;

            _ = QuasarManager.Instance;
            _ = CoolManager.Instance;
            _ = FmManager.Instance;
            _ = ClienManager.Instance;
            _ = RuliManager.Instance;
            _ = MeecoManager.Instance;
            _ = PpomppuManager.Instance;
            Constants.Logger.GetLogger().Info("퀘이사존, 쿨엔조이, 뽐뿌, 펨코, 루리웹, 클리앙, 미니기기 코리아 로드 완료");

#if DEBUG
            while (true)
            {
                await ClienManager.Instance.FetchNewArticles();
                await Task.Delay(10 * 1000);
            }
#endif

            await discordSocketClient.LoginAsync(TokenType.Bot, Setting.Value.DiscordBotToken);
            await discordSocketClient.StartAsync();
            await discordSocketClient.SetActivityAsync(new Game("돈 쓸 곳 찾기"));
            await ServiceProviderFactory.ServiceProvider.GetRequiredService<CommandHandler>().InitializeAsync();

            discordSocketClient.Ready += DiscordSocketClientOnReady;

            await Task.Delay(-1);
        }

        private static Task DiscordSocketClientOnReady()
        {
            if (!s_initialized)
            {
#if RELEASE
                JobManager.Initialize(new CommonRegistry());
#endif
                s_initialized = true;
            }

            return Task.CompletedTask;
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs e)
        {
            StopService();
            Process.GetCurrentProcess().Kill(true);
        }

        private static ServiceProvider ConfigureServices()
        {
            var flag = GatewayIntents.All;
            flag &= ~(GatewayIntents.GuildPresences | GatewayIntents.GuildScheduledEvents | GatewayIntents.GuildInvites);

            var discordSocketConfig = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = short.MaxValue,
                AlwaysDownloadUsers = true,
                GatewayIntents = flag
            });

            var commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });

            return new ServiceCollection()
                   .AddSingleton(discordSocketConfig)
                   .AddSingleton(commandService)
                   .AddSingleton<CommandHandler>()
                   .AddSingleton<DocumentStoreLifecycle>()
                   .AddTransient<UserRepository>()
                   .BuildServiceProvider();
        }

        private static Task Log(LogMessage msg)
        {
            Constants.Logger.GetLogger().Info(msg.ToString());
            return Task.CompletedTask;
        }

        private static void StopService()
        {
            try
            {
                Constants.DiscordClient.LogoutAsync().Wait();
                Constants.DiscordClient.StopAsync().Wait();

                foreach (var process in Process.GetProcessesByName("chromedriver")) process.Kill(true);
            }
            catch
            {
                //ignored
            }
        }
    }
}