using CromePlayApp.Data;
using CromePlayApp.Dtos.Calendar;
using CromePlayApp.Services.Calendar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CromePlayApp.Controllers.Api
{
    [Route("api/v1/calendar")]
    [ApiController]
    [Authorize(Roles ="Masterclub,Admin")]
    public class CalendarController : ControllerBase
    {

        private readonly ICalendarService _service;
        private readonly ApplicationDbContext _db;

        public CalendarController(ICalendarService service, ApplicationDbContext db) 
        {
            _service = service;
            _db = db;
        }



        // Helper para no repetir lógica de obtener userId en cada método
        private string GetUserId()
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("No se pudo obtener el Id de usuario.");
            return userId;
        }





        //Comprobación para ver que la API funciona
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { status = "ok", service = "calendar", time = DateTime.UtcNow });
        }



        //DELEGAR LOS SERVICIOS DE CADA METODO DE LA API 


        //Listar todos los eventos  delos juegos

        [HttpGet("events")]
        public async Task<IActionResult> GetAll()
        {

            // 1) Email del usuario autenticado
            var userEmail = User.Identity!.Name;

            // 2) Buscar el club del usuario
            var userClub = await _db.Clubes.FirstOrDefaultAsync(c => c.ClubEmail == userEmail);
            if (userClub == null)
                return NotFound("No se encontró el club para este usuario.");

            var clubId = userClub.ClubId;

            // 3) Pedir eventos por clubId (nueva firma del servicio)
            var items = await _service.GetAllAsync(clubId);

            // 4) Devolver los eventos
            return Ok(items);

        }



        //Listar eventos por Id

        [HttpGet("events/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userEmail = User.Identity!.Name;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var club = await _db.Clubes.FirstOrDefaultAsync(c => c.ClubEmail == userEmail);
            if (club is null) return NotFound();


            var item = await _service.GetByIdAsync(id, club.ClubId);
            return item is null ? NotFound() : Ok(item);
        }






        //Crear un nuevo evento de un juego en concreto


        [HttpPost("events")]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Obtener club del usuario autenticado (mismo patrón que en GameList y en GET /events)
            var userEmail = User.Identity!.Name;
            var userClub = await _db.Clubes.FirstOrDefaultAsync(c => c.ClubEmail == userEmail);
            if (userClub == null)
                return NotFound("No se encontró el club para este usuario.");

            var clubId = userClub.ClubId;

            try
            {
                var created = await _service.CreateAsync(clubId, dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                // Reglas de negocio (ej. End < Start, juego fuera del club, etc.)
                return BadRequest(new { error = ex.Message });
            }
        }






        //Actualizar un evento de un juego en concreto

        [HttpPut("events/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);



            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();


            var club = await _db.Clubes.FirstOrDefaultAsync(c => c.ClubEmail == userEmail);
            if (club is null) return NotFound();


            try
            {
                var updated = await _service.UpdateAsync(id, club.ClubId, dto);
                return updated is null ? NotFound() : Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        //Eliminar un evento de un juego en concreto

        [HttpDelete("events/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {

            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();


            var club = await _db.Clubes.FirstOrDefaultAsync(c => c.ClubEmail == userEmail);
            if (club is null) return NotFound();


          
            var ok = await _service.DeleteAsync(id, club.ClubId);
            return ok ? NoContent() : NotFound();
        }


    }
}
