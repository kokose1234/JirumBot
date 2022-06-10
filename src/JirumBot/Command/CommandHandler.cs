using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.Command
{
    public class CommandHandler
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _commands.CommandExecuted += CommandExecutedAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync() => await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (rawMessage is not SocketUserMessage { Source: MessageSource.User } message) return;

            var context = new SocketCommandContext(_client, message);
            var argPos = 0;

            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
                  message.HasCharPrefix('/', ref argPos))) return;

            if (!context.Channel.Name.Contains("-알림") && context.Channel.Id != Setting.Value.TestChannelId)
            {
                await message.DeleteAsync();
                return;
            }

            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (context == null) return;

            if (!command.IsSpecified)
            {
                await context.Message.ReplyAsync("존재하지 않는 명령어 입니다. /?를 입력하여 사용 가능한 명령어를 알아보세요!");
                return;
            }

            if (result.IsSuccess)
            {
                if (command.Value.Name != "?")
                    Constants.Logger.GetCommandLogger().Info($"{GetFullUserName((SocketUser)context?.Message.Author)} : {context.Message.Content}");

                return;
            }

            if (result.Error != null)
            {
                switch (result.Error)
                {
                    case CommandError.UnknownCommand:
                        await context.Message.ReplyAsync("알 수 없는 명령어입니다.");
                        break;
                    case CommandError.ParseFailed:
                        await context.Message.ReplyAsync("명령어 분석 실패.");
                        break;
                    case CommandError.BadArgCount:
                        await context.Message.ReplyAsync($"사용법이 틀렸습니다. 명령어 인수 개수를 확인해 주세요.\r\n{command.Value.Summary}");
                        break;
                    case CommandError.MultipleMatches:
                        await context.Message.ReplyAsync("중복 명령어가 발견되었습니다.");
                        break;
                    case CommandError.UnmetPrecondition:
                        await context.Message.ReplyAsync("권한이 충분하지 않습니다.");
                        break;
                    case CommandError.Unsuccessful:
                        await context.Message.ReplyAsync("명령어 실행에 실패했습니다.");
                        break;
                    default:
                        await context.Message.ReplyAsync(result.ErrorReason);
                        break;
                }
            }
        }

        private static string GetFullUserName(IUser usr)
        {
            var sb = new StringBuilder(usr.Username);
            sb.Append("#").Append(usr.DiscriminatorValue.ToString("D4"));
            return sb.ToString();
        }
    }
}