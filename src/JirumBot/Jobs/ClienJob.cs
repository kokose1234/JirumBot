using System;
using System.Collections.Immutable;
using System.Linq;
using Discord;
using FluentScheduler;
using JirumBot.CrawlManager;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace JirumBot.Jobs
{
    public class ClienJob : IJob
    {
        public async void Execute()
        {
            if (Constants.TestMode) return;

            var guild = Constants.DiscordClient.GetGuild(Setting.Value.DiscordGuildId);
            var repo = ServiceProviderFactory.ServiceProvider.GetService<UserRepository>();
            if (repo == null)
            {
                Console.WriteLine("[ClienJob] repo가 null임.");
                return;
            }

            var users = repo.All();

            if (await ClienManager.Instance.FetchNewArticles())
            {
                if (ClienManager.Instance.Articles.Count == 0) return;

                foreach (var user in users)
                {
                    var channel = guild?.GetTextChannel(ulong.Parse(user.ChannelId));
                    if (channel != null)
                    {
                        var guildUser = guild.GetUser(ulong.Parse(user.UserId));
                        if (guildUser == null) continue;

                        foreach (var article in ClienManager.Instance.Articles.ToImmutableArray())
                        {
                            if (user.Keywords.Count > 0 && user.Keywords.Any(keyword => article.Title.ToLower().Contains(keyword.ToLower())))
                            {
                                var builder = new EmbedBuilder();

                                builder.WithColor(35, 47, 62);
                                builder.WithCurrentTimestamp();
                                builder.WithUrl(article.Url);
                                builder.WithTitle(article.Title);

                                await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                            }
                        }
                    }
                }

                ClienManager.Instance.Articles.Clear();
            }
        }
    }
}