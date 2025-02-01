using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Termostat.Models
{
    public class Mieszkanie
    {
        public int Id { get; set; }

        [Range(0, 5, ErrorMessage = "Liczba okien musi być między 0 a 5")]
        public int LiczbaOkien { get; set; }

        [Range(1, 5, ErrorMessage = "Liczba pokoi musi być między 1 a 5")]
        public int LiczbaPokoi { get; set; }

        [Range(10, 17, ErrorMessage = "Temperatura bazowa musi być między 10 a 17")]
        public int BazowaTemperatura { get; set; }

        // Właściciel mieszkania
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        // Lista współlokatorów (relacja wiele-do-wielu)
        public ICollection<MieszkanieWspolokator> Wspolokatorzy { get; set; } = new List<MieszkanieWspolokator>();
        public virtual ICollection<Harmonogram> Harmonogramy { get; set; } = new List<Harmonogram>();
    }
}
