using System.ComponentModel.DataAnnotations;

namespace BR_Architects.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string TenProject { get; set; }
        public string LinkAnh { get; set; }
        public string MoTaChiTiet { get; set; }
        public string DiaDiem { get; set; }

        public int? NamHoanThanh { get; set; }
    }
}
