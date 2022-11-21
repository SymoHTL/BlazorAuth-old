namespace Model.Entities.Auth;

[Table("USERS")]
public class User {
    [Key]
    [Column("ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("USERNAME", TypeName = "VARCHAR(32)")]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    [Column("EMAIL", TypeName = "VARCHAR(50)")]
    public string Email { get; set; } = null!;


    [Required]
    [MinLength(6)]
    [Column("PASSWORD_HASH", TypeName = "TEXT")]
    public string PasswordHash { get; set; } = null!;

    [Required] [NotMapped] [MinLength(6)] public string LoginPassword { get; set; } = null!;


    public List<Token> Tokens { get; set; } = new();
    public List<RoleClaim> RoleClaims { get; set; } = new();

    [NotMapped] public IEnumerable<string> PlainRoles => RoleClaims.Select(x => x.Role.Identifier);


    public User ClearSensitiveData() {
        //PasswordHash = null!;
        return this;
    }

    public static string HashPassword(string plainPassword) {
        var salt = BC.GenerateSalt(8);
        return BC.HashPassword(plainPassword, salt);
    }

    public static bool VerifyPassword(string plainPassword, string hashedPassword) {
        return BC.Verify(plainPassword, hashedPassword);
    }
}