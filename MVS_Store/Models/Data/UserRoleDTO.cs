using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVS_Store.Models.Data
{
    [Table("TableUserRoles")]
    public class UserRoleDTO
    {
        [Key, Column(Order = 0)]
        public int UserID { get; set; }
        [Key, Column(Order = 1)]
        public int RoleID { get; set; }

        [ForeignKey("UserID")]
        public virtual UserDTO User { get; set; }
        [ForeignKey("RoleID")]
        public virtual RoleDTO Role { get; set; }
    }
}