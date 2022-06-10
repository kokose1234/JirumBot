using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JirumBot.Data;

namespace JirumBot.CrawlManager;

public abstract class SimpleCrawlManager : ICrawlManager, IDisposable
{
    protected readonly HttpClient HttpClient = new();
    protected readonly HtmlDocument Document = new();
    protected readonly HashSet<string> ArticleHistories = new();

    public List<Article> Articles { get; } = new();
    public bool IsStopped { get; protected set; }


    protected SimpleCrawlManager()
    {
        HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 Edg/100.0.1185.44");
    }

    public abstract Task<bool> FetchNewArticles();

    public void Dispose()
    {
        HttpClient?.Dispose();
    }
}