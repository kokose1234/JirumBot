using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.CrawlManager
{
    public class ClienManager : ChromeManager
    {
        private static ClienManager s_instance;
        public static ClienManager Instance => s_instance ??= new ClienManager();

        public ClienManager()
        {
            Driver.Navigate().GoToUrl("https://www.clien.net/service/board/jirum");
            Task.Delay(500).Wait();
        }

        public override async Task<bool> FetchNewArticles()
        {
            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);
                _document.LoadHtml(Driver.PageSource);

                var list = _document.DocumentNode.SelectNodes(Setting.Value.ClienBasePath);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        var title = node.FirstChild.InnerText.Trim();
                        var url = $"https://www.clien.net{node.GetAttributeValue("href", "(null)")}";

                        if (!url.Contains("(null)") && !_articleHistories.Contains(url))
                        {
                            Articles.Add(new() { Title = title, Url = url });
                            _articleHistories.Add(url);
                        }
                    }
                }

                FixLinks();

                return true;
            }
            catch (Exception ex)
            {
                Constants.Logger.GetExceptionLogger().Error(ex, "클리앙 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}