
namespace MVS_Store.Models.ViewModels.Cart
{
    public class CartViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;

        public string Image { get; set; }
    }
}