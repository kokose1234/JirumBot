using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using JirumBot.Data;
using JirumBot.Tools;
using Microsoft.Extensions.Options;
using IResult = Discord.Interactions.IResult;

namespace JirumBot.Services;

public class StartupService
{
    private readonly IServiceProvider _provider;
    private readonly DiscordSocketClient _discord;
    private readonly CommandService _commands;
    private readonly InteractionService _interaction;
    private readonly LoggingService _logging;
    private readonly DiscordSetting _config;

    public StartupService(IOptions<DiscordSetting> option,
                          IServiceProvider provider,
                          DiscordSocketClient discord,
                          CommandService commands,
                          InteractionService interaction,
                          LoggingService logging)
    {
        _provider = provider;
        _config = option.Value;
        _discord = discord;
        _commands = commands;
        _interaction = interaction;
        _logging = logging;
    }

    public async Task StartAsync()
    {
        await _discord.LoginAsync(TokenType.Bot, _config.Token);
        await _discord.StartAsync();
#if DEBUG
        await _discord.SetStatusAsync(UserStatus.DoNotDisturb);
        await _discord.SetGameAsync("돈 쓸 곳 찾을까 말까");
#else
        await _discord.SetStatusAsync(UserStatus.Online);
                await _discord.SetGameAsync("돈 쓸 곳 찾기");
#endif

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

        _discord.Ready += DiscordOnReady;
        _discord.MessagesBulkDeleted += (_, _) => Task.CompletedTask;
        _discord.InteractionCreated += DiscordOnInteractionCreated;
        _discord.MessageReceived += ClientOnMessageReceived;
        _commands.CommandExecuted += CommandsOnCommandExecuted;
        _interaction.SlashCommandExecuted += InteractionOnSlashCommandExecuted;
    }


    private async Task InteractionOnSlashCommandExecuted(SlashCommandInfo command, IInteractionContext ctx, IResult result)
    {
        if (result.IsSuccess)
        {
            if (command.Name != "명령어")
            {
                _logging.GetCommandLogger().Info($"{Utils.GetFullUserName((SocketUser) ctx.User)} : /{command.Name}");
            }

            return;
        }

        try
        {
            await ctx.Interaction.DeferAsync(true);
        }
        catch
        {
            // ignore
        }

        if (result.Error != null)
        {
            switch (result.Error)
            {
                case InteractionCommandError.UnknownCommand:
                    await ctx.Interaction.FollowupAsync("알 수 없는 명령어입니다. /명령어를 입력하여 사용 가능한 명령어를 알아보세요!");
                    break;
                case InteractionCommandError.BadArgs:
                    await ctx.Interaction.FollowupAsync("사용법이 틀렸습니다. 명령어 인수를 확인해 주세요.");
                    break;
                case InteractionCommandError.Unsuccessful:
                    await ctx.Interaction.FollowupAsync("명령어 실행에 실패했습니다.");
                    break;
                case InteractionCommandError.UnmetPrecondition:
                    await ctx.Interaction.FollowupAsync("권한이 충분하지 않습니다.");
                    break;
                case InteractionCommandError.ParseFailed:
                    await ctx.Interaction.FollowupAsync("명령어 분석 실패.");
                    break;
                default:
                    await ctx.Interaction.FollowupAsync(result.ErrorReason);
                    break;
            }
        }
    }


    private async Task DiscordOnReady()
    {
        if (File.Exists("register-command.txt"))
        {
            await _discord.Rest.DeleteAllGlobalCommandsAsync();
            await _interaction.RegisterCommandsGloballyAsync();
        }

        _discord.PurgeUserCache();
        await _discord.DownloadUsersAsync(new[] {_discord.GetGuild(_config.GuildId)});
    }

    private async Task DiscordOnInteractionCreated(SocketInteraction arg)
    {
        var ctx = new SocketInteractionContext(_discord, arg);
        await _interaction.ExecuteCommandAsync(ctx, _provider);
    }

    private async Task ClientOnMessageReceived(SocketMessage rawMessage)
    {
        if (rawMessage is not SocketUserMessage {Source: MessageSource.User} message) return;
        if (message.Channel.Id == _config.TalkChannelId && message.Content.StartsWith('!'))
        {
            await message.DeleteAsync();
            return;
        }

        var context = new SocketCommandContext(_discord, message);
        var argPos = 0;

        if (!(message.HasMentionPrefix(_discord.CurrentUser, ref argPos) || message.HasCharPrefix('!', ref argPos))) return;

        await _commands.ExecuteAsync(context, argPos, _provider);
        await message.DeleteAsync();
    }


    private async Task CommandsOnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, Discord.Commands.IResult result)
    {
        if (!command.IsSpecified)
        {
            await context.Message.ReplyAsync("존재하지 않는 명령어 입니다. !명령어를 입력하여 사용 가능한 명령어를 알아보세요!");
            return;
        }

        if (result.IsSuccess)
        {
            _logging.GetCommandLogger().Info($"{Utils.GetFullUserName(context.Message.Author as SocketUser)} : {context.Message.Content}");
            return;
        }

        if (result.Error != null)
        {
            switch (result.Error)
            {
                case CommandError.UnknownCommand:
                    await context.Channel.SendMessageAsync("알 수 없는 명령어입니다. !명령어를 입력하여 사용 가능한 명령어를 알아보세요!");
                    break;
                case CommandError.ParseFailed:
                    await context.Channel.SendMessageAsync("명령어 분석 실패.");
                    break;
                case CommandError.BadArgCount:
                    await context.Channel.SendMessageAsync($"사용법이 틀렸습니다. 명령어 인수를 확인해 주세요.\r\n{command.Value.Summary}");
                    break;
                case CommandError.MultipleMatches:
                    await context.Channel.SendMessageAsync("중복 명령어가 발견되었습니다.");
                    break;
                case CommandError.UnmetPrecondition:
                    await context.Channel.SendMessageAsync("권한이 충분하지 않습니다.");
                    break;
                case CommandError.Unsuccessful:
                    await context.Channel.SendMessageAsync("명령어 실행에 실패했습니다.");
                    break;
                default:
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                    break;
            }
        }
    }
}