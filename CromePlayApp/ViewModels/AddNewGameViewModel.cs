using CromePlayApp.Dtos.Calendar;
using System.ComponentModel.DataAnnotations;

namespace CromePlayApp.ViewModels
{
    //Modelo viewmodel para poder crear nuevos clubes con validaciones
    public class AddNewGameViewModel
    {
        public int GameId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string? GameName { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500)]
        public string? GameDescription { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una sección")]
        public int ClubId { get; set; }

        public string? ClubEmail { get; set; }

    }


}
