namespace Domain.Repositories.Interfaces;

public interface IRoleClaimRepository : IRepository<RoleClaim> {
    Task<List<Role>> ReadRolesByUserIdAsync(int id, CancellationToken ctsToken = default);
    
}