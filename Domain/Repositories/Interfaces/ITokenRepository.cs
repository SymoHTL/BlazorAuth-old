namespace Domain.Repositories.Interfaces;

public interface ITokenRepository : IRepository<Token> {
    Task<Token?> FindByIdentifier(string token, CancellationToken ct = default);
    Task<Token?> GenerateTokenAsync(User user, string deviceName, CancellationToken ct = default);
}