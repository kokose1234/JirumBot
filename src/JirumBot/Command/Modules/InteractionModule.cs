using Discord.Interactions;
using JirumBot.Data;
using JirumBot.Database.Repositories;
using Microsoft.Extensions.Options;

namespace JirumBot.Command.Modules;

public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly UserRepository _repository;
    private readonly DiscordSetting _config;

    public InteractionModule(IOptions<DiscordSetting> option, UserRepository repository)
    {
        _config = option.Value;
        _repository = repository;
    }

  
}