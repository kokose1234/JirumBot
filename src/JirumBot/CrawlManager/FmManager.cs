using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.CrawlManager
{
    public class FmManager : ChromeManager
    {
        private static FmManager s_instance;
        public static FmManager Instance => s_instance ??= new FmManager();

        public FmManager()
        {
            Driver.Navigate().GoToUrl("https://www.fmkorea.com/index.php?mid=hotdeal&listStyle=list&page=1");
            Task.Delay(500).Wait();
        }

        public override async Task<bool> FetchNewArticles()
        {
            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);
                _document.LoadHtml(Driver.PageSource);

                var list = _document.DocumentNode.SelectNodes(Setting.Value.FmBasePath);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        if (!node.InnerText.Contains("종료됨"))
                        {
                            var title = node.InnerText.Trim();
                            var url = $"https://www.fmkorea.com{node.GetAttributeValue("href", "(null)").Replace("&amp;", "&")}";

                            if (!url.Contains("(null)") && !_articleHistories.Contains(url))
                            {
                                Articles.Add(new() { Title = title, Url = url });
                                _articleHistories.Add(url);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Constants.Logger.GetExceptionLogger().Error(ex, "에펨코리아 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}