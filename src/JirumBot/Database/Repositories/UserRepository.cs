using JirumBot.Data;
using Raven.Client.Documents.Session;

namespace JirumBot.Database.Repositories;

public class UserRepository
{
    private readonly IDocumentSession _documentSession;
    private readonly DocumentStoreLifecycle _lifecycle;

    public UserRepository(DocumentStoreLifecycle lifecycle)
    {
        _lifecycle = lifecycle;
        _documentSession = _lifecycle.Store.OpenSession();
    }

    public List<User> All() => _documentSession.Query<User>().ToList();

    public User? GetById(ulong id)
    {
        return _documentSession.Query<User>().FirstOrDefault(x => x.UserId == id.ToString());
    }

    public User? GetByChannelId(ulong id)
    {
        return _documentSession.Query<User>().FirstOrDefault(x => x.ChannelId == id.ToString());
    }

    public void Create(User user)
    {
        _documentSession.Store(user);
        _documentSession.SaveChanges();
    }

    public void Delete(User user)
    {
        _documentSession.Delete(user);
        _documentSession.SaveChanges();
    }

    public void AddKeyword(ulong id, string keyword)
    {
        var user = GetById(id);
        user.Keywords.Add(keyword);

        _documentSession.SaveChanges();
    }

    public void DeleteKeyword(ulong id, string keyword)
    {
        var user = GetById(id);
        user.Keywords.Remove(keyword);

        _documentSession.SaveChanges();
    }

    public void ClearKeyword(ulong id)
    {
        var user = GetById(id);
        user.Keywords.Clear();

        _documentSession.SaveChanges();
    }
}