namespace Model.Entities.Auth;

[Table("TOKENS")]
public class Token {
    public Token() {
        // generate a 255 character random string using a cryptographically strong random sequence of values with special characters
        Identifier = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!§$%&/()=?}][{€@'+*~", 255)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("IDENTIFIER", TypeName = "VARCHAR(256)")]
    public string Identifier { get; set; }

    [Column("USER_ID")] public int UserId { get; set; }

    public User User { get; set; } = null!;
    
    [Required]
    [Column("DEVICE_NAME", TypeName = "VARCHAR(30)")]
    public string DeviceName { get; set; } = null!;

    [Column("EXPIRES")] public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(14);

    [Column("IS_REVOKED")] public bool IsRevoked { get; set; }

    [Column("IS_ACTIVE")] public bool IsActive => !IsRevoked && !IsExpired;

    [Column("LAST_LOGIN")] public DateTime LastLogin { get; set; } = DateTime.UtcNow;

    [NotMapped] public bool IsExpired => DateTime.UtcNow >= Expires;


    public void Update() {
        LastLogin = DateTime.UtcNow;
    }
}