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
                Console.WriteLine("쿨앤조이 로그인 중 오류 발생");
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

                var isFirstJirum = IsFirstJirum();
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
                Console.WriteLine("쿨엔조이 새로고침 중 오류 발생");
                Console.WriteLine(ex);
                return false;
            }
        }

        private bool IsFirstJirum() => Driver.FindElementByXPath(Setting.Value.CoolJirumCategoryPath).Text.Contains("지름,알뜰정보");
    }
}