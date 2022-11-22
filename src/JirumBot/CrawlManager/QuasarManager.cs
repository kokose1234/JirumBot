using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.CrawlManager
{
    public class QuasarManager : ChromeManager
    {
        private static QuasarManager s_instance;
        public static QuasarManager Instance => s_instance ??= new QuasarManager();

        public QuasarManager()
        {
            Driver.Navigate().GoToUrl("https://quasarzone.com/bbs/qb_saleinfo");
            Task.Delay(500).Wait();
        }

        public override async Task<bool> FetchNewArticles()
        {
            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);
                _document.LoadHtml(Driver.PageSource);

                var list = _document.DocumentNode.SelectNodes(Setting.Value.QuasarBasePath);
                var regex = new Regex("&(.*?);", RegexOptions.RightToLeft);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        var title = node.SelectSingleNode(Setting.Value.QuasarTitlePath).InnerText.Trim();
                        var status = node.SelectSingleNode(Setting.Value.QuasarStatusPath).InnerText;
                        var url = $"https://quasarzone.com{node.SelectSingleNode(Setting.Value.QuasarUrlPath).GetAttributeValue("href", "(null)")}";

                        if (!status.Contains("종료") && !_articleHistories.Contains(url) && !url.Contains("(null)"))
                        {
                            Articles.Add(new() { Title = regex.Replace(title, ""), Url = url });
                            _articleHistories.Add(url);
                        }
                    }
                }

                FixLinks();

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