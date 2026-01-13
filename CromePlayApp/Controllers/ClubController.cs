using CromePlayApp.Data;
using CromePlayApp.Domain.Calendar;
using CromePlayApp.Dtos.Calendar;
using CromePlayApp.Models;
using CromePlayApp.Services.Calendar;
using CromePlayApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CromePlayApp.Controllers
{
    [Authorize(Roles = "MasterClub")]
    public class ClubController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser>? _userManager;
        private readonly ICalendarService _calendarService;

        public ClubController(ApplicationDbContext db, UserManager<IdentityUser>? userManager, ICalendarService calendarService)
        {
            _db = db;
            _userManager = userManager;
            _calendarService = calendarService;
        }


        
        //Solo para acceso del correo ligado a ClubEmail mediante Identity
        public async Task<IActionResult> Index()
        {
            ViewData["Registrado"] = false;

            if (User.Identity!.Name != null)
            {
                var masterClub = await _db.Clubes.FirstOrDefaultAsync(s => s.ClubEmail == User.Identity!.Name);
                if (masterClub != null)
                {
                    ViewBag.clubId = masterClub.ClubId;
                    ViewData["Registrado"] = true;

                }
            }

            return View();
        }



        //METODO PARA VER PERFIL DEL MASTERCLUB

        [HttpGet]
        public async Task<IActionResult> SeeProfile(int id)
        {
            var profile = await _db!.Clubes
                .FirstOrDefaultAsync(c => c.ClubId == id);

            if (profile is null)
                return NotFound();

            return View(profile);
        }

        //-------------------------------------------------



        //METODO PARA LA GESTION CRUD DEL GESTOR TAREAS

        //Mostrar tareas
        [HttpGet]
        public async Task<IActionResult> Tasks (int id) 
        {
            var clubId = id;

            var tasks = await _db.TaskItems
                .Where(w => w.ClubId == clubId && !w.IsCompleted)
                .OrderBy(o => o.Id)
                .ToListAsync();

            ViewBag.clubId = clubId;

            return View(tasks);
        
        }

        //Añadir una nueva tarea

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddTask(int clubId, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return Ok(new { success = false, title = "Descripción vacía", text = "Escribe una tarea.", icon = "warning" });

            var clubExiste = await _db.Clubes.AnyAsync(c => c.ClubId == clubId);
            if (!clubExiste)
                return Ok(new { success = false, title = "Club no válido", text = $"El club {clubId} no existe.", icon = "warning" });

            var task = new TaskItem { ClubId = clubId, Description = description.Trim() };
            _db.TaskItems.Add(task);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                title = "Tarea añadida",
                text = "Se creó la tarea.",
                icon = "success",
                data = new { task.Id, task.Description }
            });
        }


        //Editar una tarea

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EditTask(int id, string description)
        {
            var task = await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == id);
            if (task is null)
                return Ok(new { success = false, title = "No encontrada", text = "La tarea no existe.", icon = "warning" });

            if (string.IsNullOrWhiteSpace(description))
                return Ok(new { success = false, title = "Descripción vacía", text = "Escribe una tarea.", icon = "warning" });

            task.Description = description.Trim();
            await _db.SaveChangesAsync();

            return Ok(new { success = true, title = "Actualizada", text = "Tarea editada.", icon = "success" });
        }


        //Completar o eliminar una tarea

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == id);
            if (task is null)
                return Ok(new { success = false, title = "No encontrada", text = "La tarea no existe.", icon = "warning" });

            _db.TaskItems.Remove(task); // completar = eliminar
            await _db.SaveChangesAsync();

            return Ok(new { success = true, title = "¡Completada!", text = "La tarea se eliminó.", icon = "success" });
        }


        //----------------------------------------------------------



        //METODO PARA EDITAR EL PERFIL DE MASTERCLUB
        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            var profile = await _db!.Clubes
                .FirstOrDefaultAsync(c => c.ClubId == id);

            if (profile is null)
                return NotFound();

            return View(profile);
        }


        [HttpPost]
        public async Task<IActionResult> EditProfile([Bind("ClubId,ClubEmail,ClubName,ClubDescription")] Club input)
        {

            if (!ModelState.IsValid) return View(input);


            var profile = await _db.Clubes
                .FirstOrDefaultAsync(c => c.ClubId == input.ClubId);

            if (profile == null)
            {
                return NotFound();
            }

            //Actualiza las propiedades permitidas

            if(!string.IsNullOrWhiteSpace(input.ClubEmail))
                profile.ClubEmail = input.ClubEmail.Trim();

            profile.ClubName = input.ClubName;
            profile.ClubDescription = input.ClubDescription;

            await _db.SaveChangesAsync();

            return RedirectToAction("SeeProfile", new { id = input.ClubId });

        }



        //----------------------------------------------------------------------


        //LISTADO DE JUEGOS

        [HttpGet]
        public async Task<IActionResult> GameList(int id)
        {
            var userEmail = User.Identity!.Name;


            if (!User.IsInRole("MasterClub"))
               return Forbid();

                //Buscar el club del usuario
                var userclub = await _db.Clubes.FirstOrDefaultAsync(c => c.ClubEmail == userEmail);

                if (userclub == null)
                    return NotFound("No se encontró el club para este usuario.");

                //Buscar el club relacionado con el game
                var clubId = userclub.ClubId;

                var gamesClub = await _db.Games
                    .Where(c => c.ClubId == clubId)
                    .OrderBy(o => o.GameName)
                    .ToListAsync();

                var events = await _calendarService.GetAllAsync(clubId);

                //Pintamos la vista con la nueva viewmodel
                var viewmodel = new GamesListViewModel
                {

                    Club = await _db.Clubes.FirstOrDefaultAsync(s => s.ClubId == clubId),
                    GamesList = gamesClub,


                    Events = events.Select(e => new CalendarEventItemViewModel
                    {
                        Id = e.Id,
                        Title = e.Title,
                        AvailablePlace = e.AvailablePlace,
                        Price = e.Price,
                        Start = e.Start,
                        End = e.End,
                        Type = e.Type,
                        IsAllDay = e.IsAllDay
                    }).OrderBy(o => o.Title).ToList()


                };



            return View(viewmodel);


        }



        //---------------------------------------------------------


        //METODO PARA AÑADIR UN NUEVO JUEGO


        [HttpGet]
        public async Task<IActionResult> AddNewGame()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                ModelState.AddModelError(string.Empty, "No se pudo determinar el usuario actual.");
                return View(new AddNewGameViewModel());
            }

            // Buscar un game existente del usuario para deducir su clubId
            var anyUserClub = await _db.Clubes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClubEmail == userEmail);

            if (anyUserClub is null)
            {

                ModelState.AddModelError(string.Empty, "No se pudo deducir tu sección (no tienes clubes previos).");
                return View(new AddNewGameViewModel { ClubEmail = userEmail });
            }

            // Prellenar los ocultos (opcional, solo para depurar/mostrar)
            var vm = new AddNewGameViewModel
            {
                ClubEmail = userEmail,
                ClubId = anyUserClub.ClubId
            };

            return View(vm);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewGame(AddNewGameViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                ModelState.AddModelError(string.Empty, "No se pudo determinar el usuario actual.");
                return View(vm);
            }

            var anyUserClub = await _db.Clubes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClubEmail == userEmail);



            if (anyUserClub is null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo deducir tu sección (no tienes clubes previos).");
                return View(vm);
            }

            //Confirmar que el club existe 
            var sektionExists = await _db.Clubes
                .AsNoTracking()
                .AnyAsync(s => s.ClubId == anyUserClub.ClubId);

            if (!sektionExists)
            {
                ModelState.AddModelError(string.Empty, "La sección asociada al usuario no existe.");
                return View(vm);
            }

            //Evitar duplicados por nombre del mismo club
            var duplicate = await _db.Games
                .AnyAsync(c => c.ClubId == anyUserClub.ClubId && c.GameName == vm.GameName);

            if (duplicate)
            {
                ModelState.AddModelError(nameof(vm.GameName), "Ya existe un club con ese nombre en tu sección.");
                return View(vm);
            }

            var newGame = new Game
            {
                GameName = vm.GameName!,
                GameDescription = vm.GameDescription!,
                ClubId = anyUserClub.ClubId

            };

            _db.Games.Add(newGame);
            await _db.SaveChangesAsync();

            return RedirectToAction("GameList");
        }




        //----------------------------------------------------------



        //METODO PARA AÑADIR UN NUEVO EVENTO



        [HttpGet]
        public async Task<IActionResult> CreateEvent()
        {
            var userEmail = User.Identity!.Name;

            var userClub = await _db.Clubes.FirstOrDefaultAsync(c => c.ClubEmail == userEmail);
            if (userClub == null)
                return NotFound("No se encontró el club para este usuario.");

            var clubId = userClub.ClubId;

            //Lista de juegos para el select
            ViewBag.Games = await _db.Games
                .Where(g => g.ClubId == clubId)
                .OrderBy(g => g.GameName)
                .Select(g => new SelectListItem
                {
                    Value = g.GameId.ToString(),
                    Text = g.GameName
                })
                .ToListAsync();

            //Valores por defecto del formulario
            var dto = new CreateEventDto
            {
                Title = string.Empty,
                AvailablePlace = 1,
                Price = 0,
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(1),
                Type = string.Empty,
                IsAllDay = false,
                     
            };

            return View(dto);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventDto dto)
        {
            if (!ModelState.IsValid)
            {
                var userEmail = User.Identity!.Name;
                var userClub = await _db.Clubes.FirstOrDefaultAsync(c => c.ClubEmail == userEmail);
                if (userClub != null)
                {
                    ViewBag.Games = await _db.Games
                        .Where(g => g.ClubId == userClub.ClubId)
                        .OrderBy(g => g.GameName)
                        .Select(g => new SelectListItem
                        {
                            Value = g.GameId.ToString(),
                            Text = g.GameName
                        })
                        .ToListAsync();
                }
                return View(dto);
            }

            var userEmailOk = User.Identity!.Name;
            var userClubOk = await _db.Clubes.FirstOrDefaultAsync(c => c.ClubEmail == userEmailOk);
            if (userClubOk == null)
                return NotFound("No se encontró el club para este usuario.");

            try
            {
                await _calendarService.CreateAsync(userClubOk.ClubId, dto);
                TempData["Success"] = "Evento creado correctamente.";
                return RedirectToAction(nameof(GameList));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                ViewBag.Games = await _db.Games
                    .Where(g => g.ClubId == userClubOk.ClubId)
                    .OrderBy(g => g.GameName)
                    .Select(g => new SelectListItem
                    {
                        Value = g.GameId.ToString(),
                        Text = g.GameName
                    })
                    .ToListAsync();

                return View(dto);
            }
        }





        //-------------------------------------------------------------


        //METODO PARA EDITAR UN JUEGO

        [HttpGet]
        public async Task<IActionResult> EditGame(int id)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return NotFound(); 
            }

            
            var anyUserClub = await _db.Clubes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClubEmail == userEmail);

            if (anyUserClub is null)
            {
                return Forbid(); 
            }

            var game = await _db.Games
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.GameId == id);

            if (game is null)
            {
                return NotFound();
            }


            var vm = new EditGameViewModel
            {
                GameId = game.GameId,
                GameName = game.GameName,
                GameDescription = game.GameDescription

            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> EditGame(EditGameViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                ModelState.AddModelError(string.Empty, "No se pudo determinar el usuario actual.");
                return View(vm);
            }

            var anyUserClub = await _db.Clubes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClubEmail == userEmail);

            if (anyUserClub is null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo deducir tu club.");
                return View(vm);
            }

            var game = await _db.Games
                .FirstOrDefaultAsync(c => c.GameId == vm.GameId);

            if (game is null)
            {
                return NotFound();
            }



            // Evitar duplicados de nombre dentro de la sección (excluyendo el propio club)
            var duplicate = await _db.Games
                .AnyAsync(c => c.ClubId == game.ClubId
                            && c.GameId != game.GameId
                            && c.GameName == vm.GameName);

            if (duplicate)
            {
                ModelState.AddModelError(nameof(vm.GameName), "Ya existe un juego con ese nombre en tu club.");
                return View(vm);
            }

            // Actualizar campos editables
            game.GameName = vm.GameName!;
            game.GameDescription = vm.GameDescription!;

            // Guardar
            var affected = await _db.SaveChangesAsync();
            if (affected == 0)
            {
                ModelState.AddModelError(string.Empty, "No se pudo guardar los cambios.");
                return View(vm);
            }

            return RedirectToAction("GameList");
        }


        //----------------------------------------------------------------

        //EDITAR UN EVENTO

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            //Verificar el email a traves de Identity
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return NotFound(); 
            }

            //Comparar el email del club con el de Identity 
            var anyUserClub = await _db.Clubes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClubEmail == userEmail);

            if (anyUserClub is null)
            {
                return Forbid(); 
            }

            var eve = await _db.CalendarEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (eve is null)
                return NotFound();
           
            

            var vm = new UpdateEventDto
            {
                GameId = eve.GameId,
                Title = eve.Title,
                AvailablePlace = eve.AvailablePlace,
                Start = eve.Start,
                Price = eve.Price,
                End = eve.End,
                Type = eve.Type

            };

            return View(vm);
        }



        [HttpPost]
        public async Task<IActionResult> EditEvent(int id, UpdateEventDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userEmail = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                ModelState.AddModelError(string.Empty, "No se pudo determinar el usuario actual.");
                return View(dto);
            }

            // Deducir clubId del usuario desde un game previo suyo
            var anyUserClub = await _db.Clubes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClubEmail == userEmail);

            if (anyUserClub is null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo deducir tu juego.");
                return View(dto);
            }


            var eve = await _db.CalendarEvents
                .FirstOrDefaultAsync(e => e.Id == id);


            if (eve is null)
                return NotFound();
            

            // Actualizar campos editables
            eve.Title = dto.Title!;
            eve.AvailablePlace = dto.AvailablePlace!;
            eve.Price = dto.Price!;
            eve.Start = dto.Start!;
            eve.End = dto.End!;
            eve.Type = dto.Type!;

            // Guardar
            var affected = await _db.SaveChangesAsync();
            if (affected == 0)
            {
                ModelState.AddModelError(string.Empty, "No se pudo guardar los cambios.");
                return View(dto);
            }

            return RedirectToAction("GameList");
        }



        //--------------------------------------------------------------------



        //ELIMINAR UN JUEGO (Combinando con un método AJAX en el archivo JS)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

            var game = await _db.Games.FindAsync(id);
            if (game is null)
            {
                if (isAjax)
                    return Ok(new
                    {
                        success = false,
                        title = "No encontrado",
                        text = "El juego no existe o no está disponible.",
                        icon = "warning"
                    });

                TempData["Error"] = "El juego no existe o no está disponible.";
                return RedirectToAction("GameList");
            }

            _db.Games.Remove(game);
            await _db.SaveChangesAsync();

            if (isAjax)
                return Ok(new
                {
                    success = true,
                    title = "¡Eliminado!",
                    text = $"Se eliminó el juego \"{game.GameName}\".",
                    icon = "success"
                });

            TempData["Success"] = "Juego eliminado correctamente.";
            return RedirectToAction("GameList");
        }




        //-----------------------------------------------------------------


        //ELIMINAR UN EVENTO

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            //Usando peticiones con AJAX
            var isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                if (isAjax)
                    return Ok(new
                    {
                        success = false,
                        title = "No autorizado",
                        text = "Debe iniciar sesión para realizar esta acción.",
                        icon = "error"
                    });

                return Unauthorized();
            }
            var club = await _db.Clubes.FirstOrDefaultAsync(c => c.ClubEmail == userEmail);
            if (club is null)
            {
                if (isAjax)
                    return Ok(new
                    {
                        success = false,
                        title = "No autorizado",
                        text = "No se pudo determinar el club del usuario.",
                        icon = "error"
                    });

                TempData["Error"] = "No se pudo determinar el club del usuario.";
                return RedirectToAction("GameList");
            }


            var ok = await _calendarService.DeleteAsync(id, club.ClubId);

            if (!ok)
            {
                if (isAjax)
                    return Ok(new
                    {
                        success = false,
                        title = "No encontrado",
                        text = "El evento no existe o no pertenece a tu club.",
                        icon = "warning"
                    });

                TempData["Error"] = "El evento no existe o no pertenece a tu club.";
                return RedirectToAction("GameList");
            }

            if (isAjax)
                return Ok(new
                {
                    success = true,
                    title = "¡Eliminado!",
                    text = "Se eliminó el evento correctamente.",
                    icon = "success"
                });

            TempData["Success"] = "Evento eliminado correctamente.";
            return RedirectToAction("GameList");
        }



        //-----------------------------------------------------------------


        //METODO PARA VER MIEMBROS INSCRITOS EN EL EVENTO

        [HttpGet]

        public async Task<IActionResult> EventMembers(int id)
        {



            // Cargamos el evento
            var eve = await _db.CalendarEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);

            if (eve == null)
                return NotFound();

            // Consulta directa de inscripciones para el Game del evento
            var enrollments = await _db.Enrollments
                .Where(en => en.GameId == eve.GameId)
                .Include(en => en.Member)     
                .AsNoTracking()
                .ToListAsync();


            var vm = new EventMembersViewModel
            {
                EventId = eve.Id,
                Title = eve.Title,
                Start = eve.Start,
                End = eve.End,
                Members = enrollments
                    .Where(en => en.Member != null)
                    .Select(en => new MemberItem
                    {
                        MemberId = en.Member!.MemberId,
                        MemberName = en.Member.MemberName ?? "",
                        MemberEmail = en.Member.MemberEmail,
                        MemberAddress = en.Member.MemberAddress,
                        MemberPhone = en.Member.MemberPhone
                    })
                    .ToList()
            };


            return View(vm);

        }






    }
}