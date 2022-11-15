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
    public class CoolJob : IJob
    {
        public async void Execute()
        {
            if (Constants.TestMode) return;
            if (TimeUtil.IsBetween(DateTime.Now.TimeOfDay, Setting.Value.StopTime, Setting.Value.StartTime)) return;

            var guild = Constants.DiscordClient.GetGuild(Setting.Value.DiscordGuildId);
            var repo = ServiceProviderFactory.ServiceProvider.GetService<UserRepository>();
            if (repo == null)
            {
                Console.WriteLine("[CoolJob] repo가 null임.");
                return;
            }

            var users = repo.All();

            if (await CoolManager.Instance.FetchNewArticles())
            {
                if (CoolManager.Instance.Articles.Count == 0) return;

                foreach (var user in users)
                {
                    var channel = guild?.GetTextChannel(ulong.Parse(user.ChannelId));
                    if (channel != null)
                    {
                        var guildUser = guild.GetUser(ulong.Parse(user.UserId));
                        if (guildUser == null) continue;

                        foreach (var article in CoolManager.Instance.Articles.ToImmutableArray())
                        {
                            if (user.Keywords.Count > 0 && user.Keywords.Any(keyword => article.Title.ToLower().Contains(keyword.ToLower())))
                            {
                                var builder = new EmbedBuilder();

                                builder.WithColor(Color.LightGrey);
                                builder.WithCurrentTimestamp();
                                builder.WithUrl(article.Url);
                                builder.WithTitle(article.Title);

                                await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                            }
                        }
                    }
                }

                CoolManager.Instance.Articles.Clear();
            }

            //장터기능 (특정 유저 전용 기능)
            if (await CoolMarketManager.Instance.FetchNewArticles())
            {
                if (CoolMarketManager.Instance.Articles.Count == 0) return;

                var user = users.FirstOrDefault(x => x.UserId == "911514547129569300");
                if (user != null)
                {
                    var channel = guild?.GetTextChannel(ulong.Parse(user.ChannelId));
                    if (channel != null)
                    {
                        var guildUser = guild.GetUser(ulong.Parse(user.UserId));
                        if (guildUser != null)
                        {
                            foreach (var article in CoolMarketManager.Instance.Articles.ToImmutableArray())
                            {
                                if (user.Keywords.Count > 0 && user.Keywords.Any(keyword => article.Title.ToLower().Contains(keyword.ToLower())))
                                {
                                    var builder = new EmbedBuilder();

                                    builder.WithColor(Color.DarkGrey);
                                    builder.WithCurrentTimestamp();
                                    builder.WithUrl(article.Url);
                                    builder.WithTitle(article.Title);

                                    await channel.SendMessageAsync($"{guildUser.Mention}[장터] {article.Title}", false, builder.Build());
                                }
                            }
                        }
                    }

                    CoolMarketManager.Instance.Articles.Clear();
                }
            }
        }
    }
}