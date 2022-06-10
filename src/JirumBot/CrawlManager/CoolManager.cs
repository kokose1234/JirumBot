using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.CrawlManager
{
    public class CoolManager : SimpleCrawlManager
    {
        private static CoolManager s_instance;
        public static CoolManager Instance => s_instance ??= new CoolManager();

        public override async Task<bool> FetchNewArticles()
        {
            if (HttpClient == null | IsStopped) return false;

            try
            {
                var response = await HttpClient.GetStringAsync("https://coolenjoy.net/bbs/jirum");
                if (string.IsNullOrEmpty(response)) return false;
                Document.LoadHtml(response);

                var list = Document.DocumentNode.SelectNodes(Setting.Value.CoolBasePath);
                foreach (var node in list)
                {
                    if (node != null)
                    {
                        if (!node.InnerText.Contains("종료됨"))
                        {
                            var title = node.FirstChild.InnerText.Trim();
                            var url = node.GetAttributeValue("href", "(null)");

                            if (url != "(null)" && !ArticleHistories.Contains(url))
                            {
                                Articles.Add(new() { Title = title, Url = url });
                                ArticleHistories.Add(url);
                            }
                        }
                    }
                }

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