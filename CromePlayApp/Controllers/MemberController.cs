using CromePlayApp.Data;
using CromePlayApp.Domain.Calendar;
using CromePlayApp.Models;
using CromePlayApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CromePlayApp.Controllers
{
    [Authorize(Roles = "Member, MasterClub")]
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public MemberController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        //METODO PARA VALIDAR QUE EL MEMBER LOGADO PUEDA ACCEDER A SU PERFIL
        public async Task<IActionResult> Index()
        {
            ViewData["Registrado"] = false;

            if (User.Identity!.Name != null)
            {
                var member = await _db.Members.FirstOrDefaultAsync(s => s.MemberEmail == User.Identity!.Name);
                if (member != null)
                {
                    ViewBag.MemberId = member.MemberId;
                    ViewData["Registrado"] = true;

                }
            }


            return View();
        }


        //METODO PARA EDITAR EL PERFIL Y MOSTRAR EL LISTADO DE EVENTOS INSCRITOS

        public async Task<IActionResult> EditProfile(int id)
        {
            var member = await _db.Members
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
                return NotFound();

            var enrollments = await _db.Enrollments
                .Where(e => e.MemberId == id)
                .Include(e => e.Game)
                .ToListAsync();

            var events = await _db.CalendarEvents
                .AsNoTracking()
                .Where(ev => enrollments.Select(e => e.GameId).Contains(ev.GameId))
                .OrderBy(ev => ev.Start)
                .Select(s => new MemberEnrollmentsListViewModel
                {
                    MemberId = member.MemberId,
                    EventTitle = s.Title,
                    EventStart = s.Start,
                    EventEnd = s.End,
                    Price = s.Price,
                    AvailablePlace = s.AvailablePlace,
                    IsAllDay = s.IsAllDay,
                    EventType = s.Type,
                    GameId = s.GameId,
                    CalendarEventId = s.Id
                })
                .ToListAsync();


            ViewBag.MemberId = member.MemberId;
            ViewBag.MemberName = member.MemberName;
            ViewBag.MemberEmail = member.MemberEmail;
            ViewBag.MemberAddress = member.MemberAddress;
            ViewBag.MemberPhone = member.MemberPhone;


            return View(events);
        }




        //METODO PARA EDITAR EL PERFIL
       
        [HttpPost]
        public async Task<IActionResult> EditProfile(int memberId, string memberName, string memberAddress, string memberPhone)
        {
            var existingMember = await _db.Members
                .FirstOrDefaultAsync(m => m.MemberId == memberId);

            if (existingMember == null)
                return NotFound();

            existingMember.MemberName = memberName;
            existingMember.MemberAddress = memberAddress;
            existingMember.MemberPhone = memberPhone;

            await _db.SaveChangesAsync();

            return RedirectToAction("EditProfile", new { id = memberId });
        }





        //METODO PARA ELIMINAR UNA SUSCRIPCION A UN EVENTO

        [HttpPost]
        public async Task<IActionResult> DeleteEnrollment(int id, int memberId)
        {
            var isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

            var evnt = await _db.CalendarEvents.FirstOrDefaultAsync(e => e.Id == id);
            if (evnt is null)
            {
                if (isAjax)
                    return Ok(new { success = false, title = "No encontrado", text = "El evento no existe.", icon = "warning" });

                TempData["Error"] = "El evento no existe.";
                return RedirectToAction("EditProfile", new { id = memberId });
            }

            var enrollment = await _db.Enrollments.FirstOrDefaultAsync(en => en.MemberId == memberId && en.GameId == evnt.GameId);
            if (enrollment is null)
            {
                if (isAjax)
                    return Ok(new { success = false, title = "Sin inscripción", text = "No se encontró inscripción.", icon = "info" });

                TempData["Error"] = "No se encontró inscripción.";
                return RedirectToAction("EditProfile", new { id = memberId });
            }

            _db.Enrollments.Remove(enrollment);
            evnt.AvailablePlace += 1;

            await _db.SaveChangesAsync();

            if (isAjax)
                return Ok(new { success = true, title = "¡Eliminado!", text = $"Se eliminó la inscripción de \"{evnt.Title}\".", icon = "success" });

            TempData["Success"] = "Inscripción eliminada correctamente.";
            return RedirectToAction("EditProfile", new { id = memberId });
        }








        //METODO PARA AÑADIR UNA NUEVA SUSCRIPCION EN UN EVENTO

        [HttpGet]
        public async Task<IActionResult> NewEnrollment(string? sortOrder, string? searchString)
        {
            var identityUserId = _userManager.GetUserId(User);
            var member = await _db.Members
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdentityUserId == identityUserId);
            if (member == null) return Forbid();


            
            //VieBag para los filtros y el buscador
            ViewBag.CurrentSort = sortOrder;
            ViewBag.Currentfilter = searchString;

            //Filtramos los eventos por tipo y fecha de inicio
            ViewBag.Typesort = sortOrder == "Type_asc" ? "Type_desc" : "Type_asc";
            ViewBag.Startsort = sortOrder == "Start_asc" ? "Start_desc" : "Start_asc";


            //Consulta para poder filtrar el evento a traves del buscador y mediante el titulo del evento
            var Query1 = _db.CalendarEvents.Include(i => i.Game).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var filter = searchString.Trim().ToLower();

                Query1 = Query1.Where(b =>
                    b.Title!.ToLower().Contains(filter)
                
                 );
            }



            //Filtro del evento para tipo y fecha inicio
            Query1 = sortOrder switch
            {

                "Type_desc" => Query1.OrderByDescending(b => b.Type),
                "Type_asc" => Query1.OrderBy(b => b.Type),

                "Start_desc" => Query1.OrderByDescending(b => b.Start),
                "Start_asc" => Query1.OrderBy(b => b.Start),

                _ => Query1.OrderBy(b => b.Type)
            };



            Query1 = Query1.Where(e => e.Start >= DateTime.UtcNow); 
            var events = await Query1
                .AsNoTracking()
                .ToListAsync();




            // View model para visualizar los datos en la vista
            var vms = new List<EventEnrollmentViewModel>();
            foreach (var ev in events)
            {
                var already = await _db.Enrollments
                    .AnyAsync(en => en.MemberId == member.MemberId && en.GameId == ev.GameId);

                vms.Add(new EventEnrollmentViewModel
                {
                    MemberId = member.MemberId,
                    CalendarEventId = ev.Id,
                    GameId = ev.GameId,
                    MemberName = member.MemberName,
                    EventTitle = ev.Title,
                    EventStart = ev.Start,
                    EventEnd = ev.End,
                    AvailablePlace = ev.AvailablePlace,
                    Price = ev.Price,
                    IsAllDay = ev.IsAllDay,
                    EventType = ev.Type,
                    IsAlreadyEnrolled = already
                });
            }

            return View(vms);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewEnrollment(EventEnrollmentViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var calendarEvent = await _db.CalendarEvents
                .FirstOrDefaultAsync(e => e.Id == vm.CalendarEventId);
            if (calendarEvent == null) return NotFound();

            var identityUserId = _userManager.GetUserId(User);
            var member = await _db.Members
                .FirstOrDefaultAsync(m => m.IdentityUserId == identityUserId);
            if (member == null) return Forbid();

            vm.MemberId = member.MemberId;
            vm.GameId = calendarEvent.GameId;

            //Evitamos que se inscriba en un evento en el cual ya lo esta
            var already = await _db.Enrollments
                .AnyAsync(en => en.MemberId == vm.MemberId && en.GameId == vm.GameId);

            if (already)
            {
                ModelState.AddModelError(string.Empty, "Ya estás inscrito/a en este evento.");
                // Rellenar info para repintar la vista
                vm.EventTitle = calendarEvent.Title;
                vm.EventStart = calendarEvent.Start;
                vm.EventEnd = calendarEvent.End;
                vm.AvailablePlace = calendarEvent.AvailablePlace;
                vm.Price = calendarEvent.Price;
                vm.IsAllDay = calendarEvent.IsAllDay;
                vm.EventType = calendarEvent.Type;
                vm.IsAlreadyEnrolled = true;
                return View(vm);
            }

            //Nos aseguramos de que queden plazas para dicho evento
            if (calendarEvent.AvailablePlace <= 0)
            {
                ModelState.AddModelError(string.Empty, "No quedan plazas disponibles.");
                vm.EventTitle = calendarEvent.Title;
                vm.EventStart = calendarEvent.Start;
                vm.EventEnd = calendarEvent.End;
                vm.AvailablePlace = calendarEvent.AvailablePlace;
                vm.Price = calendarEvent.Price;
                vm.IsAllDay = calendarEvent.IsAllDay;
                vm.EventType = calendarEvent.Type;
                return View(vm);
            }

            //Creamos el nuevo registro del evento y restamos la plaza 
            var enrollment = new Enrollment
            {
                MemberId = vm.MemberId,
                GameId = vm.GameId
            };

            _db.Enrollments.Add(enrollment);
            calendarEvent.AvailablePlace -= 1;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Si se producce un conflicto devolvemos el catch con la vista de nuevo para que se pueda poder intentar de nuevo
                ModelState.AddModelError(string.Empty, "Se ha producido un conflicto de concurrencia. Inténtalo de nuevo.");
                vm.EventTitle = calendarEvent.Title;
                vm.EventStart = calendarEvent.Start;
                vm.EventEnd = calendarEvent.End;
                vm.AvailablePlace = calendarEvent.AvailablePlace;
                vm.Price = calendarEvent.Price;
                vm.IsAllDay = calendarEvent.IsAllDay;
                vm.EventType = calendarEvent.Type;
                return View(vm);
            }


            return RedirectToAction(nameof(EditProfile), "Member", new { id = member.MemberId });
        }
    }
}




