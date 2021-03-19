using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.ChromeManagers
{
    public class CoolManager : ChromeManager
    {
        public override async Task<bool> Login(string returnUrl)
        {
            try
            {
                Driver.Navigate().GoToUrl("https://coolenjoy.net/");

                Driver.FindElementByXPath("//input[@id='ol_id']").SendKeys(Setting.Value.CoolId);
                Driver.FindElementByXPath("//input[@id='ol_pw']").SendKeys(Setting.Value.CoolPassWord);
                Driver.FindElementByXPath("//input[@id='ol_submit']").Click();

                await Task.Delay(500);
                Driver.Navigate().GoToUrl(returnUrl);
                return true;
            }
            catch (Exception ex)
            {
                Constants.Logger.GetExceptionLogger().Error(ex, "쿨앤조이 로그인 중 오류 발생");
                return false;
            }
        }

        public override async Task<bool> GetNewArticle()
        {
            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);

                var isFirstJirum = Driver.PageSource.Contains("지름,알뜰정보");
                var title = Driver.FindElementByXPath(isFirstJirum ? Setting.Value.CoolJirumTitlePath : Setting.Value.CoolJirumTitlePath2).Text;
                var url = Driver.FindElementByXPath(isFirstJirum ? Setting.Value.CoolJirumUrlPath : Setting.Value.CoolJirumUrlPath2)
                                .GetAttribute("href");

                if (LatestArticle == null)
                {
                    LatestArticle = new Article { Title = title, Url = url };
                    return true;
                }

                if (LatestArticle.Title != title)
                {
                    LatestArticle.Title = title;
                    LatestArticle.Url = url;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Constants.Logger.GetExceptionLogger().Error(ex, "쿨엔조이 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}