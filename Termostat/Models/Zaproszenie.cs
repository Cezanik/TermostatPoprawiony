using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Termostat.Models
{
    public class Zaproszenie
    {
        public int Id { get; set; }

        [Required]
        public string NadawcaId { get; set; }
        public IdentityUser Nadawca { get; set; }

        [Required]
        public string OdbiorcaId { get; set; }
        public IdentityUser Odbiorca { get; set; }

        public DateTime DataWyslania { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; } = "Oczekujące"; // Oczekujące/Zaakceptowane/Odrzucone
    }
    public class WyslijProsbeViewModel
    {
        [Required(ErrorMessage = "Wybierz użytkownika")]
        public string WybranyUzytkownikId { get; set; }

        public List<SelectListItem> DostepniUzytkownicy { get; set; } = new List<SelectListItem>();
    }
}
