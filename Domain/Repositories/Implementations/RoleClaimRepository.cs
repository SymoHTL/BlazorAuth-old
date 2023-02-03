namespace Domain.Repositories.Implementations;

public class RoleClaimRepository : ARepository<RoleClaim>, IRoleClaimRepository {
    public RoleClaimRepository(ModelDbContext context) : base(context) {
    }

    public async Task<List<Role>> ReadRolesByUserIdAsync(int id, CancellationToken ctsToken = default) {
        return await Table
            .Where(x => x.UserId == id)
            .Select(x => x.Role)
            .ToListAsync(ctsToken);
    }
    
}