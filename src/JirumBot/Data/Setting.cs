using System;
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

        [JsonProperty("discordCategoryId")]
        public ulong DiscordCategoryId { get; init; }

        [JsonProperty("testChannelId")]
        public ulong TestChannelId { get; init; }

        [JsonProperty("adminRoleId")]
        public ulong AdminRoleId { get; init; }

        [JsonProperty("coolBasePath")]
        public string CoolBasePath { get; init; }

        [JsonProperty("quasarBasePath")]
        public string QuasarBasePath { get; init; }

        [JsonProperty("quasarTitlePath")]
        public string QuasarTitlePath { get; init; }

        [JsonProperty("quasarUrlPath")]
        public string QuasarUrlPath { get; init; }

        [JsonProperty("quasarStatusPath")]
        public string QuasarStatusPath { get; init; }

        [JsonProperty("ppomBasePath")]
        public string PpomBasePath { get; init; }

        [JsonProperty("ppomTitlePath")]
        public string PpomTitlePath { get; init; }

        [JsonProperty("fmBasePath")]
        public string FmBasePath { get; init; }

        [JsonProperty("clienBasePath")]
        public string ClienBasePath { get; init; }

        [JsonProperty("ruliBasePath")]
        public string RuliBasePath { get; init; }

        [JsonProperty("meecoBasePath")]
        public string MeecoBasePath { get; init; }

        [JsonProperty("meecoCategoryPath")]
        public string MeecoCategoryPath { get; init; }

        [JsonProperty("meecoTitlePath")]
        public string MeecoTitlePath { get; init; }

        [JsonProperty("meecoUrlPath")]
        public string MeecoUrlPath { get; init; }

        [JsonProperty("refreshInterval")]
        public int RefreshInterval { get; init; }

        [JsonProperty("ghostCheckInterval")]
        public int GhostCheckInterval { get; init; }

        [JsonProperty("startTime")]
        public TimeSpan StartTime { get; init; }

        [JsonProperty("stopTime")]
        public TimeSpan StopTime { get; init; }

        public static void Save() => File.WriteAllText("./Setting.json", JsonConvert.SerializeObject(Value, Formatting.Indented));
    }
}