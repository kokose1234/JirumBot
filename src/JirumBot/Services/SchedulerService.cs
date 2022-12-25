using System.Collections.Concurrent;
using System.Collections.Immutable;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Discord;
using Discord.WebSocket;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace JirumBot.Services;

public class SchedulerService
{
    private const string QUEUE_URL = "https://sqs.ap-northeast-2.amazonaws.com/022030381686/JirumBot.fifo";

    private readonly DiscordSocketClient _discord;
    private readonly LoggingService _logging;
    private readonly DiscordSetting _config;
    private readonly UserRepository _userRepository;
    private readonly AmazonSQSClient _sqsClient;
    private readonly AsyncLock _mutex;

    private readonly ConcurrentBag<JirumInfo> _jirumInfoList;

    public SchedulerService(IOptions<DiscordSetting> option, DiscordSocketClient discord, LoggingService logging, UserRepository userRepository)
    {
        _config = option.Value;
        _discord = discord;
        _logging = logging;
        _userRepository = userRepository;
        _sqsClient = new(RegionEndpoint.APNortheast2);
        _mutex = new();
        _jirumInfoList = new();
    }

    public void Start()
    {
        new Thread(GhostCheck).Start();
        new Thread(SqsReceiver).Start();
        new Thread(DiscordMessageSender).Start();
    }

    private async void SqsReceiver()
    {
        while (true)
        {
            var receiveMessageResponse = await GetMessage();

            if (receiveMessageResponse.Messages.Count > 0)
            {
                using (await _mutex.LockAsync())
                {
                    _jirumInfoList.Clear();

                    foreach (var message in receiveMessageResponse.Messages)
                    {
                        try
                        {
                            var jirumInfo = JsonConvert.DeserializeObject<JirumInfo>(message.Body);
                            if (jirumInfo != null)
                            {
                                _jirumInfoList.Add(jirumInfo);
                            }
                        }
                        catch (JsonException ex)
                        {
                            _logging.GetExceptionLogger().Error(ex, "SQS에서 잘못된 메시지를 반환함.");
                            //ignored
                        }
                        finally
                        {
                            await DeleteMessage(message);
                        }
                    }
                }
            }

            await Task.Delay(10 * 1000);
        }
    }

