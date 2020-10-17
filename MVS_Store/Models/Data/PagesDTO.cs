using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVS_Store.Models.Data
{
    [Table("Pages")] // явно вказуємо з якою таблицею буде працювати клас
    public class PagesDTO
    {
        [Key] // явно вказуємо що це початковий ключ
        public int ID { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Body { get; set; }
        public int Sorting { get; set; }
        public bool HasSidebar { get; set; }
    }
}