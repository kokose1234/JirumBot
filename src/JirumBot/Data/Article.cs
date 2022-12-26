using Newtonsoft.Json;

namespace JirumBot.Data;

[JsonObject(MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
public sealed record Article
{
    [JsonProperty("title")]
    public string Title { get; init; } = "";

    [JsonProperty("url")]
    public string Url { get; init; } = "";
}