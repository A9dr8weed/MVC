using System.ComponentModel.DataAnnotations.Schema;

namespace MVS_Store.Models.Data
{
    [Table("TableProducts")]
    public class ProductDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public string ImageName { get; set; }

        // Назначаємо зовнішній ключ
        [ForeignKey("CategoryID")]
        public virtual CategoryDTO Category { get; set; }
    }
}