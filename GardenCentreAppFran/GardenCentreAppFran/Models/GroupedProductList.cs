using System.Collections.ObjectModel;

namespace GardenCentreAppFran.Models
{
    public class GroupedProductList : ObservableCollection<Product>
    {
        public string Key { get; private set; }

        public GroupedProductList(string key, List<Product> products) : base(products)
        {
            Key = key;
        }
    }
}
