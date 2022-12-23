using Discord;
using Discord.Commands;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using Microsoft.Extensions.Options;

namespace JirumBot.Command.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
public class AdminModule : ModuleBase<SocketCommandContext>
{
    private readonly UserRepository _repository;
    private readonly DiscordSetting _config;
    private readonly CommandService _command;

    public AdminModule(IOptions<DiscordSetting> option, UserRepository repository, CommandService command)
    {
        _config = option.Value;
        _repository = repository;
        _command = command;
    }

    [Command("명령어")]
    [Summary("도움말을 봅니다.")]
    public async Task SendHelp()
    {
        var commands = _command.Commands.Where(c => c.CheckPreconditionsAsync(Context).Result.IsSuccess & c.Name != "명령어");
        var builder = new EmbedBuilder();

        builder.WithThumbnailUrl("https://cdn.discordapp.com/app-icons/821300653136805928/8caa403394f55277b851bbd841cd8b7d.png?size=512");
        builder.WithColor(Color.Blue);
        builder.AddField("<:scroll:643684183293820998> 명령어 <:scroll:643684183293820998>", "사용하실 수 있는 명령어 목록입니다.");
        foreach (var command in commands)
        {
            builder.AddField("!" + command.Name, string.IsNullOrEmpty(command.Summary) ? "설명이 없습니다." : command.Summary, true);
        }

        await Context.Message.Channel.SendMessageAsync("", false, builder.Build());
    }


    [Command("청소", true)]
    [Summary("현재 채널을 청소합니다.")]
    public async Task ClearChat()
    {
        var messages = await Context.Channel.GetMessagesAsync(short.MaxValue).FlattenAsync();
        await ((ITextChannel) Context.Channel).DeleteMessagesAsync(messages.Where(x => !x.IsPinned));
    }
}