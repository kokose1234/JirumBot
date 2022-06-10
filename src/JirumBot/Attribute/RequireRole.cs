using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JirumBot.Attribute
{
    public class RequireRoleAttribute : PreconditionAttribute
    {
        private readonly string[] _names;

        public RequireRoleAttribute(string[] names) => _names = names;
        public RequireRoleAttribute(string name) => _names = new[] { name };

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (!(context.User is SocketGuildUser user)) 
                return Task.FromResult(PreconditionResult.FromError("권한이 부족합니다."));

            return Task.FromResult(_names.Any(role => 
                user.Roles.Any(r => r.Name == role)) ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("권한이 부족합니다."));
        }
    }
}
