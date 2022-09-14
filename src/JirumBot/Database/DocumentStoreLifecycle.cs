using JirumBot.Database.Indexes;
using Raven.Client.Documents;

namespace JirumBot.Database;

public class DocumentStoreLifecycle : IDisposable
{
    public IDocumentStore Store { get; }

    public DocumentStoreLifecycle()
    {
        Store = new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:8080" },
            Database = "data"
        }.Initialize();

        new UserById().Execute(Store);
        new UserByChannelId().Execute(Store);
    }

    public void Dispose() => Store.Dispose();
}