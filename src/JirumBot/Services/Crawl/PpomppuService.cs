using System.Collections.Immutable;
using Discord;
using Discord.WebSocket;
using JirumBot.CrawlManagers;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using JirumBot.Tools;
using Microsoft.Extensions.Options;

namespace JirumBot.Services.Crawl
{
    public class PpomppuService : ChromeManager
    {
        private readonly DiscordSocketClient _discord;
        private readonly BotSetting _config;
        private readonly LoggingService _logging;
        private readonly UserRepository _userRepository;

        public PpomppuService(DiscordSocketClient discord, IOptions<BotSetting> config, LoggingService logging, UserRepository userRepository)
        {
            _discord = discord;
            _config = config.Value;
            _logging = logging;
            _userRepository = userRepository;

            Driver.Navigate().GoToUrl("https://www.ppomppu.co.kr/zboard/zboard.php?id=ppomppu");
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

                                    builder.WithColor(Color.Blue);
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

        public override async Task<bool> FetchNewArticles()
        {
            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);
                _document.LoadHtml(Driver.PageSource);

                var list = _document.DocumentNode.SelectNodes(_config.PpomBasePath);
                foreach (var node in list)
                {
                    if (node != null)
                    {
                        if (!node.InnerHtml.Contains("line-through"))
                        {
                            var title = node.SelectSingleNode(_config.PpomTitlePath).InnerText.Trim();
                            var url = $"https://www.ppomppu.co.kr/zboard{node.GetAttributeValue("href", "(null)").Replace("amp;", "")}";

                            if (url != "(null)" && !_articleHistories.Contains(url))
                            {
                                Articles.Add(new() {Title = title, Url = url});
                                _articleHistories.Add(url);
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logging.GetExceptionLogger().Error(ex, "뽐뿌 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}