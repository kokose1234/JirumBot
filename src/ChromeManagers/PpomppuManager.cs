using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.ChromeManagers
{
    public class PpomppuManager : ChromeManager
    {
        public override async Task<bool> Login(string returnUrl)
        {
            Driver.Navigate().GoToUrl(returnUrl);
            await Task.Delay(300);

            return true;
        }

        public override async Task<bool> GetNewArticle()
        {
            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);

                var isFirstJirum = !Driver.Url.Contains("market_bbs");
                var thumbnailPath = isFirstJirum ? Setting.Value.PpomJirumThumbnailUrlPath : Setting.Value.PpomJirumThumbnailUrlPath2;
                var (title, url) = GetArticleInfo(isFirstJirum ? Setting.Value.PpomJirumTitlePath : Setting.Value.PpomJirumTitlePath2,
                    isFirstJirum ? Setting.Value.PpomJirumUrlPath : Setting.Value.PpomJirumUrlPath2);
                var thumbnailUrl = GetThumbnailUrl(thumbnailPath);

                if (LatestArticle == null)
                {
                    LatestArticle = new Article {Title = title, Url = url, ThumbnailUrl = thumbnailUrl};
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
                Constants.Logger.GetExceptionLogger().Error(ex, "뽐뿌 새로고침 중 오류 발생");
                return false;
            }
        }

        private string GetThumbnailUrl(string path)
        {
            try
            {
                return Driver.FindElementByXPath(path).GetAttribute("src");
            }
            catch (Exception)
            {
                return "";
            }
        }

        private (string, string) GetArticleInfo(string path, string path2)
        {
            try
            {
                return (Driver.FindElementByXPath(path).Text, Driver.FindElementByXPath(path2).GetAttribute("href"));
            }
            catch (Exception)
            {
                return (Driver.FindElementByXPath(path.Replace("a[1]", "a[2]")).Text,
                    Driver.FindElementByXPath(path2.Replace("a[1]", "a[2]")).GetAttribute("href"));
            }
        }
    }
}