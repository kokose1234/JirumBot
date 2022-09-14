using System.Collections.Immutable;
using Discord;
using Discord.WebSocket;
using JirumBot.CrawlManagers;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using Microsoft.Extensions.Options;

namespace JirumBot.Services.Crawl
{
    public class ClienService : SimpleCrawlManager
    {
        private readonly DiscordSocketClient _discord;
        private readonly BotSetting _config;
        private readonly LoggingService _logging;
        private readonly UserRepository _userRepository;

        public ClienService(DiscordSocketClient discord, IOptions<BotSetting> config, LoggingService logging, UserRepository userRepository)
        {
            _discord = discord ?? throw new ArgumentNullException(nameof(discord));
            _logging = logging ?? throw new ArgumentNullException(nameof(logging));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _config = config.Value;
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

                    if(articles.Count == 0) return;

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

                                    builder.WithColor(35, 47, 62);
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
                var response = await _httpClient.GetStringAsync("https://www.clien.net/service/board/jirum");
                if (string.IsNullOrEmpty(response)) return false;
                _document.LoadHtml(response);

                var list = _document.DocumentNode.SelectNodes(_config.ClienBasePath);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        var title = node.FirstChild.InnerText.Trim();
                        var url = $"https://www.clien.net{node.GetAttributeValue("href", "(null)")}";

                        if (!url.Contains("(null)") && !_articleHistories.Contains(url))
                        {
                            Articles.Add(new() {Title = title, Url = url});
                            _articleHistories.Add(url);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logging.GetExceptionLogger().Error(ex, "클리앙 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}