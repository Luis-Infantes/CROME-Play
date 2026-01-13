using System.ComponentModel.DataAnnotations;

namespace CromePlayApp.Dtos.Calendar
{
    public class UpdateEventDto
    {

        [Required, StringLength(100)]
        public string Title { get; set; } = default!;


        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe haber al menos 1 plaza.")]
        public int AvailablePlace { get; set; }

        public int Price { get; set; }


        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        public string? Type { get; set; }
        public bool IsAllDay { get; set; } = false;

        [Required]
        public int GameId { get; set; }

    }
}
