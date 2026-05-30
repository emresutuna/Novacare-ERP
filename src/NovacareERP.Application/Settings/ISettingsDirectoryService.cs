namespace NovacareERP.Application.Settings;

public interface ISettingsDirectoryService
{
    SettingsWorkspaceViewModel GetWorkspace();
    SettingsUserViewModel? GetUser(Guid id);
}
