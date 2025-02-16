using Marten;

namespace Movie.Data;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
}

public class Repository<T> : IRepository<T> where T : class
{
    private readonly IDocumentSession _session;

    public Repository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _session.LoadAsync<T>(id) ?? null;
    }
}