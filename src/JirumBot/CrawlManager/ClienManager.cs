using System;
using System.Threading.Tasks;
using JirumBot.Data;
using OpenQA.Selenium;

namespace JirumBot.CrawlManager
{
    public class ClienManager : SimpleCrawlManager
    {
        private static ClienManager s_instance;
        public static ClienManager Instance => s_instance ??= new ClienManager();

        public override async Task<bool> FetchNewArticles()
        {
            if (httpClient == null | IsStopped) return false;

            try
            {
                var response = await httpClient.GetStringAsync("https://www.clien.net/service/board/jirum");
                if (string.IsNullOrEmpty(response)) return false;
                document.LoadHtml(response);

                var list = document.DocumentNode.SelectNodes(Setting.Value.ClienBasePath);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        var title = node.FirstChild.InnerText.Trim();
                        var url = $"https://www.clien.net{node.GetAttributeValue("href", "(null)")}";

                        if (!url.Contains("(null)") && !articleHistories.Contains(url))
                        {
                            Articles.Add(new() { Title = title, Url = url });
                            articleHistories.Add(url);
                        }
                    }
                }

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