    private async void DiscordMessageSender()
    {
        while (true)
        {
            using (await _mutex.LockAsync())
            {
                if (!_jirumInfoList.IsEmpty)
                {
                    var users = await _userRepository.All();
                    var guild = _discord.GetGuild(_config.GuildId);

                    if (guild != null)
                    {
                        foreach (var user in users)
                        {
                            if (user.Keywords.Count <= 0) continue;

                            var channel = guild.GetTextChannel(user.ChannelId);
                            var guildUser = guild.GetUser(user.UserId);
                            if (channel == null || guildUser == null) continue;

                            foreach (var jirumInfo in _jirumInfoList)
                            {
                                if (jirumInfo.CityArticles.Count > 0)
                                {
                                    foreach (var article in jirumInfo.ClienArticles)
                                    {
                                        if (!user.Keywords.Any(x => article.Title.Contains(x, StringComparison.CurrentCultureIgnoreCase))) continue;

                                        var builder = new EmbedBuilder();
                                        builder.WithColor(255, 168, 0);
                                        builder.WithCurrentTimestamp();
                                        builder.WithUrl(article.Url);
                                        builder.WithTitle(article.Title);

                                        await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                                    }
                                }

                                if (jirumInfo.ClienArticles.Count > 0)
                                {
                                    foreach (var article in jirumInfo.ClienArticles)
                                    {
                                        if (!user.Keywords.Any(x => article.Title.Contains(x, StringComparison.CurrentCultureIgnoreCase))) continue;

                                        var builder = new EmbedBuilder();
                                        builder.WithColor(35, 47, 62);
                                        builder.WithCurrentTimestamp();
                                        builder.WithUrl(article.Url);
                                        builder.WithTitle(article.Title);

                                        await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                                    }
                                }

                                if (jirumInfo.PpomArticles.Count > 0)
                                {
                                    foreach (var article in jirumInfo.PpomArticles)
                                    {
                                        if (!user.Keywords.Any(x => article.Title.Contains(x,  StringComparison.CurrentCultureIgnoreCase))) continue;

                                        var builder = new EmbedBuilder();
                                        builder.WithColor(Color.Blue);
                                        builder.WithCurrentTimestamp();
                                        builder.WithUrl(article.Url);
                                        builder.WithTitle(article.Title);

                                        await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                                    }
                                }

                                if (jirumInfo.CoolArticles.Count > 0)
                                {
                                    foreach (var article in jirumInfo.CoolArticles)
                                    {
                                        if (!user.Keywords.Any(x => article.Title.Contains(x, StringComparison.CurrentCultureIgnoreCase))) continue;

                                        var builder = new EmbedBuilder();
                                        builder.WithColor(Color.LightGrey);
                                        builder.WithCurrentTimestamp();
                                        builder.WithUrl(article.Url);
                                        builder.WithTitle(article.Title);

                                        await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                                    }
                                }

                                if (jirumInfo.CoolMarketArticles.Count > 0)
                                {
                                    if (user.UserId != 911514547129569300ul) continue;

                                    foreach (var article in jirumInfo.CoolMarketArticles)
                                    {
                                        if (!user.Keywords.Any(x => article.Title.Contains(x, StringComparison.CurrentCultureIgnoreCase))) continue;

                                        var builder = new EmbedBuilder();
                                        builder.WithColor(Color.DarkGrey);
                                        builder.WithCurrentTimestamp();
                                        builder.WithUrl(article.Url);
                                        builder.WithTitle(article.Title);

                                        await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                                    }
                                }

                                if (jirumInfo.FmArticles.Count > 0)
                                {
                                    foreach (var article in jirumInfo.FmArticles)
                                    {
                                        if (!user.Keywords.Any(x => article.Title.Contains(x, StringComparison.CurrentCultureIgnoreCase))) continue;

                                        var builder = new EmbedBuilder();
                                        builder.WithColor(135, 206, 235);
                                        builder.WithCurrentTimestamp();
                                        builder.WithUrl(article.Url);
                                        builder.WithTitle(article.Title);

                                        await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                                    }
                                }

                                if (jirumInfo.QuasarArticles.Count > 0)
                                {
                                    foreach (var article in jirumInfo.QuasarArticles)
                                    {
                                        if (!user.Keywords.Any(x => article.Title.Contains(x, StringComparison.CurrentCultureIgnoreCase))) continue;

                                        var builder = new EmbedBuilder();
                                        builder.WithColor(new Color(255, 153, 0));
                                        builder.WithCurrentTimestamp();
                                        builder.WithUrl(article.Url);
                                        builder.WithTitle(article.Title);

                                        await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                                    }
                                }

                                if (jirumInfo.RuliArticles.Count > 0)
                                {
                                    foreach (var article in jirumInfo.RuliArticles)
                                    {
                                        if (!user.Keywords.Any(x => article.Title.Contains(x, StringComparison.CurrentCultureIgnoreCase))) continue;

                                        var builder = new EmbedBuilder();
                                        builder.WithColor(26, 112, 220);
                                        builder.WithCurrentTimestamp();
                                        builder.WithUrl(article.Url);
                                        builder.WithTitle(article.Title);

                                        await channel.SendMessageAsync($"{guildUser.Mention}{article.Title}", false, builder.Build());
                                    }
                                }
                            }
                        }
                    }

                    _jirumInfoList.Clear();
                }
            }

            await Task.Delay(10 * 1000);
        }
    }

    private async void GhostCheck()
    {
        while (true)
        {
            await Task.Delay(5 * 1000);

            var guild = _discord.GetGuild(_config.GuildId);
            if (guild != null)
            {
                var users = await _userRepository.All();
                var categoryChannel = guild.GetCategoryChannel(_config.CategoryId);
                var normalUsers = guild.Users
                                       .Where(user => !user.IsBot)
                                       .ToImmutableList();
                var ghostUsers = normalUsers
                                 .Where(user => users.All(x => x.UserId != user.Id))
                                 .ToImmutableList();

                if (ghostUsers.Count != 0)
                {
                    foreach (var user in ghostUsers)
                    {
                        var builder = new EmbedBuilder();
                        var channel = await guild.CreateTextChannelAsync($"{user.Username}-알림",
                            properties => properties.CategoryId = categoryChannel.Id, RequestOptions.Default);
                        await _userRepository.Create(new() {UserId = user.Id, ChannelId = channel.Id, Keywords = new List<string>()});

                        builder.WithThumbnailUrl("https://cdn.discordapp.com/app-icons/821300653136805928/8caa403394f55277b851bbd841cd8b7d.png?size=512");
                        builder.WithColor(Color.Blue);
                        builder.WithTitle("환영합니다");
                        builder.WithDescription("이곳에 /명령어를 입력해 보세요");

                        await channel.SendMessageAsync("", false, builder.Build());
                        await channel.AddPermissionOverwriteAsync(user,
                            new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow,
                                attachFiles: PermValue.Allow, embedLinks: PermValue.Allow));
                    }
                }
            }
        }
    }

    private async Task<ReceiveMessageResponse> GetMessage()
    {
        return await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = QUEUE_URL,
            WaitTimeSeconds = 0,
            MaxNumberOfMessages = 10
        });
    }

    private async Task DeleteMessage(Message message)
    {
        await _sqsClient.DeleteMessageAsync(QUEUE_URL, message.ReceiptHandle);
    }
}