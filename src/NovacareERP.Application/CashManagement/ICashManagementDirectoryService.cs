namespace NovacareERP.Application.CashManagement;

public interface ICashManagementDirectoryService
{
    CashManagementWorkspaceViewModel GetWorkspace();
    void AddAccount(CashAccountFormViewModel form);
}
