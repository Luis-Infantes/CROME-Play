using System.ComponentModel.DataAnnotations;

namespace CromePlayApp.ViewModels
{
    public class AddNewClubViewModel
    {
        public int ClubId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string? ClubName { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [StringLength(50)]
        public string? ClubEmail { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500)]
        public string? ClubDescription { get; set; }


    }
}
