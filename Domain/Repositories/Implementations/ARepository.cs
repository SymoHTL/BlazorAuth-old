namespace Domain.Repositories.Implementations;

public abstract class ARepository<TEntity> : IRepository<TEntity> where TEntity : class {
    protected readonly ModelDbContext Context;
    protected readonly DbSet<TEntity> Table;

    protected ARepository(ModelDbContext context) {
        Context = context;
        Table = Context.Set<TEntity>();
    }

    public async Task<List<TEntity>> ReadAsync(CancellationToken ct = default) {
        return await Table.ToListAsync(ct);
    }

    public async Task<TEntity?> ReadAsync(int id, CancellationToken ct = default) {
        return await Table.FindAsync(new object?[] { id }, ct);
    }

    public async Task<List<TEntity>> ReadAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default) {
        return await Table.Where(filter).ToListAsync(ct);
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default) {
        Table.Add(entity);
        await Context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<List<TEntity>> CreateAsync(List<TEntity> entity, CancellationToken ct = default) {
        Table.AddRange(entity);
        await Context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct = default) {
        Context.ChangeTracker.Clear();
        Table.Update(entity);
        await Context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(IEnumerable<TEntity> entity, CancellationToken ct = default) {
        Context.ChangeTracker.Clear();
        Table.UpdateRange(entity);
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken ct = default) {
        Table.Remove(entity);
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(IEnumerable<TEntity> entity, CancellationToken ct = default) {
        Table.RemoveRange(entity);
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default) {
        Table.RemoveRange(Table.Where(filter));
        await Context.SaveChangesAsync(ct);
    }
}