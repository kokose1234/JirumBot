using System;
using System.Linq;
using System.Threading.Tasks;
using JirumBot.Data;
using OpenQA.Selenium;

namespace JirumBot.CrawlManager;

public class CoolMarketManager : ChromeManager
{
    private static CoolMarketManager s_instance;
    public static CoolMarketManager Instance => s_instance ??= new CoolMarketManager();

    public CoolMarketManager()
    {
        Driver.Navigate().GoToUrl("https://coolenjoy.net/bbs/mart2?sca=%ED%8C%90%EB%A7%A4");
        Task.Delay(500).Wait();

        var id = Driver.FindElement(By.XPath(Setting.Value.CoolIdPath));
        var pw = Driver.FindElement(By.XPath(Setting.Value.CoolPasswordPath));
        var button = Driver.FindElement(By.XPath(Setting.Value.CoolLoginPath));

        if (id != null && pw != null && button != null)
        {
            id.SendKeys(Setting.Value.CoolId);
            pw.SendKeys(Setting.Value.CoolPassword);
            button.Click();
        }

        Task.Delay(500).Wait();
    }

    public override async Task<bool> FetchNewArticles()
    {
        try
        {
            Driver.Navigate().Refresh();
            await Task.Delay(500);
            _document.LoadHtml(Driver.PageSource);

            var list = _document.DocumentNode.SelectNodes(Setting.Value.CoolMarketBasePath);

            foreach (var node in list)
            {
                if (node != null)
                {
                    var title = node.SelectSingleNode(Setting.Value.CoolMarketTitlePath).InnerText.Replace("댓글", "").Trim();
                    //var price = node.SelectSingleNode(Setting.Value.CoolMarketPricePath).InnerText.Trim();
                    var url = node.SelectSingleNode(Setting.Value.CoolMarketTitlePath).GetAttributeValue("href", "(null)").Replace("amp;", "");

                    if (url != "(null)" && !_articleHistories.Contains(url))
                    {
                        Articles.Add(new() {Title = title, Url = url});
                        _articleHistories.Add(url);
                    }
                }
            }

            FixLinks();

            return true;
        }
        catch (Exception ex)
        {
            Constants.Logger.GetExceptionLogger().Error(ex, "쿨엔조이 장터 새로고침 중 오류 발생");
            return false;
        }
    }
}