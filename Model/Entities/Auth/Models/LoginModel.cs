namespace Model.Entities.Auth.Models;

public class LoginModel {
    public LoginModel() {
    }

    public LoginModel(string email, string password) {
        Email = email;
        Password = password;
    }

    [Required] [EmailAddress] public string Email { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;
}