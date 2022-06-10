using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace JirumBot.Attribute
{
    public class PrivateCommandAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context != null & ((SocketCommandContext) context).IsPrivate)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError($"{context?.User.Mention}님, 해당 명령어는 개인 대화에서만 사용 가능합니다."));
        }
    }
}
