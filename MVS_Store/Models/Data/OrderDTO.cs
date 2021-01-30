using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVS_Store.Models.Data
{
    [Table("TableOrders")]
    public class OrderDTO
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserID")]
        public virtual UserDTO Users { get; set; }
    }
}