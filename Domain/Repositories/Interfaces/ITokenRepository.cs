namespace Domain.Repositories.Interfaces; 

public interface ITokenRepository : IRepository<Token> {
    Task<Token> FindByValueAsync(string value, CancellationToken ct = default);
    Task<string> RequestTokenAsync(User user, CancellationToken ct = default);
    Task SetActiveAsync(Token token, CancellationToken ctsToken = default);
    Task SetInactiveAsync(Token token, CancellationToken ctsToken = default);
    Task LoginAsync(Token token, CancellationToken ct = default);
    Task<List<Token>> ReadForUserAsync(int id, CancellationToken ctsToken);
    Task SetDeletedAsync(Token token, CancellationToken ctsToken);
}