using CromePlayApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Policy;
using static System.Collections.Specialized.BitVector32;

namespace CromePlayApp.ViewModels
{
    public class GamesListViewModel
    {

        public Club? Club { get; set; }

        public List<Game> GamesList { get; set; } = new List<Game>();


        //lista de eventos para pintar en la misma vista
        public List<CalendarEventItemViewModel> Events { get; set; } = new();



    }
}
