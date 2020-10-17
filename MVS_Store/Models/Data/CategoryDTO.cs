using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVS_Store.Models.Data
{
    // Клас контекста даних для категорій
    [Table("TableCategories")]
    public class CategoryDTO
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}