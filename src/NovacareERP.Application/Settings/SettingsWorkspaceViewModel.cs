namespace NovacareERP.Application.Settings;

public sealed class SettingsWorkspaceViewModel
{
    public IReadOnlyList<SettingsUserViewModel> Users { get; init; } = [];
    public IReadOnlyList<SettingsListItemViewModel> Definitions { get; init; } = [];
    public IReadOnlyList<SettingsListItemViewModel> Integrations { get; init; } = [];
    public IReadOnlyList<SettingsListItemViewModel> Templates { get; init; } = [];
}
