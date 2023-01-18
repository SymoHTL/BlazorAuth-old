namespace Domain.Repositories.Implementations;

public class TokenRepository : ARepository<Token>, ITokenRepository {
    private readonly RequestStore _requestStore;

    public TokenRepository(ModelDbContext context, RequestStore requestStore) : base(context) {
        _requestStore = requestStore;
    }

    public async Task<Token> FindByValueAsync(string value, CancellationToken ct = default) {
        var token = await Table
            .FirstOrDefaultAsync(t => t.Value == value, ct);
        if (token is null) throw new TokenNotFoundException();
        if (token.ExpirationDate <= DateTime.UtcNow) throw new TokenExpiredException();
        if (token.Deleted) throw new TokenDeletedException();
        if (!token.IsActive) throw new TokenUnValidException();

        return token;
    }

    public async Task<string> RequestTokenAsync(User user, CancellationToken ct) {
        if (_requestStore.IpAddress == null || _requestStore.Request == null) throw new RequestStoreException();

        var ipAddress = _requestStore.IpAddress.ToString();
        var userAgent = _requestStore.Request.Headers["User-Agent"].ToString();

        var token = await FindByIpAndUserAgentAsync(ipAddress, userAgent, ct);

        if (token == null) {
            token = new Token(userAgent, DateTime.UtcNow.AddDays(7), user.Id, ipAddress);
            token = await CreateAsync(token, ct);
            return token.Value;
        }

        token.LastLoginDate = DateTime.UtcNow;
        await UpdateAsync(token, ct);
        return token.Value;
    }


    public async Task SetDeletedAsync(Token token, CancellationToken ctsToken) {
        token.Deleted = true;
        await UpdateAsync(token, ctsToken);
    }

    public async Task SetActiveAsync(Token token, CancellationToken ctsToken = default) {
        token.IsActive = true;
        await UpdateAsync(token, ctsToken);
    }

    public async Task SetInactiveAsync(Token token, CancellationToken ctsToken = default) {
        token.IsActive = false;
        await UpdateAsync(token, ctsToken);
    }

    public async Task LoginAsync(Token token, CancellationToken ct = default) {
        token.LastLoginDate = DateTime.UtcNow;
        await UpdateAsync(token, ct);
    }

    public async Task<List<Token>> ReadForUserAsync(int id, CancellationToken ctsToken) {
        return await Table
            .Where(t => t.UserId == id && t.Deleted == false)
            .OrderByDescending(t => t.LastLoginDate)
            .ThenByDescending(t => t.IsActive)
            .ToListAsync(ctsToken);
    }

    private async Task<Token?> FindByIpAndUserAgentAsync(string ipAddress, string userAgent,
        CancellationToken ct = default) {
        return await Table
            .FirstOrDefaultAsync(t => t.IpAddress == ipAddress &&
                                      t.UserAgent == userAgent &&
                                      t.IsActive &&
                                      !t.Deleted &&
                                      t.ExpirationDate > DateTime.UtcNow, ct);
    }
}