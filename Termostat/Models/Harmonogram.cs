using System.ComponentModel.DataAnnotations;

namespace Termostat.Models
{
    public class Harmonogram : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nazwa { get; set; }

        [Range(0, 23)]
        public int Start { get; set; }

        [Range(1, 24)]
        public int End { get; set; }

        [Range(18, 35)]
        public int DocelowaTemperatura { get; set; }
        public bool IsActive { get; set; } = true; 

        [Required]
        public int MieszkanieId { get; set; }

        public virtual Mieszkanie? Mieszkanie { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Start >= End)
            {
                yield return new ValidationResult(
                    "Wartość 'Start' musi być mniejsza niż 'End'.",
                    new[] { nameof(Start), nameof(End) });
            }
        }
      
    }
    public class HarmonogramViewModel
    {
        public Harmonogram Harmonogram { get; set; }
        public decimal CenaDzienna { get; set; }
        public decimal CenaTygodniowa { get; set; }
        public decimal CenaMiesieczna { get; set; }
    }
    public class PodsumowanieKosztowViewModel
    {
        public decimal LacznyKosztDzienny { get; set; }
        public decimal LacznyKosztTygodniowy { get; set; }
        public decimal LacznyKosztMiesieczny { get; set; }
    }
}
