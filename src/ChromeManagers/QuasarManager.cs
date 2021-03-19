using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.ChromeManagers
{
    public class QuasarManager : ChromeManager
    {
        public override async Task<bool> Login(string returnUrl)
        {
            try
            {
                Driver.Navigate().GoToUrl("https://quasarzone.com/login");

                Driver.FindElementByXPath("//input[@id='login_id']").SendKeys(Setting.Value.QuasarId);
                Driver.FindElementByXPath("//input[@id='password']").SendKeys(Setting.Value.QuasarPassWord);
                Driver.FindElementByXPath("//p[@class='login-bt']//a").Click();

                await Task.Delay(500);
                Driver.Navigate().GoToUrl(returnUrl);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("퀘이사존 로그인 중 오류 발생");
                Console.WriteLine(ex);
                return false;
            }
        }

        public override async Task<bool> GetNewArticle()
        {
            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);

                var isFirstJirum = Driver.PageSource.Contains("지름/할인정보");
                var title = Driver.FindElementByXPath(isFirstJirum ? Setting.Value.QuasarJirumTitlePath : Setting.Value.QuasarJirumTitlePath2).Text;
                var url = Driver.FindElementByXPath(isFirstJirum ? Setting.Value.QuasarJirumUrlPath : Setting.Value.QuasarJirumUrlPath2)
                                .GetAttribute("href");
                var thumbnailUrl = isFirstJirum
                    ? Driver.FindElementByXPath(Setting.Value.QuasarJirumThumbnailUrlPath).GetCssValue("background-image")
                            .Replace("url(\"", "").Replace("\")", "")
                    : "";

                if (LatestArticle == null)
                {
                    LatestArticle = new Article {Title = title, Url = url, ThumbnailUrl = thumbnailUrl};
                    return true;
                }

                if (LatestArticle.Title != title)
                {
                    LatestArticle.Title = title;
                    LatestArticle.Url = url;
                    LatestArticle.ThumbnailUrl = thumbnailUrl;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("퀘이사존 새로고침 중 오류 발생");
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}