using System.Collections.Immutable;
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
    private readonly BotSetting _config;
    private readonly InteractionService _interaction;
    private readonly UserRepository _userRepository;

    public UserModule(IServiceProvider provider, IOptions<BotSetting> config, InteractionService interaction, UserRepository userRepository)
    {
        _provider = provider;
        _config = config.Value;
        _interaction = interaction;
        _userRepository = userRepository;
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
    [SlashCommand("목록", "검색 키워드 목록을 봅니다.")]
    public async Task ShowKeywords()
    {
        var user = _userRepository.GetById(Context.User.Id);
        if (user == null)
        {
            await RespondAsync("user가 null", ephemeral: true);
            return;
        }


        var keywords = user.Keywords.OrderBy(x => x).ToImmutableArray();
        if (keywords.Length == 0)
        {
            await RespondAsync("키워드가 등록되지 않았습니다", ephemeral: true);
            return;
        }

        await RespondAsync($"```{string.Join(", ", user.Keywords)}```", ephemeral: true);
    }

    [EnabledInDm(false)]
    [SlashCommand("추가", "검색 키워드를 추가합니다. 사용법: /추가 1080")]
    public async Task AddKeyword(string keyword)
    {
        if (keyword.Length < 2)
        {
            await RespondAsync("키워드는 최소 2글자 부터 등록 가능합니다.", ephemeral: true);
            return;
        }

        var user = _userRepository.GetById(Context.User.Id);
        if (user == null)
        {
            await RespondAsync("user가 null", ephemeral: true);
            return;
        }

        if (user.Keywords.Any(x => x == keyword))
        {
            await RespondAsync($"{keyword}는 이미 존재하는 키워드 입니다.", ephemeral: true);
            return;
        }

        _userRepository.AddKeyword(Context.User.Id, keyword);
        await RespondAsync($"{keyword} 추가 완료.", ephemeral: true);
    }

    [EnabledInDm(false)]
    [SlashCommand("제거", "검색 키워드를 삭제합니다. 사용법: /제거 1080")]
    public async Task RemoveKeyword(string keyword)
    {
        var user = _userRepository.GetById(Context.User.Id);
        if (user == null)
        {
            await RespondAsync("user가 null", ephemeral: true);
            return;
        }

        if (user.Keywords.All(x => x != keyword))
        {
            await RespondAsync($"{keyword}는 존재하지 않는 키워드 입니다.", ephemeral: true);
            return;
        }

        _userRepository.DeleteKeyword(Context.User.Id, keyword);
        await RespondAsync($"{keyword} 제거 완료.", ephemeral: true);
    }

    [EnabledInDm(false)]
    [SlashCommand("초기화", "검색 키워드를 초기화합니다.")]
    public async Task ClearKeyword()
    {
        var user = _userRepository.GetById(Context.User.Id);
        if (user == null)
        {
            await RespondAsync("user가 null", ephemeral: true);
            return;
        }

        _userRepository.ClearKeyword(Context.User.Id);
        await RespondAsync("키워드 초기화 완료.", ephemeral: true);
    }

    [EnabledInDm(false)]
    [SlashCommand("정보", "봇 정보를 확인합니다.")]
    public async Task SendInfo()
    {
        var obj = JObject.Parse(await File.ReadAllTextAsync("./JirumBot.deps.json"));

        if (obj != null)
        {
            var builder = new EmbedBuilder();
            builder.WithColor(Color.Blue);
            builder.WithTitle("봇 정보");
            builder.WithDescription("제작: 의문의 봇 장인, 아이디어: SpearDragon");
            builder.AddField("라이브러리 리스트", "봇 제작에 사용된 라이브러리 리스트 입니다.");
            foreach (JProperty lib in obj["targets"][".NETCoreApp,Version=v6.0"]["JirumBot/1.0.0"]["dependencies"])
            {
                builder.AddField(lib.Name, lib.Value);
            }

            await RespondAsync(embed: builder.Build(), ephemeral: true);
        }
    }
}