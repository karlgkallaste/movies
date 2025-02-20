using Marten;

namespace Movies.Data;

public interface IEntityRepository<T> where T : IWithId
{
    Task<T?> GetById(Guid id);

}

public class EntityRepository<T> : IEntityRepository<T> where T : IWithId
{
    private readonly IDocumentSession _documentSession;

    public EntityRepository(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<T?> GetById(Guid id)
    {
        return await _documentSession.LoadAsync<T>(id);
    }
}