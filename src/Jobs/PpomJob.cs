using System.Linq;
using Discord;
using FluentScheduler;
using JirumBot.Data;

namespace JirumBot.Jobs
{
    public class PpomJob : IJob
    {
        public async void Execute()
        {
            var guild = Constants.DiscordClient.GetGuild(Setting.Value.DiscordGuildId);
            var channel = guild?.GetTextChannel(Setting.Value.DiscordChannelId);

            if (channel != null)
            {
                if (await Constants.PpomJirumManager.GetNewArticle())
                {
                    if (Setting.Value.Keywords.Any(keyword => Constants.PpomJirumManager.LatestArticle.Title.ToLower().Contains(keyword.ToLower())))
                    {
                        var builder = new EmbedBuilder();

                        builder.WithColor(Color.Blue);
                        builder.WithCurrentTimestamp();
                        builder.WithUrl(Constants.PpomJirumManager.LatestArticle.Url);
                        builder.WithTitle(Constants.PpomJirumManager.LatestArticle.Title);
                        builder.WithDescription(Constants.PpomJirumManager.LatestArticle.Title);
                        builder.WithThumbnailUrl(Constants.PpomJirumManager.LatestArticle.ThumbnailUrl);

                        await channel.SendMessageAsync("", false, builder.Build());
                    }
                }

                if (await Constants.PpomJirumManager2.GetNewArticle())
                {
                    if (Setting.Value.Keywords.Any(keyword =>
                        Constants.PpomJirumManager2.LatestArticle.Title.ToLower().Contains(keyword.ToLower())))
                    {
                        var builder = new EmbedBuilder();

                        builder.WithColor(Color.Blue);
                        builder.WithCurrentTimestamp();
                        builder.WithUrl(Constants.PpomJirumManager2.LatestArticle.Url);
                        builder.WithTitle($"[장터] {Constants.PpomJirumManager2.LatestArticle.Title}");
                        builder.WithDescription(Constants.PpomJirumManager2.LatestArticle.Title);

                        if (Constants.PpomJirumManager2.LatestArticle.ThumbnailUrl != "")
                            builder.WithThumbnailUrl(Constants.PpomJirumManager2.LatestArticle.ThumbnailUrl);

                        await channel.SendMessageAsync("", false, builder.Build());
                    }
                }
            }
        }
    }
}