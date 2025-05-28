namespace GardenCentreAppFran.Models
{
    public class CartItem
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
    }
}
