using CromePlayApp.Domain.Calendar;
using CromePlayApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CromePlayApp.ViewModels
{

public class EventEnrollmentViewModel
{

        [Required]
        public int MemberId { get; set; }

        [Required]
        public int CalendarEventId { get; set; }

        [Required]
        public int GameId { get; set; }

        //Datos para la vista 
        public string? MemberName { get; set; }

        public string? EventTitle { get; set; }

        public DateTime? EventStart { get; set; }
        public DateTime? EventEnd { get; set; }

        public int? AvailablePlace { get; set; }

        public int Price { get; set; }

        public bool? IsAllDay { get; set; }

        public string? EventType { get; set; }

        public bool IsAlreadyEnrolled { get; set; } = false;
    }


}
