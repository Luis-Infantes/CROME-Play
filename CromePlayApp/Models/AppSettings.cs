namespace CromePlayApp.Models
{
    //Modelo de datos que define el color de fondo de la app
    //Creado para poder manejarlo en un método junto con la viewmodel SelectColorViewModel
    public class AppSettings
    {
        public int Id { get; set; }

        public string? BackgroundClass { get; set; } = "white"; // Color por defecto

        public string? HomeBackgroundFileName { get; set; }
    }
}
