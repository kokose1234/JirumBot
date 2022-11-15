using System;
using System.Threading.Tasks;
using JirumBot.Data;

namespace JirumBot.CrawlManager
{
    public class PpomppuManager : ChromeManager
    {
        private static PpomppuManager s_instance;
        public static PpomppuManager Instance => s_instance ??= new PpomppuManager();

        private PpomppuManager() => Driver.Navigate().GoToUrl("https://www.ppomppu.co.kr/zboard/zboard.php?id=ppomppu");

        public override async Task<bool> FetchNewArticles()
        {
            try
            {
                Driver.Navigate().Refresh();
                await Task.Delay(500);
                _document.LoadHtml(Driver.PageSource);

                var list = _document.DocumentNode.SelectNodes(Setting.Value.PpomBasePath);
                foreach (var node in list)
                {
                    if (node != null)
                    {
                        if (!node.InnerHtml.Contains("line-through"))
                        {
                            var title = node.SelectSingleNode(Setting.Value.PpomTitlePath).InnerText.Trim();
                            var link = node.GetAttributeValue("href", "(null)").Replace("amp;", "");
                            var url = "https://www.ppomppu.co.kr";

                            if (link.StartsWith("view.php"))
                            {
                                url += $"/zboard/{link}";
                            }
                            else
                            {
                                url += link;
                            }

                            if (url != "(null)" && !_articleHistories.Contains(url))
                            {
                                Articles.Add(new() {Title = title, Url = url});
                                _articleHistories.Add(url);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Constants.Logger.GetExceptionLogger().Error(ex, "뽐뿌 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}