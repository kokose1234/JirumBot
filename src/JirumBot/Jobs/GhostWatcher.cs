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

using Discord;
using FluentScheduler;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace JirumBot.Jobs
{
    public class GhostWatcher : IJob
    {
        public void Execute()
        {
            var guild = Constants.DiscordClient.GetGuild(Setting.Value.DiscordGuildId);
            if (guild != null)
            {
                var userRepository = ServiceProviderFactory.ServiceProvider.GetService<UserRepository>();
                if (userRepository == null)
                {
                    Console.WriteLine("userRepository가 null임");
                    return;
                }

                var users = userRepository.All();
                var categoryChannel = guild.GetCategoryChannel(Setting.Value.DiscordCategoryId);
                var normalUsers = guild.Users
                                       .Where(user => !user.IsBot)
                                       .ToImmutableList();
                var ghostUsers = normalUsers
                                 .Where(user => users.All(x => ulong.Parse(x.UserId) != user.Id))
                                 .ToImmutableList();

                if (ghostUsers.Count != 0)
                {
                    ghostUsers.ForEach(async user =>
                    {
                        var builder = new EmbedBuilder();
                        var commands = Program.Service.Commands;
                        var channel = await guild.CreateTextChannelAsync($"{user.Username}-알림",
                            properties => properties.CategoryId = categoryChannel.Id, RequestOptions.Default);
                        userRepository.Create(new User { UserId = user.Id.ToString(), ChannelId = channel.Id.ToString(), Keywords = new List<string>() });

                        builder.WithThumbnailUrl("https://cdn.discordapp.com/app-icons/821300653136805928/8caa403394f55277b851bbd841cd8b7d.png?size=512");
                        builder.WithColor(Color.Blue);
                        builder.WithTitle("명령어 목록");
                        builder.AddField("<:scroll:643684183293820998> 명령어 <:scroll:643684183293820998>", $"{GetFullUserName(user)}님이 사용하실 수 있는 명령어 목록입니다.");

                        foreach (var command in commands)
                        {
                            builder.AddField("/" + command.Name, string.IsNullOrEmpty(command.Summary) ? "설명이 없습니다." : command.Summary, true);
                        }

                        await channel.SendMessageAsync("", false, builder.Build());
                    });
                }
            }
        }

        private static string GetFullUserName(IUser usr)
        {
            var sb = new StringBuilder(usr.Username);
            sb.Append("#").Append(usr.DiscriminatorValue.ToString("D4"));
            return sb.ToString();
        }
    }
}