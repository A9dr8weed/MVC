using MVS_Store.Models.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVS_Store.Models.ViewModels.Pages
{
    public class PageViewModel // моделька
    {
        public PageViewModel() { } // конструктор без пераметрів (за замовчуванням)

        // присвоїли моделі значення які отримає PagesDTO з БД
        public PageViewModel(PagesDTO row) // конструктор з параметрами
        {
            ID = row.ID;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }

        public int ID { get; set; }
        
        [Required] // елемент обов'язковий для заповнення
        [StringLength(50, MinimumLength = 3)] // додатковий захист (максимальна довжина може бути 50, а мінімальна 3
        public string Title { get; set; }
        
        public string Slug { get; set; }
        
        [Required] // елемент обов'язковий для заповнення
        [StringLength(int.MaxValue, MinimumLength = 3)] // додатковий захист
        [AllowHtml]
        public string Body { get; set; }
        
        public int Sorting { get; set; }
        
        [Display(Name = "Sidebar")]
        public bool HasSidebar { get; set; }
    }
}