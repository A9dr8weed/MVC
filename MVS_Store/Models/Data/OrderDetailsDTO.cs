using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVS_Store.Models.Data
{
    [Table("TableOrderDetails")]
    public class OrderDetailsDTO
    {
        [Key]
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("OrderID")]
        public virtual OrderDTO Orders { get; set; }
        [ForeignKey("UserID")]
        public virtual UserDTO Users { get; set; }
        [ForeignKey("ProductID")]
        public virtual ProductDTO Products { get; set; }
    }
}