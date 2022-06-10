using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using JirumBot.Data;
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

        // [Command("루리웹테스트", true)]
        // public async Task TestRuliWeb()
        // {
        //     if (Context.User is SocketGuildUser socketGuildUser)
        //     {
        //         if (socketGuildUser.Roles.All(x => x.Id != Setting.Value.AdminRoleId) || Context.Channel.Id != Setting.Value.TestChannelId)
        //         {
        //             await Context.Message.DeleteAsync();
        //             return;
        //         }
        //
        //         var guild = Constants.DiscordClient.GetGuild(Setting.Value.DiscordGuildId);
        //         var repo = ServiceProviderFactory.ServiceProvider.GetService<UserRepository>();
        //         if (repo == null)
        //         {
        //             Console.WriteLine("[RuliJob] repo가 null임.");
        //             return;
        //         }
        //
        //         var users = repo.All();
        //
        //         if (await Constants.RuliJirumManager.FetchNewArticle())
        //         {
        //             foreach (var user in users)
        //             {
        //                 var channel = guild?.GetTextChannel(ulong.Parse(user.ChannelId));
        //                 if (channel != null)
        //                 {
        //                     try
        //                     {
        //                         if (user.Keywords.Any(keyword => !Constants.RuliJirumManager.LatestArticle.Title.ToLower().Contains(keyword.ToLower())))
        //                         {
        //                             var builder = new EmbedBuilder();
        //
        //                             builder.WithColor(26, 112, 220);
        //                             builder.WithCurrentTimestamp();
        //                             builder.WithUrl(Constants.RuliJirumManager.LatestArticle.Url);
        //                             builder.WithTitle(Constants.RuliJirumManager.LatestArticle.Title);
        //
        //                             await channel.SendMessageAsync($"[테스트] {Constants.RuliJirumManager.LatestArticle.Title}", false, builder.Build());
        //                         }
        //                     }
        //                     catch (Exception ex)
        //                     {
        //                         Console.WriteLine(ex);
        //                     }
        //                 }
        //             }
        //         }
        //     }
        // }

        [Command("테스트모드", true)]
        public async Task TestMode()
        {
            if (Context.User is SocketGuildUser socketGuildUser)
            {
                if (socketGuildUser.Roles.All(x => x.Id != Setting.Value.AdminRoleId) || Context.Channel.Id != Setting.Value.TestChannelId)
                {
                    await Context.Message.DeleteAsync();
                    return;
                }
            }

            Constants.TestMode = true;
            await Context.Message.DeleteAsync();
        }
    }
}