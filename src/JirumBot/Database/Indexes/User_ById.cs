using JirumBot.Data;
using Raven.Client.Documents.Indexes;

namespace JirumBot.Database.Indexes;

public class UserById : AbstractIndexCreationTask<User>
{
    public UserById() => Map = user => user.Select(x => new { x.UserId });
}