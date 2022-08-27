using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JirumBot.Data;

namespace JirumBot.CrawlManager;

public abstract class SimpleCrawlManager : ICrawlManager, IDisposable
{
    protected readonly HttpClient httpClient = new();
    protected readonly HtmlDocument document = new();
    protected readonly HashSet<string> articleHistories = new();

    public List<Article> Articles { get; } = new();
    public bool IsStopped { get; protected set; }


    protected SimpleCrawlManager()
    {
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.5060.66 Safari/537.36 Edg/103.0.1264.44");
    }

    public abstract Task<bool> FetchNewArticles();

    public void Dispose()
    {
        httpClient?.Dispose();
    }
}