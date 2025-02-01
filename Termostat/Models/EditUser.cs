namespace Termostat.Models
{
    public class EditUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public List<RoleCheckbox> Roles { get; set; } = new List<RoleCheckbox>();
    }

    public class RoleCheckbox
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
