using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVS_Store.Models.Data
{
    [Table("TableSidebar")]
    public class SidebarDTO
    {
        [Key]
        public int ID { get; set; }
        public string Body { get; set; }
    }
}