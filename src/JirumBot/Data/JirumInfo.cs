using Newtonsoft.Json;

namespace JirumBot.Data;

[JsonObject(MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
public sealed record JirumInfo
{
    [JsonProperty("cool_articles")]
    public IList<Article> CoolArticles { get; init; } = new List<Article>();

    [JsonProperty("cool_market_articles")]
    public IList<Article> CoolMarketArticles { get; init; } = new List<Article>();

    [JsonProperty("city_articles")]
    public IList<Article> CityArticles { get; init; } = new List<Article>();

    [JsonProperty("quasar_articles")]
    public IList<Article> QuasarArticles { get; init; } = new List<Article>();

    [JsonProperty("ruli_articles")]
    public IList<Article> RuliArticles { get; init; } = new List<Article>();

    [JsonProperty("fm_articles")]
    public IList<Article> FmArticles { get; init; } = new List<Article>();

    [JsonProperty("ppom_articles")]
    public IList<Article> PpomArticles { get; init; } = new List<Article>();

    [JsonProperty("clien_articles")]
    public IList<Article> ClienArticles { get; init; } = new List<Article>();
}