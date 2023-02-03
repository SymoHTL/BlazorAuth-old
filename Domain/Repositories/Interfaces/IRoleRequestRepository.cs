namespace Domain.Repositories.Interfaces;

public interface IRoleRequestRepository : IRepository<RoleRequest> {
    public IRoleClaimRepository RoleClaimRepository { get; set; }

    Task<List<RoleRequest>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    
    Task<RoleRequest> CreateRequestAsync(User user, Role role, CancellationToken ct = default);
    
    Task ApproveRequestAsync(RoleRequest request, CancellationToken ct = default);
    
    Task RejectRequestAsync(RoleRequest request, CancellationToken ct = default);
}