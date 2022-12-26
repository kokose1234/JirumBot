using Discord;
using Discord.Interactions;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace JirumBot.Command.Modules;

public class UserModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IServiceProvider _provider;
    private readonly UserRepository _repository;
    private readonly DiscordSetting _config;
    private readonly InteractionService _interaction;

    public UserModule(IOptions<DiscordSetting> option, UserRepository repository, InteractionService interaction, IServiceProvider provider)
    {
        _config = option.Value;
        _repository = repository;
        _interaction = interaction;
        _provider = provider;
    }

    [EnabledInDm(false)]
    [SlashCommand("명령어", "사용 가능한 명령어를 봅니다.")]
    public async Task SendHelp()
    {
        var commands = _interaction.SlashCommands.Where(c => c.CheckPreconditionsAsync(Context, _provider).Result.IsSuccess && c.Name != "명령어");
        var builder = new EmbedBuilder();

        builder.WithThumbnailUrl("https://cdn.discordapp.com/app-icons/821300653136805928/8caa403394f55277b851bbd841cd8b7d.png?size=512");
        builder.WithColor(Color.Blue);
        builder.AddField("<:scroll:643684183293820998> 명령어 <:scroll:643684183293820998>", "사용하실 수 있는 명령어 목록입니다.");
        foreach (var command in commands)
        {
            builder.AddField("/" + command.Name, string.IsNullOrEmpty(command.Description) ? "설명이 없습니다." : command.Description, true);
        }

        await RespondAsync(embed: builder.Build(), ephemeral: true);
    }

    [EnabledInDm(false)]
    [SlashCommand("추가", "특가 알림 키워드를 추가합니다.")]
    public async Task AddKeyword([Summary("키워드", ",를 추가하여 여러개의 키워드를 추가할 수 있습니다.")] string input)
    {
        var user = await _repository.GetByUserId(Context.User.Id);
        if (user.ChannelId != Context.Channel.Id)
        {
            await RespondAsync("해당 명령어는 알림 채널에서만 사용 가능합니다.", ephemeral: true);
            return;
        }

        var keywords = input.Split(',').Select(x => x.Trim()).ToArray();
        var added = new List<string>();

        foreach (var keyword in keywords)
        {
            if (!user.Keywords.Contains(keyword) && keyword.Length >= 2)
            {
                user.Keywords.Add(keyword);
                added.Add(keyword);
            }
        }

        await _repository.ReplaceOneAsync(user);

        await RespondAsync($"{string.Join(", ", added)} 추가 완료.", ephemeral: true);
    }

    [EnabledInDm(false)]
    [SlashCommand("목록", "특가 알림 키워드를 표시합니다.")]
    public async Task ListKeyword()
    {
        var user = await _repository.GetByUserId(Context.User.Id);
        if (user.ChannelId != Context.Channel.Id)
        {
            await RespondAsync("해당 명령어는 알림 채널에서만 사용 가능합니다.", ephemeral: true);
            return;
        }

        var keywords = user.Keywords.OrderBy(x => x).ToArray();
        if (keywords.Length == 0)
        {
            await RespondAsync("키워드가 등록되지 않았습니다.", ephemeral: true);
            return;
        }

        await RespondAsync($"```{string.Join(", ", user.Keywords)}```", ephemeral: true);
    }

    [EnabledInDm(false)]
    [SlashCommand("제거", "특가 알림 키워드를 삭제합니다.")]
    public async Task RemoveKeyword([Summary("키워드", ",를 추가하여 여러개의 키워드를 제거할 수 있습니다.")] string input)
    {
        var user = await _repository.GetByUserId(Context.User.Id);
        if (user.ChannelId != Context.Channel.Id)
        {
            await RespondAsync("해당 명령어는 알림 채널에서만 사용 가능합니다.", ephemeral: true);
            return;
        }

        var keywords = input.Split(',').Select(x => x.Trim()).ToArray();
        var removed = new List<string>();

        foreach (var keyword in keywords)
        {
            if (user.Keywords.Contains(keyword))
            {
                user.Keywords.Remove(keyword);
                removed.Add(keyword);
            }
        }

        await _repository.ReplaceOneAsync(user);

        await RespondAsync($"{string.Join(", ", removed)} 제거 완료.", ephemeral: true);
    }

    [EnabledInDm(false)]
    [SlashCommand("키워드초기화", "특가 알림 키워드를 초기화합니다.")]
    public async Task ClearKeyword()
    {
        var user = await _repository.GetByUserId(Context.User.Id);
        if (user.ChannelId != Context.Channel.Id)
        {
            await RespondAsync("해당 명령어는 알림 채널에서만 사용 가능합니다.", ephemeral: true);
            return;
        }

        user.Keywords.Clear();
        await _repository.ReplaceOneAsync(user);

        await RespondAsync("키워드 초기화 완료.", ephemeral: true);
    }

    [EnabledInDm(true)]
    [SlashCommand("정보", "봇 정보를 확인합니다.")]
    public async Task Info()
    {
        var obj = JObject.Parse(await File.ReadAllTextAsync("./JirumBot.deps.json"));

        if (obj != null)
        {
            var builder = new EmbedBuilder();
            builder.WithColor(Color.Blue);
            builder.WithTitle("봇 정보");
            builder.WithDescription("제작: 의문의 봇 장인, 아이디어: SpearDragon");
            builder.AddField("라이브러리 리스트", "봇 제작에 사용된 라이브러리 리스트 입니다.");
            foreach (JProperty lib in obj["targets"][".NETCoreApp,Version=v7.0"]["JirumBot/1.0.0"]["dependencies"])
            {
                builder.AddField(lib.Name, lib.Value);
            }

            await RespondAsync(embed: builder.Build(), ephemeral: true);
        }
    }
}