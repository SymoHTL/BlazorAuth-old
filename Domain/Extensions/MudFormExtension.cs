namespace Domain.Extensions;

public static class MudFormExtension {
    public static async Task<bool> AreFormsValidAsync(this MudForm[] forms) {
        foreach (var form in forms)
            await form.Validate();
        return forms.Select(f => f.IsValid).All(a => a);
    }

    public static async Task<bool> AreFormsValidAsync(this List<MudForm> forms) {
        foreach (var form in forms)
            await form.Validate();
        return forms.Select(f => f.IsValid).All(a => a);
    }

    public static async Task<bool> IsFormValidAsync(this MudForm form) {
        await form.Validate();
        return form.IsValid;
    }
}