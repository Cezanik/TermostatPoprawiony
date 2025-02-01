using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Termostat.Models
{
    public class MieszkanieWspolokator
    {
        public int MieszkanieId { get; set; }
        public Mieszkanie Mieszkanie { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
    public class AddWspolokatorViewModel
    {
        public int MieszkanieId { get; set; }
        public string SelectedUserId { get; set; }
        public List<SelectListItem> AvailableUsers { get; set; } = new List<SelectListItem>();
    }
}
