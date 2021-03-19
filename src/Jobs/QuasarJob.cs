using System.Linq;
using Discord;
using FluentScheduler;
using JirumBot.Data;

namespace JirumBot.Jobs
{
    public class QuasarJob : IJob
    {
        public async void Execute()
        {
            var guild = Constants.DiscordClient.GetGuild(Setting.Value.DiscordGuildId);
            var channel = guild?.GetTextChannel(Setting.Value.DiscordChannelId);

            if (channel != null)
            {
                if (await Constants.QuasarJirumManager.GetNewArticle())
                {
                    if (Setting.Value.Keywords.Any(keyword => Constants.QuasarJirumManager.LatestArticle.Title.ToLower().Contains(keyword.ToLower())))
                    {
                        var builder = new EmbedBuilder();

                        builder.WithColor(new Color(255, 153, 0));
                        builder.WithCurrentTimestamp();
                        builder.WithUrl(Constants.QuasarJirumManager.LatestArticle.Url);
                        builder.WithTitle(Constants.QuasarJirumManager.LatestArticle.Title);
                        builder.WithDescription(Constants.QuasarJirumManager.LatestArticle.Title);
                        builder.WithThumbnailUrl(Constants.QuasarJirumManager.LatestArticle.ThumbnailUrl);

                        await channel.SendMessageAsync("", false, builder.Build());
                    }
                }

                if (await Constants.QuasarJirumManager2.GetNewArticle())
                {
                    if (Setting.Value.Keywords.Any(keyword =>
                        Constants.QuasarJirumManager2.LatestArticle.Title.ToLower().Contains(keyword.ToLower())))
                    {
                        var builder = new EmbedBuilder();

                        builder.WithColor(new Color(255, 153, 0));
                        builder.WithCurrentTimestamp();
                        builder.WithUrl(Constants.QuasarJirumManager2.LatestArticle.Url);
                        builder.WithTitle($"[장터] {Constants.QuasarJirumManager2.LatestArticle.Title}");
                        builder.WithDescription(Constants.QuasarJirumManager2.LatestArticle.Title);

                        await channel.SendMessageAsync("", false, builder.Build());
                    }
                }
            }
        }
    }
}