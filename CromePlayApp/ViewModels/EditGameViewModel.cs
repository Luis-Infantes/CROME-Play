using CromePlayApp.Dtos.Calendar;
using System.ComponentModel.DataAnnotations;

namespace CromePlayApp.ViewModels
{
    public class EditGameViewModel
    {

        public int GameId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string? GameName { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500)]
        public string? GameDescription { get; set; }



    }


}
