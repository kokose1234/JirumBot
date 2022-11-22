using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.CrawlManager
{
    public class CoolManager : ChromeManager
    {
        private static CoolManager s_instance;
        public static CoolManager Instance => s_instance ??= new CoolManager();

        public CoolManager()
        {
            Driver.Navigate().GoToUrl("https://coolenjoy.net/bbs/jirum");
            Task.Delay(500).Wait();
        }

        public override async Task<bool> FetchNewArticles()
        {
            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);
                _document.LoadHtml(Driver.PageSource);

                var list = _document.DocumentNode.SelectNodes(Setting.Value.CoolBasePath);
                foreach (var node in list)
                {
                    if (node != null)
                    {
                        if (!node.InnerText.Contains("종료됨"))
                        {
                            var title = node.FirstChild.InnerText.Trim();
                            var url = node.GetAttributeValue("href", "(null)");

                            if (url != "(null)" && !_articleHistories.Contains(url))
                            {
                                Articles.Add(new() {Title = title, Url = url});
                                _articleHistories.Add(url);
                            }
                        }
                    }
                }

                FixLinks();

                return true;
            }
            catch (Exception ex)
            {
                Constants.Logger.GetExceptionLogger().Error(ex, "쿨엔조이 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}