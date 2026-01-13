using CromePlayApp.Data;
using CromePlayApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace CromePlayApp.Controllers
{
    //Controlador base para poder gestionar la base de datos con una clase abstracta
    public abstract class BaseController : Controller
    {
        protected readonly ApplicationDbContext _db;

        public BaseController (ApplicationDbContext db)
        {
            _db = db;
        }


        //Método para poder gestionar el cambio de color de la app
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var setting = _db.AppSettings.FirstOrDefault();
            if (setting == null)
            {
                setting = new AppSettings(); // Valor por defecto
                _db.AppSettings.Add(setting);
                _db.SaveChanges();
            }

            ViewBag.CurrentColor = setting.BackgroundClass;

            base.OnActionExecuting(context);
        }

    }
}
