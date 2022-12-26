namespace JirumBot.Data;

public sealed record DiscordSetting
{
    public string Token { get; init; } = string.Empty;
    public ulong GuildId { get; init; }
    public ulong CategoryId { get; init; }
    public ulong AdminRoleId { get; init; }
    public ulong TalkChannelId { get; init; }
}