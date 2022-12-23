//  Copyright 2022 Jonguk Kim
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

using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JirumBot.Data;
using JirumBot.Database;
using JirumBot.Database.Interfaces;
using JirumBot.Database.Repositories;
using JirumBot.Services;
using RunMode = Discord.Commands.RunMode;

namespace JirumBot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Title = $"JirumBot v{Constants.VERSION}";
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = Environment.CurrentDirectory
            });
            builder.Configuration.AddEnvironmentVariables();
            builder.Services.AddOptions();

            builder.Services.Configure<DiscordSetting>(builder.Configuration.GetSection("DiscordSetting"));
            builder.Services.Configure<MongoConfiguration>(builder.Configuration.GetSection("MongoConfiguration"));

            builder.Services.AddSingleton(builder.Configuration);

            var discord = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = short.MaxValue,
                AlwaysDownloadDefaultStickers = true,
                AlwaysDownloadUsers = true,
                AlwaysResolveStickers = true,
                GatewayIntents = GatewayIntents.All & ~GatewayIntents.GuildPresences & ~GatewayIntents.GuildInvites & ~GatewayIntents.GuildScheduledEvents
            });
            builder.Services.AddSingleton(discord);
            builder.Services.AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Error,
                DefaultRunMode = RunMode.Async,
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
            builder.Services.AddSingleton<LoggingService>();
            builder.Services.AddSingleton<SchedulerService>();
            builder.Services.AddSingleton<IMongoContext, MongoContext>();

            builder.Services.AddSingleton<UserRepository>();

            var app = builder.Build();

            var startup = app.Services.GetService<StartupService>();
            var scheduler = app.Services.GetService<SchedulerService>();

            if (startup == null || scheduler == null) return;

            await startup.StartAsync();
            scheduler.Start();
            await app.RunAsync();
        }
    }
}