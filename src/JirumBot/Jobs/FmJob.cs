using System;
using System.Collections.Immutable;
using System.Linq;
using Discord;
using FluentScheduler;
using JirumBot.CrawlManager;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using JirumBot.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace JirumBot.Jobs
{
    public class FmJob : IJob
    {
        public async void Execute()
        {
            if (Constants.TestMode) return;
            if (TimeUtil.IsBetween(DateTime.Now.TimeOfDay, Setting.Value.StopTime, Setting.Value.StartTime)) return;

            var guild = Constants.DiscordClient.GetGuild(Setting.Value.DiscordGuildId);
            var repo = ServiceProviderFactory.ServiceProvider.GetService<UserRepository>();
            if (repo == null)
            {
                Console.WriteLine("[FmJob] repo가 null임.");
                return;
            }

            var users = repo.All();

            if (await FmManager.Instance.FetchNewArticles())
            {
                if (FmManager.Instance.Articles.Count == 0) return;

                foreach (var user in users)
                {
                    var channel = guild?.GetTextChannel(ulong.Parse(user.ChannelId));
                    if (channel != null)
                    {
                        var guildUser = guild.GetUser(ulong.Parse(user.UserId));
                        if (guildUser == null) continue;

                        foreach (var article in FmManager.Instance.Articles.ToImmutableArray())
                        {
                            if (user.Keywords.Count > 0 && user.Keywords.Any(keyword => article.Title.ToLower().Contains(keyword.ToLower())))
                            {
                                var builder = new EmbedBuilder();

                                builder.WithColor(135, 206, 235);
                                builder.WithCurrentTimestamp();
                                builder.WithUrl(article.Url);
                                builder.WithTitle(article.Title);

                                await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                            }
                        }
                    }
                }

                FmManager.Instance.Articles.Clear();
            }
        }
    }
}