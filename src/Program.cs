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
using JirumBot.Data;
using JirumBot.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace JirumBot
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            Console.Title = "JirumBot";

            await MainAsync();
        }

        private static async Task MainAsync()
        {
            await using var configureServices = ConfigureServices();
            var discordSocketClient = configureServices.GetRequiredService<DiscordSocketClient>();

            discordSocketClient.Log += Log;
            discordSocketClient.MessageReceived += DiscordSocketClientOnMessageReceived;
            JobManager.JobException += info => Constants.Logger.GetExceptionLogger().Error(info.Exception, "작업 실행중 예외 발생");
            Constants.DiscordClient = configureServices.GetRequiredService<DiscordSocketClient>();

            await Constants.CoolJirumManager.Login("https://coolenjoy.net/bbs/jirum");
            await Constants.CoolJirumManager2.Login("https://coolenjoy.net/bbs/mart2");
            await Constants.QuasarJirumManager.Login("https://quasarzone.com/bbs/qb_saleinfo");
            await Constants.QuasarJirumManager2.Login("https://quasarzone.com/bbs/qb_jijang");
            Constants.Logger.GetLogger().Info("퀘이사존, 쿨엔조이 로드 완료");

            await discordSocketClient.LoginAsync(TokenType.Bot, Setting.Value.DiscordBotToken);
            await discordSocketClient.StartAsync();
            await discordSocketClient.SetActivityAsync(new Game("돈 쓸 곳 찾는 중"));

            JobManager.Initialize(new CommonRegistry());

            await Task.Delay(-1);
        }

        private static Task DiscordSocketClientOnMessageReceived(SocketMessage arg)
        {
            if (arg.Content == "/종료")
            {
                arg.DeleteAsync();
                Constants.DiscordClient.LogoutAsync();
                Constants.DiscordClient.StopAsync();
                JobManager.RemoveAllJobs();
                JobManager.Stop();
                Constants.CoolJirumManager.Dispose();
                Constants.CoolJirumManager2.Dispose();
                Constants.QuasarJirumManager.Dispose();
                Constants.QuasarJirumManager2.Dispose();
                Environment.Exit(0);
            }

            return Task.CompletedTask;
        }

        private static ServiceProvider ConfigureServices()
        {
            var discordSocketConfig = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = short.MaxValue,
                AlwaysDownloadUsers = true
            });

            var commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });

            return new ServiceCollection()
                   .AddSingleton(discordSocketConfig)
                   .AddSingleton(commandService)
                   .BuildServiceProvider();
        }

        private static Task Log(LogMessage msg)
        {
            Constants.Logger.GetLogger().Info(msg.ToString());
            return Task.CompletedTask;
        }
    }
}