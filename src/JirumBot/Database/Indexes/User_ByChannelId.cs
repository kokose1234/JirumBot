using JirumBot.Data;
using Raven.Client.Documents.Indexes;

namespace JirumBot.Database.Indexes;

public class UserByChannelId : AbstractIndexCreationTask<User>
{
    public UserByChannelId() => Map = user => user.Select(x => new { x.ChannelId });
}