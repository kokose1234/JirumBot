using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.CrawlManager
{
    public class RuliManager : SimpleCrawlManager
    {
        private static RuliManager s_instance;
        public static RuliManager Instance => s_instance ??= new RuliManager();

        public override async Task<bool> FetchNewArticles()
        {
            if (HttpClient == null | IsStopped) return false;

            try
            {
                var response = await HttpClient.GetStringAsync("https://bbs.ruliweb.com/market/board/1020?view=default");
                if (string.IsNullOrEmpty(response)) return false;
                Document.LoadHtml(response);

                var list = Document.DocumentNode.SelectNodes(Setting.Value.RuliBasePath);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        var title = node.InnerText.Replace("\n", "").Trim();
                        var url = node.GetAttributeValue("href", "(null)");

                        if (!title.Contains("종료") && url != "(null)" && !ArticleHistories.Contains(url))
                        {
                            Articles.Add(new() { Title = title, Url = url });
                            ArticleHistories.Add(url);
                        }
                    }
                }

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