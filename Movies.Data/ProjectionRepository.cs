using Marten;

namespace Movies.Data;

public interface IProjectionRepository<T> where T : IProjection
{
    Task<T?> GetById(Guid id);
    Task<IReadOnlyList<T>> GetPagedResults(int page = 1, int pageSize = 10);
}

public class ProjectionRepository<T> : IProjectionRepository<T> where T : IProjection
{
    private readonly IDocumentSession _documentSession;

    public ProjectionRepository(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<T?> GetById(Guid id)
    {
        return await _documentSession.Query<T>()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IReadOnlyList<T>> GetPagedResults(int page = 1, int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);
        var query = _documentSession.Query<T>();

        var pagedResults = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return pagedResults;
    }
}