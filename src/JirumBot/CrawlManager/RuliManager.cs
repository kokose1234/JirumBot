using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.CrawlManager
{
    public class RuliManager : ChromeManager
    {
        private static RuliManager s_instance;
        public static RuliManager Instance => s_instance ??= new RuliManager();

        public RuliManager()
        {
            Driver.Navigate().GoToUrl("https://bbs.ruliweb.com/market/board/1020?view=default");
            Task.Delay(500).Wait();
        }

        public override async Task<bool> FetchNewArticles()
        {
            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);
                _document.LoadHtml(Driver.PageSource);

                var list = _document.DocumentNode.SelectNodes(Setting.Value.RuliBasePath);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        var title = node.InnerText.Replace("\n", "").Trim();
                        var url = node.GetAttributeValue("href", "(null)");

                        if (!title.Contains("종료") && url != "(null)" && !_articleHistories.Contains(url))
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
                Constants.Logger.GetExceptionLogger().Error(ex, "루리웹 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}