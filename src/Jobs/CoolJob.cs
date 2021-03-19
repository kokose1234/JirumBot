using System.Linq;
using Discord;
using FluentScheduler;
using JirumBot.Data;

namespace JirumBot.Jobs
{
    public class CoolJob : IJob
    {
        public async void Execute()
        {
            var guild = Constants.DiscordClient.GetGuild(Setting.Value.DiscordGuildId);
            var channel = guild?.GetTextChannel(Setting.Value.DiscordChannelId);

            if (channel != null)
            {
                if (await Constants.CoolJirumManager.GetNewArticle())
                {
                    if (Setting.Value.Keywords.Any(keyword => Constants.CoolJirumManager.LatestArticle.Title.ToLower().Contains(keyword.ToLower())))
                    {
                        var builder = new EmbedBuilder();

                        builder.WithColor(Color.LightGrey);
                        builder.WithCurrentTimestamp();
                        builder.WithUrl(Constants.CoolJirumManager.LatestArticle.Url);
                        builder.WithTitle(Constants.CoolJirumManager.LatestArticle.Title);
                        builder.WithDescription(Constants.CoolJirumManager.LatestArticle.Title);

                        await channel.SendMessageAsync("", false, builder.Build());
                    }
                }

                if (await Constants.CoolJirumManager2.GetNewArticle())
                {
                    if (Setting.Value.Keywords.Any(keyword => Constants.CoolJirumManager2.LatestArticle.Title.ToLower().Contains(keyword.ToLower())))
                    {
                        var builder = new EmbedBuilder();

                        builder.WithColor(Color.LightGrey);
                        builder.WithCurrentTimestamp();
                        builder.WithUrl(Constants.CoolJirumManager2.LatestArticle.Url);
                        builder.WithTitle($"[장터] {Constants.CoolJirumManager2.LatestArticle.Title}");
                        builder.WithDescription(Constants.CoolJirumManager2.LatestArticle.Title);

                        await channel.SendMessageAsync("", false, builder.Build());
                    }
                }
            }
        }
    }
}