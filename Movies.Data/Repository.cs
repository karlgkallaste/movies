using Marten;

namespace Movies.Data;

public interface IRepository<T> where T : class
{
    Task<T?> GetById(Guid id);
    Task<IReadOnlyList<T>> All();
}

public class Repository<T> : IRepository<T> where T : class
{
    private readonly IDocumentSession _documentSession;

    public Repository(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<T?> GetById(Guid id)
    {
        return await _documentSession.LoadAsync<T>(id) ?? null;
    }

    public async Task<IReadOnlyList<T>> All()
    {
        var query = _documentSession.Query<T>();
        return await query.ToListAsync();
    }
}