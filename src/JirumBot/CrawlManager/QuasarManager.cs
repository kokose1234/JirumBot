using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.CrawlManager
{
    public class QuasarManager : SimpleCrawlManager
    {
        private static QuasarManager s_instance;
        public static QuasarManager Instance => s_instance ??= new QuasarManager();

        public override async Task<bool> FetchNewArticles()
        {
            if (httpClient == null | IsStopped) return false;

            try
            {
                var response = await httpClient.GetStringAsync("https://quasarzone.com/bbs/qb_saleinfo");
                if (string.IsNullOrEmpty(response)) return false;
                document.LoadHtml(response);

                var list = document.DocumentNode.SelectNodes(Setting.Value.QuasarBasePath);
                var regex = new Regex("&(.*?);", RegexOptions.RightToLeft);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        var title = node.SelectSingleNode(Setting.Value.QuasarTitlePath).InnerText.Trim();
                        var status = node.SelectSingleNode(Setting.Value.QuasarStatusPath).InnerText;
                        var url = $"https://quasarzone.com{node.SelectSingleNode(Setting.Value.QuasarUrlPath).GetAttributeValue("href", "(null)")}";

                        if (!status.Contains("종료") && !articleHistories.Contains(url) && !url.Contains("(null)"))
                        {
                            Articles.Add(new() { Title = regex.Replace(title, ""), Url = url });
                            articleHistories.Add(url);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Constants.Logger.GetExceptionLogger().Error(ex, "퀘이사존 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}