using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.CrawlManager
{
    public class MeecoManager : SimpleCrawlManager
    {
        private static MeecoManager s_instance;
        public static MeecoManager Instance => s_instance ??= new MeecoManager();

        public override async Task<bool> FetchNewArticles()
        {
            if (HttpClient == null | IsStopped) return false;

            try
            {
                var response = await HttpClient.GetStringAsync("https://meeco.kr/PricePlus");
                if (string.IsNullOrEmpty(response)) return false;
                Document.LoadHtml(response);

                var list = Document.DocumentNode.SelectNodes(Setting.Value.MeecoBasePath);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        var category = node.SelectSingleNode("a[1]").InnerText;

                        if (category.Contains("특가"))
                        {
                            var title = node.SelectSingleNode("a[2]/span[1]").InnerText.Trim();
                            var url = $"https://meeco.kr{node.SelectSingleNode("a[2]").GetAttributeValue("href", "(null)")}";

                            if (!title.Contains("종료") && !title.Contains("완료") && !url.Contains("(null)") && !ArticleHistories.Contains(url))
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
                Constants.Logger.GetExceptionLogger().Error(ex, "미니기기 코리아 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}