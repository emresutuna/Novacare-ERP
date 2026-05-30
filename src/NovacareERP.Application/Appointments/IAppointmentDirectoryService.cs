namespace NovacareERP.Application.Appointments;

public interface IAppointmentDirectoryService
{
    AppointmentListViewModel GetList(string? searchTerm, DateOnly? filterDate);
    AppointmentListItemViewModel? GetById(Guid id);
    void Add(AppointmentFormViewModel form);
    bool Update(Guid id, AppointmentFormViewModel form);
    bool Delete(Guid id);
}
