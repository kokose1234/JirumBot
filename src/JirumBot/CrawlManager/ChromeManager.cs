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
        protected readonly HtmlDocument _document = new();
        protected readonly HashSet<string> _articleHistories = new();

        public List<Article> Articles { get; } = new();
        public ChromeDriver Driver { get; }

        public ChromeManager()
        {
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            var options = new ChromeOptions();
            options.AddArgument("disable-gpu");
            options.AddArgument("disable-dev-shm-usage");
            options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36");
            options.AddArgument("headless");
            Driver = new ChromeDriver(driverService, options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        }

        public abstract Task<bool> FetchNewArticles();
    }
}