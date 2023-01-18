namespace Model.Entities.Auth;

[Table("TOKENS")]
public class Token {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }

    [Column("VALUE", TypeName = "VARCHAR(255)")]
    public string Value { get; set; }

    [Column("USER_ID")] public int UserId { get; set; }

    public User User { get; set; } = null!;

    [Column("EXPIRATION_DATE")] public DateTime ExpirationDate { get; set; }

    [Column("CREATION_DATE")] public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    [Column("LAST_LOGIN_DATE")] public DateTime LastLoginDate { get; set; } = DateTime.UtcNow;

    [Column("IS_ACTIVE")] public bool IsActive { get; set; } = true;

    [Column("DELETED")] public bool Deleted { get; set; }

    [Column("USER_AGENT")] public string UserAgent { get; set; }

    [Column("IP_ADDRESS")] public string IpAddress { get; set; }

    public Token(string userAgent, DateTime expirationDate, int userId, string ipAddress) {
        UserAgent = userAgent;
        ExpirationDate = expirationDate;
        UserId = userId;
        IpAddress = ipAddress;
        Value = GenerateToken();
    }

    public string FormatLastLogin() {
        var timeSpan = DateTime.UtcNow - LastLoginDate;
        return timeSpan.TotalMinutes switch {
            < 1 => "just now",
            < 2 => "1 minute ago",
            _ => timeSpan.TotalHours switch {
                < 1 => $"{timeSpan.Minutes} minutes ago",
                < 2 => "1 hour ago",
                _ => timeSpan.TotalDays switch {
                    < 1 => $"{timeSpan.Hours} hours ago",
                    < 2 => "1 day ago",
                    < 7 => $"{timeSpan.Days} days ago",
                    < 14 => "1 week ago",
                    < 21 => "2 weeks ago",
                    < 28 => "3 weeks ago",
                    < 60 => "1 month ago",
                    < 365 => $"{timeSpan.Days / 30} months ago",
                    _ => $"{timeSpan.Days / 365} years ago"
                }
            }
        };
    }

    public string FormatIsActive() {
        return IsActive ? "Yes" : "No";
    }

    private string GenerateToken() {
        return Guid.NewGuid().ToString() + Guid.NewGuid() + Guid.NewGuid() + Guid.NewGuid() + Guid.NewGuid();
    }

    public string GetIcon() {
        // check if the user agent is a mobile device
        if (CheckIfAndroid() || CheckIfIos() || CheckIfOtherMobile())
            return MobileIcon;
        if (CheckIfWindows() || CheckIfMac() || CheckIfLinux()) return DesktopIcon;
        return UnknownIcon;
    }
    
    public string GetDevice() {
        if (CheckIfAndroid()) return "Android";
        if (CheckIfIos()) return "iOS";
        if (CheckIfOtherMobile()) return "Other Mobile";
        if (CheckIfWindows()) return "Windows";
        if (CheckIfMac()) return "Mac";
        if (CheckIfLinux()) return "Linux";
        return "Unknown";
    }

    public bool CheckIfWindows() => UserAgent.Contains("Windows NT");
    public bool CheckIfMac() => UserAgent.Contains("Macintosh");
    public bool CheckIfLinux() => UserAgent.Contains("X11") || UserAgent.Contains("Linux");
    public bool CheckIfAndroid() => UserAgent.Contains("Android");

    public bool CheckIfIos() =>
        UserAgent.Contains("iPhone") || UserAgent.Contains("iPad") || UserAgent.Contains("iPod");

    public bool CheckIfOtherMobile() => UserAgent.Contains("Windows Phone") ||
                                  UserAgent.Contains("BlackBerry") ||
                                  UserAgent.Contains("Opera Mini") ||
                                  UserAgent.Contains("IEMobile");


    public const string UnknownIcon =
        """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--! Font Awesome Pro 6.2.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc. --><path d="M256 0C114.6 0 0 114.6 0 256s114.6 256 256 256s256-114.6 256-256S397.4 0 256 0zM256 464c-114.7 0-208-93.31-208-208S141.3 48 256 48s208 93.31 208 208S370.7 464 256 464zM256 336c-18 0-32 14-32 32s13.1 32 32 32c17.1 0 32-14 32-32S273.1 336 256 336zM289.1 128h-51.1C199 128 168 159 168 198c0 13 11 24 24 24s24-11 24-24C216 186 225.1 176 237.1 176h51.1C301.1 176 312 186 312 198c0 8-4 14.1-11 18.1L244 251C236 256 232 264 232 272V288c0 13 11 24 24 24S280 301 280 288V286l45.1-28c21-13 34-36 34-60C360 159 329 128 289.1 128z"/></svg>""";

    public const string DesktopIcon =
        """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512"><!--! Font Awesome Pro 6.2.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc. --><path d="M64 0C28.7 0 0 28.7 0 64V352c0 35.3 28.7 64 64 64H240l-10.7 32H160c-17.7 0-32 14.3-32 32s14.3 32 32 32H416c17.7 0 32-14.3 32-32s-14.3-32-32-32H346.7L336 416H512c35.3 0 64-28.7 64-64V64c0-35.3-28.7-64-64-64H64zM512 64V288H64V64H512z"/></svg>""";

    public const string MobileIcon =
        """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><!--! Font Awesome Pro 6.2.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2022 Fonticons, Inc. --><path d="M16 64C16 28.7 44.7 0 80 0H304c35.3 0 64 28.7 64 64V448c0 35.3-28.7 64-64 64H80c-35.3 0-64-28.7-64-64V64zM144 448c0 8.8 7.2 16 16 16h64c8.8 0 16-7.2 16-16s-7.2-16-16-16H160c-8.8 0-16 7.2-16 16zM304 64H80V384H304V64z"/></svg>""";
}