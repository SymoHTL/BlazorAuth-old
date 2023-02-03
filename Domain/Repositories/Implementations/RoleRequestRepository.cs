namespace Domain.Repositories.Implementations; 

public class RoleRequestRepository : ARepository<RoleRequest>, IRoleRequestRepository {
    public IRoleClaimRepository RoleClaimRepository { get; set; }
    public RoleRequestRepository(ModelDbContext context, IRoleClaimRepository roleClaimRepository) : base(context) {
        RoleClaimRepository = roleClaimRepository;
    }

    public async Task<List<RoleRequest>> GetByUserIdAsync(int userId, CancellationToken ct = default) {
        return await Table
            .Include(r => r.User)
            .Include(r => r.Role)
            .Where(r => r.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<RoleRequest> CreateRequestAsync(User user, Role role, CancellationToken ct = default) {
        var existingRequest = await GetByUserIdAsync(user.Id, ct);
        if (existingRequest.Any(e => e.UserId == user.Id && e.RoleId == role.Id)) {
            throw new Exception("Request already exists");
        }
        
        var request = new RoleRequest {
            UserId = user.Id,
            RoleId = role.Id,
            RequestDate = DateTime.Now
        };

        return await CreateAsync(request, ct);
    }

    public async Task ApproveRequestAsync(RoleRequest request, CancellationToken ct = default) {
        var roleClaim = new RoleClaim {
            UserId = request.UserId,
            RoleId = request.RoleId
        };
        await RoleClaimRepository.CreateAsync(roleClaim, ct);
        await DeleteAsync(request, ct);
    }

    public async Task RejectRequestAsync(RoleRequest request, CancellationToken ct = default) {
        await DeleteAsync(request, ct);
    }

}