﻿using System.Collections.Immutable;
using Discord;
using Discord.WebSocket;
using JirumBot.CrawlManagers;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using JirumBot.Tools;
using Microsoft.Extensions.Options;

namespace JirumBot.Services.Crawl
{
    public class FmService : SimpleCrawlManager
    {
        private readonly DiscordSocketClient _discord;
        private readonly BotSetting _config;
        private readonly LoggingService _logging;
        private readonly UserRepository _userRepository;

        public FmService(DiscordSocketClient discord, IOptions<BotSetting> config, LoggingService logging, UserRepository userRepository)
        {
            _discord = discord;
            _config = config.Value;
            _logging = logging;
            _userRepository = userRepository;
        }

        public async Task MainTask()
        {
            while (true)
            {
                if (await FetchNewArticles())
                {
                    var guild = _discord.GetGuild(_config.DiscordGuildId);
                    var users = _userRepository.All();
                    var articles = Articles.ToImmutableList();

                    if (articles.Count == 0) return;

                    foreach (var user in users)
                    {
                        var channel = guild?.GetTextChannel(ulong.Parse(user.ChannelId));
                        if (channel != null)
                        {
                            var guildUser = guild.GetUser(ulong.Parse(user.UserId));
                            if (guildUser == null) continue;

                            foreach (var article in articles)
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

                    Articles.Clear();
                }

                await Task.Delay(_config.RefreshInterval * 1000);
            }
        }

        protected override async Task<bool> FetchNewArticles()
        {
            if (IsStopped) return false;

            try
            {
                var response = await _httpClient.GetStringAsync("https://www.fmkorea.com/index.php?mid=hotdeal&listStyle=list&page=1");
                if (string.IsNullOrEmpty(response)) return false;
                _document.LoadHtml(response);

                var list = _document.DocumentNode.SelectNodes(_config.FmBasePath);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        if (!node.InnerText.Contains("종료됨"))
                        {
                            var title = node.InnerText.Trim();
                            var url = $"https://www.fmkorea.com{node.GetAttributeValue("href", "(null)").Replace("&amp;", "&")}";

                            if (!url.Contains("(null)") && !_articleHistories.Contains(url))
                            {
                                Articles.Add(new() {Title = title, Url = url});
                                _articleHistories.Add(url);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logging.GetExceptionLogger().Error(ex, "에펨코리아 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}