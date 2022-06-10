﻿using System;
using System.Linq;
using System.Threading.Tasks;
using JirumBot.Data;
using OpenQA.Selenium;

namespace JirumBot.CrawlManager
{
    public class FmManager : SimpleCrawlManager
    {
        private static FmManager s_instance;
        public static FmManager Instance => s_instance ??= new FmManager();

        public override async Task<bool> FetchNewArticles()
        {
            if (HttpClient == null | IsStopped) return false;

            try
            {
                var response = await HttpClient.GetStringAsync("https://www.fmkorea.com/index.php?mid=hotdeal&listStyle=list&page=1");
                if (string.IsNullOrEmpty(response)) return false;
                Document.LoadHtml(response);

                var list = Document.DocumentNode.SelectNodes(Setting.Value.FmBasePath);

                foreach (var node in list)
                {
                    if (node != null)
                    {
                        if (!node.InnerText.Contains("종료됨"))
                        {
                            var title = node.InnerText.Trim();
                            var url = $"https://www.fmkorea.com{node.GetAttributeValue("href", "(null)").Replace("&amp;", "&")}";

                            if (!url.Contains("(null)") && !ArticleHistories.Contains(url))
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
                Constants.Logger.GetExceptionLogger().Error(ex, "에펨코리아 새로고침 중 오류 발생");
                return false;
            }
        }
    }
}