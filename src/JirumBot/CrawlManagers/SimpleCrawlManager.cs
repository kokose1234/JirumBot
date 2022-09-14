using HtmlAgilityPack;
using JirumBot.Data;

namespace JirumBot.CrawlManagers;

public abstract class SimpleCrawlManager : ICrawlManager, IDisposable
{
    protected readonly HttpClient _httpClient = new();
    protected readonly HtmlDocument _document = new();
    protected readonly HashSet<string> _articleHistories = new();

    public List<Article> Articles { get; } = new();
    public bool IsStopped { get; protected set; }


    protected SimpleCrawlManager()
    {
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
    }

    protected abstract Task<bool> FetchNewArticles();

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}