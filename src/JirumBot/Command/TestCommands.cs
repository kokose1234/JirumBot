using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using JirumBot.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace JirumBot.Command
{
    public class Testcommands : ModuleBase<SocketCommandContext>
    {
        [Command("유령삭제", true)]
        public async Task RemoveGhostChannel()
        {
            var userRepository = ServiceProviderFactory.ServiceProvider.GetService<UserRepository>();
            if (userRepository == null)
            {
                Console.WriteLine("userRepository가 null임");
                return;
            }

            var users = userRepository.All();
            var guild = Context.Guild;
            var ghostChannels = users
                                .Where(user => guild.GetUser(ulong.Parse(user.UserId)) == null)
                                .Select(user => guild.GetChannel(ulong.Parse(user.ChannelId)))
                                .ToImmutableList();

            if (ghostChannels.Count != 0)
            {
                ghostChannels.ForEach(async channel =>
                {
                    var user = userRepository.GetByChannelId(channel.Id);
                    if (user != null)
                    {
                        userRepository.Delete(user);
                    }

                    await channel.DeleteAsync();
                });
            }
        }

        // [Command("테스트", true)]
        // public async Task Test()
        // {
        //     if (await CoolMarketManager.Instance.FetchNewArticles())
        //     {
        //         var list = CoolMarketManager.Instance.Articles.ToImmutableArray();
        //         Console.WriteLine(list.Length);
        //
        //         foreach (var article in list)
        //         {
        //             Console.WriteLine($"{article.Title} : {article.Url}");
        //         }
        //     }
        // }
        //
        // [Command("테스트모드", true)]
        // public async Task TestMode()
        // {
        //     if (Context.User is SocketGuildUser socketGuildUser)
        //     {
        //         if (socketGuildUser.Roles.All(x => x.Id != Setting.Value.AdminRoleId) || Context.Channel.Id != Setting.Value.TestChannelId)
        //         {
        //             await Context.Message.DeleteAsync();
        //             return;
        //         }
        //     }
        //
        //     Constants.TestMode = true;
        //     await Context.Message.DeleteAsync();
        // }
    }
}