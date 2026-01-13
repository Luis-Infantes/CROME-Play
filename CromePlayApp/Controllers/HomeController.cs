using System.Diagnostics;
using CromePlayApp.Data;
using CromePlayApp.Models;
using CromePlayApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CromePlayApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env , ApplicationDbContext db): base(db) 
        {
            _logger = logger;
            _env = env;

        }

        // Acceso privado a la carpeta "images/logos".
        private string HomeImagesFolder => Path.Combine(_env.WebRootPath, "images", "logos");

        //Definimos el tipo de archivo que vamos a encontrar en la carpeta
        private static readonly string[] AllowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp", ".avif" };

        //Lista privada para cargar las imágenes
        private List<string> GetHomeBackgroundOptions()
        {
            if (!Directory.Exists(HomeImagesFolder))
                return new List<string>();

            return Directory.GetFiles(HomeImagesFolder)
                .Select(Path.GetFileName)
                .Where(f => f != null && AllowedExt.Contains(Path.GetExtension(f!), StringComparer.OrdinalIgnoreCase))
                .ToList()!;
        }



        //MÉTODO PARA GESTIONAR EL CAMBIO DEL LOGO PRINCIPAL DEL HOME A TRAVES DEL ADMIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult SetHomeBackground(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                TempData["HomeBgMsg"] = "Selecciona un fondo válido.";
                return RedirectToAction(nameof(Index));
            }

            var targetPath = Path.Combine(HomeImagesFolder, fileName);

            if (!System.IO.File.Exists(targetPath) ||
                !AllowedExt.Contains(Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase))
            {
                TempData["HomeBgMsg"] = "El archivo seleccionado no existe o el formato no es válido.";
                return RedirectToAction(nameof(Index));
            }

            var setting = _db.AppSettings.First();
            setting.HomeBackgroundFileName = fileName;
            _db.SaveChanges();

            TempData["HomeBgMsg"] = "Fondo del Home actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }





        //METODO PARA GESTIONAR LA VISTA INDEX DEL HOME
        public IActionResult Index()
        {
            var model = new SelectColorViewModel();

            //Recupera el color actual desde la base de datos por defecto
            var setting = _db.AppSettings.FirstOrDefault();
            if (setting == null)
            {
                setting = new AppSettings();

                _db.AppSettings.Add(setting);
                _db.SaveChanges();
            }

            ViewBag.CurrentColor = setting.BackgroundClass;

            var available = GetHomeBackgroundOptions();

            var currentName = setting.HomeBackgroundFileName;

            if(string.IsNullOrWhiteSpace(currentName) || !System.IO.File.Exists(Path.Combine(HomeImagesFolder, currentName)))
            {
                currentName = available.FirstOrDefault();
            }


            model.CurrentBackgroundName = currentName;
            model.CurrentBackgroundUrl = string.IsNullOrWhiteSpace(currentName)
                ? null
                : $"/images/logos/{currentName}";


            model.AvailableBackgrounds = available;


            return View(model);
        }


        //---------------------------------------



        //METODO PARA EL CAMBIO DE TONO DEL BACKGROUND DEL HOME (Combinado con un método en el archivo JS)
        [Authorize(Roles = "Admin")]
        [HttpPost]

        public IActionResult Changecolor([FromBody] SelectColorViewModel model)
        {
            var setting = _db.AppSettings.FirstOrDefault();
            if (setting != null)
            {
                setting.BackgroundClass = model.SelectedClass;
                _db.SaveChanges();
            }
            return Json(new { selectedClass = model.SelectedClass });
        }




    }


}

