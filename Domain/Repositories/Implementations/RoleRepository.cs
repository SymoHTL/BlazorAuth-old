namespace Domain.Repositories.Implementations;

public class RoleRepository : ARepository<Role>, IRoleRepository {
    
    private readonly IRoleRequestRepository _roleRequestRepository;
    public RoleRepository(ModelDbContext context, IRoleRequestRepository roleRequestRepository) : base(context) {
        _roleRequestRepository = roleRequestRepository;
    }

    public async Task<List<Role>> ReadForRequestAsync(int userId, CancellationToken ctsToken = default) {
        var roleClaims = await _roleRequestRepository.RoleClaimRepository.ReadRolesByUserIdAsync(userId, ctsToken);
        
        return await Table
            //.Where(x => x.Identifier != "Admin") exclude certain roles you don't want to show
            .Where(x => x.Users.All(u => u.Id != userId) &&
                        !x.RoleRequests.Any(r => r.UserId == userId))
            .ToListAsync(ctsToken);
    }

    public Task<List<Role>> SearchForRoleAsync(string search, int userId, CancellationToken ctsToken = default) {
        return Table
            .Where(x => x.Identifier.Contains(search))
            .Where(x => !x.Users.Any(u => u.Id == userId) &&
                        !x.RoleRequests.Any(r => r.UserId == userId))
            .ToListAsync(ctsToken);
    }
}