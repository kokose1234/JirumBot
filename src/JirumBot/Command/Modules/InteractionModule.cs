using Discord;
using Discord.Interactions;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using JirumBot.Services;
using Microsoft.Extensions.Options;

namespace JirumBot.Command.Modules;

public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly UserRepository _repository;
    private readonly DiscordSetting _config;
    private readonly SchedulerService _scheduler;

    public InteractionModule(IOptions<DiscordSetting> option, UserRepository repository, SchedulerService scheduler)
    {
        _config = option.Value;
        _repository = repository;
        _scheduler = scheduler;
    }

    [RequireRole(821294661548507147ul)]
    [UserCommand("채널 생성")]
    public async Task CreateChannel(IUser user)
    {
        var guild = Context.Guild;
        if (guild != null)
        {
            var users = await _repository.All();
            var categoryChannel = guild.GetCategoryChannel(_config.CategoryId);

            if (users.All(x => x.UserId != user.Id))
            {
                var builder = new EmbedBuilder();
                var channel = await guild.CreateTextChannelAsync($"{user.Username}-알림",
                    properties => properties.CategoryId = categoryChannel.Id, RequestOptions.Default);
                await _repository.Create(new() {UserId = user.Id, ChannelId = channel.Id, Keywords = new List<string>()});

                builder.WithThumbnailUrl("https://cdn.discordapp.com/app-icons/821300653136805928/8caa403394f55277b851bbd841cd8b7d.png?size=512");
                builder.WithColor(Color.Blue);
                builder.WithTitle("환영합니다");
                builder.WithDescription("이곳에 /명령어를 입력해 보세요");

                await channel.SendMessageAsync("", false, builder.Build());
                await channel.AddPermissionOverwriteAsync(user,
                    new OverwritePermissions(sendMessages: PermValue.Allow, viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow,
                        attachFiles: PermValue.Allow, embedLinks: PermValue.Allow));

                await RespondAsync("채널 생성 완료", ephemeral: true);
            }
        }
    }
}