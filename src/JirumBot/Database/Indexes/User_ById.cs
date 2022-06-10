using JirumBot.Data;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace JirumBot.Database.Indexes;

public class User_ById : AbstractIndexCreationTask<User>
{
    public User_ById() => Map = user => user.Select(x => new { x.UserId });
}