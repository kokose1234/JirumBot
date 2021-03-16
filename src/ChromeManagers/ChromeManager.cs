using System;
using System.Threading.Tasks;
using JirumBot.Data;
using OpenQA.Selenium.Chrome;

namespace JirumBot.ChromeManagers
{
    public abstract class ChromeManager
    {
        private readonly ChromeDriverService driverService;
        private readonly ChromeOptions options;

        public ChromeDriver Driver { get; }
        public Article LatestArticle { get; protected set; }

        public ChromeManager()
        {
            driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            options = new ChromeOptions();
            options.AddArgument("disable-gpu");
            options.AddArgument("user-agent=Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_6)" +
                                "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
            options.AddArgument("headless");
            Driver = new ChromeDriver(driverService, options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        }

        public abstract Task<bool> Login(string returnUrl);
        public abstract Task<bool> GetNewArticle();
    }
}