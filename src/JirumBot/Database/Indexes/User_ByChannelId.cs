using JirumBot.Data;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace JirumBot.Database.Indexes;

public class User_ByChannelId : AbstractIndexCreationTask<User>
{
    public User_ByChannelId() => Map = user => user.Select(x => new { x.ChannelId });
}