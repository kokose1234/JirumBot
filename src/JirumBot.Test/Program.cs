using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;

namespace JirumBot.Lab;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //
        // using var wc = new HttpClient();
        // wc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 Edg/100.0.1185.44");
        //
        // var text = await wc.GetStringAsync("https://meeco.kr/PricePlus");
        // var document = new HtmlDocument();
        // document.LoadHtml(text);
        //
        // var a = document.DocumentNode.SelectNodes("//tbody/tr[not(@class)]/td[2]");
        // var regex = new Regex("((.*?))", RegexOptions.RightToLeft);
        //
        // foreach (var node in a)
        // {
        //     var category = node.SelectSingleNode("a[1]").InnerText;
        //
        //     if (category.Contains("특가"))
        //     {
        //         var title = node.SelectSingleNode("a[2]/span[1]").InnerText.Trim();
        //         var url = $"https://meeco.kr{node.SelectSingleNode("a[2]").GetAttributeValue("href", "(null)")}";
        //
        //         if (!title.Contains("종료") && !title.Contains("완료"))
        //         {
        //             Console.WriteLine($"{title} ({url})");
        //         }
        //     }
        // }

        var document = new HtmlDocument();
        var service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;
        var options = new ChromeOptions();
        options.AddArgument("disable-gpu");
        options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36 Edg/100.0.1185.44");
        options.AddArgument("headless");
        var driver = new ChromeDriver(service, options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

        driver.Navigate().GoToUrl("https://www.ppomppu.co.kr/zboard/zboard.php?id=ppomppu");
        await Task.Delay(500);
        document.LoadHtml(driver.PageSource);

        var a = document.DocumentNode.SelectNodes("//tbody/tr[contains(@class, 'list')]/td[3]/table[1]/tbody[1]/tr[1]/td[2]/div[1]/a[1]");

        foreach (var node in a)
        {
            if (!node.InnerHtml.Contains("line-through"))
            {
                var title = node.SelectSingleNode("font[1]").InnerText.Trim();
                var url = $"https://www.ppomppu.co.kr/zboard{node.GetAttributeValue("href", "(null)").Replace("amp;", "")}";

                Console.WriteLine($"{title} ({url})");
            }
        }
    }
}