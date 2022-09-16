using Discord;
using Discord.Commands;
using JirumBot.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JirumBot.Command
{
    public class GeneralCommands : ModuleBase<SocketCommandContext>
    {
        [Command("?", true)]
        [Summary("도움말을 봅니다.")]
        public async Task SendHelp()
        {
            static string GetFullUserName(IUser usr)
            {
                var sb = new StringBuilder(usr.Username);
                sb.Append("#").Append(usr.DiscriminatorValue.ToString("D4"));
                return sb.ToString();
            }

            var commands = Program.Service.Commands.Where(x => !x.Name.Contains("테스트"));
            var builder = new EmbedBuilder();

            builder.WithThumbnailUrl("https://cdn.discordapp.com/app-icons/821300653136805928/8caa403394f55277b851bbd841cd8b7d.png?size=512");
            builder.WithColor(Color.Blue);
            builder.WithTitle("명령어 목록");
            builder.AddField("<:scroll:643684183293820998> 명령어 <:scroll:643684183293820998>", $"{GetFullUserName(Context.User)}님이 사용하실 수 있는 명령어 목록입니다.");

            foreach (var command in commands)
            {
                if (command.Name != "유령삭제" && command.Name != "테스트모드")
                {
                    builder.AddField("/" + command.Name, string.IsNullOrEmpty(command.Summary) ? "설명이 없습니다." : command.Summary, true);
                }
            }

            await Context.Message.ReplyAsync("", false, builder.Build());
        }

        [Command("목록", true)]
        [Summary("검색 키워드 목록을 봅니다. 사용법: /목록")]
        public async Task SendKeywordList()
        {
            var repo = ServiceProviderFactory.ServiceProvider.GetService<UserRepository>();
            var user = repo?.GetById(Context.User.Id);
            if (user == null)
            {
                await Context.Message.ReplyAsync("user가 null");
                return;
            }

            var keywords = user.Keywords.OrderBy(x => x).ToArray();
            if (keywords.Length == 0)
            {
                await Context.Message.ReplyAsync("키워드가 등록되지 않았습니다.");
                return;
            }

            await Context.Message.ReplyAsync($"```{string.Join(", ", user.Keywords)}```");
        }

        [Command("추가", true)]
        [Summary("검색 키워드를 추가합니다. 사용법: /추가 1080")]
        public async Task AddKeyword(string keyword)
        {
            if (keyword.Length < 2)
            {
                await Context.Message.ReplyAsync("키워드는 최소 2글자 부터 등록 가능합니다.");
                return;
            }

            var repo = ServiceProviderFactory.ServiceProvider.GetService<UserRepository>();
            var user = repo?.GetById(Context.User.Id);
            if (user == null)
            {
                await Context.Message.ReplyAsync("user가 null");
                return;
            }

            if (user.Keywords.Any(x => string.Equals(x, keyword, StringComparison.CurrentCultureIgnoreCase)))
            {
                await Context.Message.ReplyAsync($"{keyword}는 이미 존재하는 키워드 입니다.");
                return;
            }

            repo.AddKeyword(Context.User.Id, keyword);
            await Context.Message.ReplyAsync($"{keyword} 추가 완료.");
        }

        [Command("제거", true)]
        [Summary("검색 키워드를 삭제합니다. 사용법: /제거 1080")]
        public async Task RemoveKeyword(string keyword)
        {
            var repo = ServiceProviderFactory.ServiceProvider.GetService<UserRepository>();
            var user = repo?.GetById(Context.User.Id);
            if (user == null)
            {
                await Context.Message.ReplyAsync("user가 null");
                return;
            }

            if (!user.Keywords.Any(x => string.Equals(x, keyword, StringComparison.CurrentCultureIgnoreCase)))
            {
                await Context.Message.ReplyAsync($"{keyword}는 존재하지 않는 키워드 입니다.");
                return;
            }

            repo.DeleteKeyword(Context.User.Id, keyword);
            await Context.Message.ReplyAsync($"{keyword} 제거 완료.");
        }

        [Command("키워드초기화", true)]
        [Summary("검색 키워드를 초기화합니다. 사용법: /키워드초기화")]
        public async Task ClearKeyword()
        {
            var repo = ServiceProviderFactory.ServiceProvider.GetService<UserRepository>();
            var user = repo?.GetById(Context.User.Id);
            if (user == null)
            {
                await Context.Message.ReplyAsync("user가 null");
                return;
            }


            repo.ClearKeyword(Context.User.Id);
            await Context.Message.ReplyAsync($"키워드 초기화 완료.");
        }

        [Command("정보", true)]
        [Summary("봇 정보를 확인합니다. 사용법: /정보")]
        public async Task Info()
        {
            var obj = JObject.Parse(await File.ReadAllTextAsync("./JirumBot.deps.json"));

            if (obj != null)
            {
                var builder = new EmbedBuilder();
                builder.WithColor(Color.Blue);
                builder.WithTitle("봇 정보");
                builder.WithDescription("제작: 의문의 봇 장인, 아이디어: SpearDragon");
                builder.AddField("라이브러리 리스트", "봇 제작에 사용된 라이브러리 리스트 입니다.");
                foreach (JProperty lib in obj["targets"][".NETCoreApp,Version=v6.0"]["JirumBot/1.0.0"]["dependencies"])
                {
                    builder.AddField(lib.Name, lib.Value);
                }

                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
        }
    }
}