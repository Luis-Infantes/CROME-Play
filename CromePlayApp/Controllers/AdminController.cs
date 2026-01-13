using CromePlayApp.Data;
using CromePlayApp.Models;
using CromePlayApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using Microsoft.Extensions.DependencyInjection;

namespace CromePlayApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly RoleManager<IdentityRole>? _roleManager;
        private readonly UserManager<IdentityUser>? _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ApplicationDbContext db, ILogger<AdminController> logger) : base(db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }






        //METODO PARA BORRAR CUENTAS REGISTRADAS Y QUE A SU VEZ ELIMINEN EL TOKEN DE MANERA QUE PUEDA VOLVERSE A USAR PARA REALIZAR UN NUEVO REGISTRO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CleanIdentityOrphans()
        {
            //Acceso solo para el Admin
            if (_userManager == null)
                return StatusCode(500, new { ok = false, message = "UserManager no disponible." });

            // Contadores para feedback
            int deletedUsers = 0;
            int deletedTokens = 0;

            // Transacción para garantizar atomicidad
            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {

                // Usuarios de Identity que no tienen registro en PendingUsers ni ningún tipo de rol asignado
                var orphanUsers = await _db.Users
                    .Where(u =>
                        !_db.PendingUsers.Any(p => p.PendingUserEmail == u.Email) &&
                        !_db.Members.Any(m => m.MemberEmail == u.Email) 
                       
                    )
                    .ToListAsync();


                foreach (var user in orphanUsers)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                        deletedUsers++;
                    else
                        _logger.LogWarning("No se pudo borrar IdentityUser {UserId}: {Errors}",
                            user.Id, string.Join("; ", result.Errors.Select(e => e.Description)));
                }

                //Tokens huérfanos cuyo UserId no existe en AspNetUsers
                var orphanTokens = _db.UserTokens
                    .Where(t => !_db.Users.Any(u => u.Id == t.UserId))
                    .ToList(); 
                if (orphanTokens.Count > 0)
                {
                    _db.UserTokens.RemoveRange(orphanTokens);
                    deletedTokens += orphanTokens.Count;
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return Json(new
                {
                    ok = true,
                    title = "Limpieza completada",
                    text = $"Usuarios borrados: {deletedUsers}. Tokens borrados: {deletedTokens}.",
                    icon = "success",
                    deletedUsers,
                    deletedTokens
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                var msg = ex.GetBaseException().Message;
                _logger.LogError(ex, "Error limpiando orphans: {Message}", msg);

                return StatusCode(500, new
                {
                    ok = false,
                    title = "Error",
                    text = msg,
                    icon = "error"
                });
            }
        }











        //METODO PARA CARGAR EL FORMULARIO DE CREACION DE ROLES Y LA TABLA DE ROLES
        [HttpGet]
        //Carga lista de roles y muestra el formulario
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager!.Roles.ToListAsync();

            // Flag para bloquear el formulario cuando ya existan ambos
            var masterExists = await _roleManager.RoleExistsAsync("MasterClub");
            var memberExists = await _roleManager.RoleExistsAsync("Member");
            ViewBag.AllCreated = masterExists && memberExists;

            return View(roles); 
        }



        //Crea solo si procede y vuelve al Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            var allowed = new[] { "MasterClub", "Member" };

            if (!string.IsNullOrWhiteSpace(roleName) && allowed.Contains(roleName))
            {
                // Chequeo secuencial para evitar concurrencia del DbContext
                var masterExists = await _roleManager!.RoleExistsAsync("MasterClub");
                var memberExists = await _roleManager.RoleExistsAsync("Member");

                if (!(masterExists && memberExists) && !await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    TempData["RoleMsg"] = $"Rol '{roleName}' creado.";
                }
                else
                {
                    TempData["RoleMsg"] = "Los roles requeridos ya están creados o el rol existe.";
                }
            }
            else
            {
                TempData["RoleMsg"] = "Solo se permiten 'MasterClub' o 'Member'.";
            }

            return RedirectToAction(nameof(Index)); // PRG
        }




        //----------------------------------


        //METODO PARA ASIGNAR UN NUEVO ROL 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToRole(string userEmail, string roleName)
        {
            TempData["Trace"] = $"CHK1: Inicio AddUserToRole email='{userEmail}', role='{roleName}'\n";


            //Validación básica
            if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(roleName))
            {
                TempData["Error"] = "El email y el rol son obligatorios.";
                return RedirectToAction(nameof(Index));
            }
            roleName = roleName.Trim();


            //Localizar usuario
            var user = await _userManager!.FindByEmailAsync(userEmail);
            if (user is null)
            {
                TempData["Error"] = $"No se encontró un usuario con el email '{userEmail}'.";
                return RedirectToAction(nameof(Index));
            }

            //Validar rol existe
            var roleExists = await _roleManager!.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                TempData["Error"] = $"El rol '{roleName}' no existe.";
                return RedirectToAction(nameof(Index));
            }


            //Asignar rol si no lo tiene
            var alreadyInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (!alreadyInRole)
            {
                var addResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!addResult.Succeeded)
                {
                    TempData["Error"] = string.Join("; ", addResult.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(Index));
                }
            }



            //Migración para MEMBER
            if (string.Equals(roleName, "Member", StringComparison.OrdinalIgnoreCase))
            {
                await using var tx = await _db.Database.BeginTransactionAsync();
                try
                {
                    var pendings = await _db.PendingUsers
                        .Where(p => p.IdentityUserId == user.Id || p.PendingUserEmail == user.Email)
                        .ToListAsync();

                    PendingUser? pending = null;

                    if (pendings.Count > 0)
                    {
                        pending = pendings
                            .OrderByDescending(p => !string.IsNullOrWhiteSpace(p.IdentityUserId))
                            .First();

                        if (string.IsNullOrWhiteSpace(pending.IdentityUserId))
                            pending.IdentityUserId = user.Id;

                        var duplicates = pendings.Where(p => p != pending).ToList();
                        if (duplicates.Count > 0)
                            _db.PendingUsers.RemoveRange(duplicates);
                    }

                    var alreadyMember = await _db.Members.AnyAsync(m => m.IdentityUserId == user.Id);

                    if (!alreadyMember)
                    {
                        var member = new Member
                        {
                            IdentityUserId = user.Id,
                            MemberEmail = user.Email ?? pending?.PendingUserEmail ?? string.Empty,
                        };

                        _db.Members.Add(member);
                    }

                    if (pending is not null)
                        _db.PendingUsers.Remove(pending);

                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();

                    TempData["Ok"] = "Rol 'Member' asignado y usuario migrado a Members.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    await tx.RollbackAsync();
                    TempData["Error"] = "Error guardando en DB: " + (dbEx.InnerException?.Message ?? dbEx.Message);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    TempData["Error"] = "Error migrando usuario a Members: " + ex.Message;
                    return RedirectToAction(nameof(Index));
                }
            }



            //Migración MASTERCLUB -> CLUB
            if (string.Equals(roleName, "MasterClub", StringComparison.OrdinalIgnoreCase))
            {
                await using var tx = await _db.Database.BeginTransactionAsync();
                try
                {
                    var candidates = await _db.PendingUsers
                        .Where(p =>
                            p.IdentityUserId == user.Id ||
                            p.PendingUserEmail == (user.Email ?? userEmail))
                        .ToListAsync();

                    PendingUser? pending = null;

                    if (candidates.Count > 0)
                    {
                        pending = candidates
                            .OrderByDescending(p => !string.IsNullOrWhiteSpace(p.IdentityUserId))
                            .First();

                        if (string.IsNullOrWhiteSpace(pending.IdentityUserId))
                            pending.IdentityUserId = user.Id;

                        var duplicates = candidates.Where(p => p != pending).ToList();
                        if (duplicates.Count > 0)
                            _db.PendingUsers.RemoveRange(duplicates);
                    }

                    //Derivar ClubEmail
                    var clubEmail = (userEmail ?? user.Email ?? pending?.PendingUserEmail)?.Trim();
                    if (string.IsNullOrWhiteSpace(clubEmail))
                    {
                        TempData["Error"] = "No se pudo determinar el ClubEmail.";
                        await tx.RollbackAsync();
                        return RedirectToAction(nameof(Index));
                    }

                    //Eliminar pending principal
                    if (pending is not null)
                        _db.PendingUsers.Remove(pending);

                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();

                    TempData["Ok"] = "Rol 'MasterClub' asignado y Club del usuario creado/asegurado.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    await tx.RollbackAsync();
                    TempData["Error"] = "Error guardando en DB: " + (dbEx.InnerException?.Message ?? dbEx.Message);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    TempData["Error"] = "Error migrando usuario a Club (MasterClub): " + ex.Message;
                    return RedirectToAction(nameof(Index));
                }
            }

            //Sí asigna otros roles sin migración
            TempData["Ok"] = $"Rol '{roleName}' asignado (sin migración).";
            return RedirectToAction(nameof(Index));
        }

        //--------------------------------------------------------------


        //METODO PARA LA CREACION DE UNA NUEVA SECCION

        public IActionResult AddNewClub() {

            var vm = new AddNewClubViewModel();
            return View(vm);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewClub(AddNewClubViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                
                return View(vm);
            }

            //Crea un nuevo club con las caracteristicas definidas
            var club = new Club
            {
                ClubId = vm.ClubId,
                ClubName = vm.ClubName,
                ClubEmail = vm.ClubEmail!,
                ClubDescription = vm.ClubDescription!
            };

            //Añade un nuevo club y guarda los cambios
            _db.Clubes.Add(club);
            await _db.SaveChangesAsync();

            // Redirige a la lista (o detalle) tras crear.
            return RedirectToAction(nameof(RoleTable));
        }


        //--------------------------------------------------------------


        //METODO PARA ELIMINAR UN CLUB (Combinado con un método con AJAX creado en el archivo JS)

        [HttpPost]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

            var club = await _db.Clubes.FindAsync(id);
            if (club is null)
            {
                if (isAjax)
                    return Ok(new { success = false, message = "El club no existe o no está disponible." });

                TempData["Error"] = "El club no existe o no está disponible.";
                return RedirectToAction("RoleTable");
            }


            var userClub = await _userManager!.FindByEmailAsync(club.ClubEmail!);
            if (userClub != null)
            {
                // Esto elimina logins, claims y tokens asociados cuando borras el usuario
                var result = await _userManager.DeleteAsync(userClub);
                if (!result.Succeeded)
                    return StatusCode(500, new { ok = false, message = "No se pudo borrar el usuario de Identity." });
            }



            _db.Clubes.Remove(club);
            await _db.SaveChangesAsync();

            if (isAjax)
                return Ok(new { success = true, message = "Club eliminado correctamente." });

            TempData["Success"] = "Club eliminado correctamente.";
            return RedirectToAction("RoleTable");
        }














        //---------------------------------------------------

        //ACCESO A TODOS LOS USUARIOS CON SUS ROLES ASIGNADOS
        [HttpGet]
        public async Task<IActionResult> RoleTable(int? page, string? sortOrder, string? searchString)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = searchString;

            //Creado filtros
            ViewBag.Titlesort = sortOrder == "title_asc" ? "title_desc" : "title_asc";
            ViewBag.Usersort = sortOrder == "user_asc" ? "user_desc" : "user_asc";
            ViewBag.Membersort = sortOrder == "member_asc" ? "member_desc" : "member_asc";

            //Creando paginador
            int pageSize = 4;
            int pageNumber = page ?? 1;
            if (pageNumber < 1) pageNumber = 1;


            var Query1 = _db.Games.Include(i => i.Club).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var q = searchString.Trim();

                Query1 = Query1.Where(b =>
                     (b.Club != null && b.Club.ClubName != null && b.Club.ClubName.Contains(q)) ||
                     (b.Club != null && b.Club.ClubEmail.Contains(q))
                 );
            }

            //Filtrado por nombre e email de club
            Query1 = sortOrder switch
            {

                "title_desc" => Query1.OrderByDescending(b => b.Club!.ClubName),
                "title_asc" => Query1.OrderBy(b => b.Club!.ClubName),

                "user_desc" => Query1.OrderByDescending(b => b.Club!.ClubEmail),
                "user_asc" => Query1.OrderBy(b => b.GameName),

                _ => Query1.OrderBy(b => b.Club!.ClubName)
            };



            // Totales y límites
            var totalItems = await Query1.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            // Página actual
            var items = await Query1
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var clubesPaged = new StaticPagedList<Game>(items, pageNumber, pageSize, totalItems);




            //Consulta para filtrar por email de member
            var Query2 = _db.Members.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var q = searchString.Trim();

                Query2 = Query2.Where(b => b.MemberEmail != null);
            }

            Query2 = sortOrder switch
            {

                "member_desc" => Query2.OrderByDescending(b => b.MemberEmail),
                "member_asc" => Query2.OrderBy(b => b.MemberEmail),

                _ => Query2.OrderBy(b => b.MemberEmail)
            };

            var clubes = await _db!.Clubes.ToListAsync();
            var members = await Query2.ToListAsync();
            var pendingUsers = await _db!.PendingUsers.ToListAsync();
            var allUsers = await _db.Users.ToListAsync();

            //Usuarios sin rol
            var usersWithoutRoles = await _db.Users
                        .Where(u => !_db.UserRoles.Any(ur => ur.UserId == u.Id))
                        .ToListAsync();


            var vm = new AdminRoleUserViewModel
            {
                Clubes = clubes,
                ClubsPaged = clubesPaged,
                Games = items,
                Members = members,
                IdentityUsers = allUsers,
                PendingUsers = pendingUsers,
                FondoFileName = "fondo_4.jpg"

            };


            return View(vm);
        }


        //---------------------------------------------------------------------------------------



        //METODO PARA ELIMINAR USUARIO (Combinado con un método con AJAX creado en el archivo JS)

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {

            var club = _db.Games.Include(c => c.Club).FirstOrDefault(c => c.ClubId == id);


            if (club != null)
            {
                if (club.Club != null)
                    _db.Clubes.Remove(club.Club);

                _db.Games.Remove(club);
                await _db.SaveChangesAsync();
                return Ok();
            }

            return BadRequest();
        }



        //METODO PARA ELIMINAR UN GAME (Combinado con un método con AJAX creado en el archivo JS)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                // Carga solo para comprobar existencia por GameId
                var game = await _db.Games
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.GameId == id);

                // Si no existe ya, lo tratamos como éxito (idempotente)
                if (game == null)
                    return Json(new { ok = true, deletedId = id, entity = "game", title = "¡Eliminado!", text = "Juego eliminado.", icon = "success" });

                // Borrado directo en DB (evita problemas de concurrencia/tracking)
                var affected = await _db.Games
                    .Where(g => g.GameId == id)
                    .ExecuteDeleteAsync();

                // Si por carrera afectó 0 filas, lo damos por eliminado igualmente
                if (affected == 0)
                    return Json(new { ok = true, deletedId = id, entity = "game", title = "¡Eliminado!", text = "Juego eliminado.", icon = "success" });

                return Json(new { ok = true, deletedId = id, entity = "game", title = "¡Eliminado!", text = "Juego eliminado.", icon = "success" });
            }
            catch (Exception ex)
            {
                var msg = ex.GetBaseException().Message;
                return StatusCode(500, new { ok = false, title = "Error", text = msg, icon = "error" });
            }
        }


        //---------------------------------------------------------------------



        //METODO PARA ELIMINAR UN MIEMBRO CON ROLE MEMBER (Se compagina con un método que hay en el archivo JS)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMember(int id)
        {
            //Cargar el miembro a eliminar
            var member = await _db.Members.FindAsync(id);
            if (member == null)
                return NotFound(new { ok = false, message = "Miembro no encontrado", id });

            //Cargar las inscripciones del miembro (con el Club para actualizar plazas)
            var enrollments = await _db.Enrollments
                .Where(e => e.MemberId == id)
                .ToListAsync();

            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {

                var user = await _userManager!.FindByEmailAsync(member.MemberEmail!); 
                if (user != null)
                {
                    // Esto elimina logins, claims y tokens asociados cuando borras el usuario
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                        return StatusCode(500, new { ok = false, message = "No se pudo borrar el usuario de Identity." });
                }



                //Eliminar el miembro
                _db.Members.Remove(member);

                //Guardar y confirmar
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return Json(new { ok = true, deletedId = id, entity = "member" });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();

                // Devuelve el error al cliente para depuración (puedes dejar solo un log en producción)
                var baseMsg = ex.GetBaseException().Message;
                return StatusCode(500, new { ok = false, message = baseMsg });
            }
        }


        //---------------------------------------------------------------------------




        //METODO PARA ELIMINAR UN USER EN PENDING (Se compagina con un método que hay en el archivo JS)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePendingUser(int id)
        {
            try
            {
                var pendingUser = await _db.PendingUsers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.PendingUserId == id);

                // Si no existe ya, lo tratamos como éxito idempotente
                if (pendingUser == null)
                    return Json(new { ok = true, deletedId = id, entity = "pending", title = "¡Eliminado!", text = "Usuario pendiente eliminado.", icon = "success" });

                var email = pendingUser.PendingUserEmail;
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var user = await _userManager!.FindByEmailAsync(email);
                    if (user != null)
                    {
                        var result = await _userManager.DeleteAsync(user);
                        if (!result.Succeeded)
                            return StatusCode(500, new { ok = false, title = "Error", text = "No se pudo borrar el usuario de Identity.", icon = "error" });
                    }
                }

                var affected = await _db.PendingUsers
                    .Where(c => c.PendingUserId == id)
                    .ExecuteDeleteAsync();

                // Si no afectó filas, igualmente lo damos por eliminado (idempotente)
                if (affected == 0)
                    return Json(new { ok = true, deletedId = id, entity = "pending", title = "¡Eliminado!", text = "Usuario pendiente eliminado.", icon = "success" });

                return Json(new { ok = true, deletedId = id, entity = "pending", title = "¡Eliminado!", text = "Usuario pendiente eliminado.", icon = "success" });
            }
            catch (Exception ex)
            {
                var msg = ex.GetBaseException().Message;
                return StatusCode(500, new { ok = false, title = "Error", text = msg, icon = "error" });
            }
        }






    }
}

