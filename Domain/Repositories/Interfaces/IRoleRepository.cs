namespace Domain.Repositories.Interfaces;

public interface IRoleRepository : IRepository<Role> {
    Task<List<Role>> ReadForRequestAsync(int userId, CancellationToken ctsToken = default);
    Task<List<Role>> SearchForRoleAsync(string search, int userId, CancellationToken ctsToken = default);
}