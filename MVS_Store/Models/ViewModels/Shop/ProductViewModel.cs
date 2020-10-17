using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVS_Store.Models.Data;

namespace MVS_Store.Models.ViewModels.Shop
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {

        }

        public ProductViewModel(ProductDTO row)
        {
            ID = row.ID;
            Name = row.Name;
            Slug = row.Slug;
            Description = row.Description;
            Price = row.Price;
            CategoryName = row.CategoryName;
            CategoryID = row.CategoryID;
            ImageName = row.ImageName;
        }

        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        [Required]
        [DisplayName("Category")]
        public int CategoryID { get; set; }
        [DisplayName("Image")]
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}