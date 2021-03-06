using System;
using System.Threading.Tasks;
using JirumBot.Data;
using OpenQA.Selenium;

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
                Constants.Logger.GetExceptionLogger().Error(ex, "퀘이사존 로그인 중 오류 발생");
                return false;
            }
        }

        public override async Task<bool> GetNewArticle()
        {
            if (Driver == null | IsStopped) return false;

            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);

                var isFirstJirum = Driver.Url.Contains("qb_saleinfo");
                var thumbnailPath = isFirstJirum ? Setting.Value.QuasarJirumThumbnailUrlPath : Setting.Value.QuasarJirumThumbnailUrlPath2;
                var title = Driver.FindElementByXPath(isFirstJirum ? Setting.Value.QuasarJirumTitlePath : Setting.Value.QuasarJirumTitlePath2)
                                  .Text;
                var url = Driver.FindElementByXPath(isFirstJirum ? Setting.Value.QuasarJirumUrlPath : Setting.Value.QuasarJirumUrlPath2)
                                .GetAttribute("href");
                var thumbnailUrl = Driver.FindElementByXPath(thumbnailPath).GetCssValue("background-image").Replace("url(\"", "").Replace("\")", "");

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
            catch (UnhandledAlertException ex) when (ex.AlertText == "가입 후 30일 이후에 장터게시판에 글을 읽으실 수 있습니다.")
            {
                Constants.Logger.GetExceptionLogger().Error(ex, "퀘이사존 가입 후 30일이 지나지 않아 장터게시판을 열 수 없습니다.");
                IsStopped = true;
                return false;
            }
            catch (Exception ex)
            {
                Constants.Logger.GetExceptionLogger().Error(ex, "퀘이사존 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}