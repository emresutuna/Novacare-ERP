namespace NovacareERP.Application.Settings;

public sealed class SettingsUserViewModel
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = "";
    public string Email { get; init; } = "";
    public string PhoneNumber { get; init; } = "";
    public bool IsActive { get; init; }
    public string RoleName { get; init; } = "";
}
