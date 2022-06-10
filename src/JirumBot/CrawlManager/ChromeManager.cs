using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JirumBot.Data;
using OpenQA.Selenium.Chrome;

namespace JirumBot.CrawlManager
{
    public abstract class ChromeManager : ICrawlManager
    {
        private readonly ChromeDriverService _driverService;
        private readonly ChromeOptions _options;
        protected readonly HtmlDocument Document = new();
        protected readonly HashSet<string> ArticleHistories = new();

        public List<Article> Articles { get; } = new();
        public ChromeDriver Driver { get; }
        public bool IsStopped { get; protected set; }

        public ChromeManager()
        {
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;
            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");
            _options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 Edg/100.0.1185.44");
            _options.AddArgument("headless");
            Driver = new ChromeDriver(_driverService, _options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        }

        public abstract Task<bool> FetchNewArticles();
    }
}