using System;
using System.Collections.Immutable;
using System.IO;
using Newtonsoft.Json;

namespace JirumBot.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed record Setting
    {
        private static readonly Lazy<Setting> Lazy = new(JsonConvert.DeserializeObject<Setting>(File.ReadAllText("./Setting.json")));

        public static Setting Value => Lazy.Value;

        [JsonProperty("discordBotToken")]
        public string DiscordBotToken { get; init; }
        [JsonProperty("discordGuildId")]
        public ulong DiscordGuildId { get; init; }
        [JsonProperty("discordChannelId")]
        public ulong DiscordChannelId { get; init; }
        [JsonProperty("coolId")]
        public string CoolId { get; init; }
        [JsonProperty("coolPassWord")]
        public string CoolPassWord { get; init; }
        [JsonProperty("coolJirumTitlePath")]
        public string CoolJirumTitlePath { get; init; }
        [JsonProperty("coolJirumUrlPath")]
        public string CoolJirumUrlPath { get; init; }
        [JsonProperty("coolJirumTitlePath2")]
        public string CoolJirumTitlePath2 { get; init; }
        [JsonProperty("coolJirumUrlPath2")]
        public string CoolJirumUrlPath2 { get; init; }
        [JsonProperty("quasarId")]
        public string QuasarId { get; init; }
        [JsonProperty("quasarPassWord")]
        public string QuasarPassWord { get; init; }
        [JsonProperty("quasarJirumTitlePath")]
        public string QuasarJirumTitlePath { get; init; }
        [JsonProperty("quasarJirumUrlPath")]
        public string QuasarJirumUrlPath { get; init; }
        [JsonProperty("quasarJirumThumbnailUrlPath")]
        public string QuasarJirumThumbnailUrlPath { get; init; }
        [JsonProperty("quasarJirumTitlePath2")]
        public string QuasarJirumTitlePath2 { get; init; }
        [JsonProperty("quasarJirumUrlPath2")]
        public string QuasarJirumUrlPath2 { get; init; }
        [JsonProperty("quasarJirumThumbnailUrlPath2")]
        public string QuasarJirumThumbnailUrlPath2 { get; init; }
        [JsonProperty("refreshInterval")]
        public int RefreshInterval { get; init; }
        [JsonProperty("keywords")]
        public ImmutableList<string> Keywords { get; init; }
    }
}