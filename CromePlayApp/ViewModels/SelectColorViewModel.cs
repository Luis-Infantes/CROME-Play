namespace CromePlayApp.ViewModels
{
    //View model para manejar datos para el cambio de color del fondo de la app
    public class SelectColorViewModel
    {
        public string? SelectedClass { get; set; }
        public List<string> Classes { get; set; } = new List<string> 
        {
            "white", "blue", "green", "yellow", "gray"
        };

        public string? CurrentBackgroundName { get; set; }
        public string? CurrentBackgroundUrl { get; set; }

        public List<string> AvailableBackgrounds { get; set; } = new();
    }
}
