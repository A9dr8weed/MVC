using MVS_Store.Models.Data;
using System.Web.Mvc;

namespace MVS_Store.Models.ViewModels.Pages
{
    public class SidebarViewModel
    {
        public SidebarViewModel()
        {
        }

        public SidebarViewModel(SidebarDTO row)
        {
            ID = row.ID;
            Body = row.Body;
        }

        public int ID { get; set; }
        
        [AllowHtml] // дозволяє HTML теги 
        public string Body { get; set; }
    }
}