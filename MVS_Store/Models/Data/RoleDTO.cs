using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVS_Store.Models.Data
{
    [Table("TableRoles")]
    public class RoleDTO
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }    
    }
}