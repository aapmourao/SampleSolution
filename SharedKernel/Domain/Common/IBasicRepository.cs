namespace SharedKernel.Domain.Common;

public interface IBasicRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<T>> GetListAsync(CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
    Task RemoveAsync(T entity, CancellationToken cancellationToken);
}