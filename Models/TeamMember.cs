using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BR_Architects.Models
{
    public class TeamMember
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ChucVu { get; set; }

        public string GioiThieu { get; set; }

        public string LinkAnh { get; set; }

        public virtual User User { get; set; }
    }
}
