using System.ComponentModel.DataAnnotations;

namespace BR_Architects.Models
{
    public class Contact
    {
        public int Id { get; set; }

        public string NguoiGui { get; set; }

        public string Email { get; set; }

        public string TieuDe { get; set; }

        public string NoiDung { get; set; }

        public bool DaDoc { get; set; } = false;
    }
}
