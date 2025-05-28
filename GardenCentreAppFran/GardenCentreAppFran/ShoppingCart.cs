using System.Collections.Generic;
using System.Linq;

namespace GardenCentreAppFran.Models
{
    public class ShoppingCart
    {
        public List<Product> Items { get; private set; } = new List<Product>();

        public void AddProduct(Product product)
        {
            Items.Add(product);
        }

        public void RemoveProduct(Product product)
        {
            Items.Remove(product);
        }

        public double GetTotalPrice()
        {
            return Items.Sum(item => item.Price);
        }

        public void ClearCart()
        {
            Items.Clear();
        }
    }
}
