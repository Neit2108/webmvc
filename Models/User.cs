using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BR_Architects.Models
{
    public class User
    {
        public int Id { get; set; }

        public string HoTen { get; set; }

        public string Email { get; set; }

        public string Pass { get; set; }

        public string SecretKey { get; set; }

        public bool IsAdmin { get; set; } = false;

        public virtual ICollection<TeamMember> TeamMembers { get; set; }
    }
}