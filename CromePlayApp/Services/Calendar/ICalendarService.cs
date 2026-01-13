using CromePlayApp.Domain.Calendar;
using CromePlayApp.Dtos.Calendar;

namespace CromePlayApp.Services.Calendar
{
    //Diseño de una interface para decalarar los métodos CRUD de la API
    public interface ICalendarService
    {
        
        Task<IEnumerable<CalendarEvent>> GetAllAsync(int clubId);
        Task<CalendarEvent?> GetByIdAsync(int id, int clubId);
        Task<CalendarEvent> CreateAsync(int clubId, CreateEventDto dto);
        Task<CalendarEvent?> UpdateAsync(int id, int clubId, UpdateEventDto dto);
        Task<bool> DeleteAsync(int id, int clubId);
    }
}
