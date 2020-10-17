using MVS_Store.Models.Data;

namespace MVS_Store.Models.ViewModels.Shop
{
    public class CategoryViewModel
    {
        public CategoryViewModel ()
        {

        }

        public CategoryViewModel (CategoryDTO row)
        {
            ID = row.ID;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}