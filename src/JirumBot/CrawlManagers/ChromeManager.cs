using HtmlAgilityPack;
using JirumBot.Data;
using OpenQA.Selenium.Chrome;

namespace JirumBot.CrawlManagers
{
    public abstract class ChromeManager : ICrawlManager
    {
        private readonly ChromeDriverService _driverService;
        private readonly ChromeOptions _options;
        protected readonly HtmlDocument _document = new();
        protected readonly HashSet<string> _articleHistories = new();

        public List<Article> Articles { get; } = new();
        public ChromeDriver Driver { get; }
        public bool IsStopped { get; protected set; }

        public ChromeManager()
        {
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;
            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");
            _options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
            _options.AddArgument("headless");
            Driver = new ChromeDriver(_driverService, _options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        }

        public abstract Task<bool> FetchNewArticles();
    }
}