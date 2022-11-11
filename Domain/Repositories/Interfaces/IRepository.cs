namespace Domain.Repositories.Interfaces;

public interface IRepository<TEntity> where TEntity : class {
    Task<TEntity?> ReadAsync(int id, CancellationToken ct = default);
    Task<List<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default);
    Task<List<TEntity>> ReadAsync(CancellationToken ct = default);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default);
    Task<List<TEntity>> CreateAsync(List<TEntity> entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(IEnumerable<TEntity> entity, CancellationToken ct = default);
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteAsync(IEnumerable<TEntity> entity, CancellationToken ct = default);
    Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default);
}