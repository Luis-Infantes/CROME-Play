using CromePlayApp.Data;
using CromePlayApp.Domain.Calendar;
using CromePlayApp.Dtos.Calendar;
using Microsoft.EntityFrameworkCore;

namespace CromePlayApp.Services.Calendar

{   //Clase Servicio para desarrollar e implementar la lógica de todos los métodos CRUD de la interface de calendar
    public class CalendarService : ICalendarService
    {
        private readonly ApplicationDbContext _db;

        public CalendarService(ApplicationDbContext db)
        {
            _db = db; 
        }



        public async Task<IEnumerable<CalendarEvent>> GetAllAsync(int clubId)
        {

            return await _db.CalendarEvents
                .Include(e => e.Game)
                .Where(e => e.Game != null && e.Game.ClubId == clubId)
                .OrderBy(e => e.Start)
                .ToListAsync();

        }




        public async Task<CalendarEvent?> GetByIdAsync(int id, int clubId)
        {
            return await _db.CalendarEvents
                            .FirstOrDefaultAsync(e => e.Id == id && e.GameId == clubId);
        }





        public async Task<CalendarEvent> CreateAsync(int clubId, CreateEventDto dto)
        {
            if (dto.End < dto.Start)
                throw new ArgumentException("End no puede ser anterior a Start.");


            // Validar que el juego existe y pertenece al club indicado
            var game = await _db.Games.FirstOrDefaultAsync(g => g.GameId == dto.GameId && g.ClubId == clubId);
            if (game == null)
                throw new ArgumentException("El juego no existe o no pertenece a este club.");

            var entity = new CalendarEvent
            {
                Title = dto.Title,
                AvailablePlace = dto.AvailablePlace,
                Start = dto.Start,
                End = dto.End,
                Price = dto.Price,
                Type = dto.Type,
                IsAllDay = dto.IsAllDay,
                CreatedAt = DateTime.UtcNow,
                GameId = dto.GameId,

            };


            _db.CalendarEvents.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }









        public async Task<CalendarEvent?> UpdateAsync(int id, int clubId, UpdateEventDto dto)
        {
            if (dto.End < dto.Start)
                throw new ArgumentException("End no puede ser anterior a Start.");

            var entity = await _db.CalendarEvents
                                  .FirstOrDefaultAsync(e => e.Id == id && e.GameId == clubId);
            if (entity is null) return null;

            entity.Title = dto.Title;
            entity.Start = dto.Start;
            entity.End = dto.End;
            entity.Price = dto.Price;
            entity.Type = dto.Type;
            entity.IsAllDay = dto.IsAllDay;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return entity;
        }






        public async Task<bool> DeleteAsync(int id, int clubId)
        {
            var entity = await _db.CalendarEvents
                                  .FirstOrDefaultAsync(e => e.Id == id && e.Game!.ClubId == clubId);
            if (entity is null) return false;

            _db.CalendarEvents.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }






    }
}

