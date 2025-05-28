using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GardenCentreAppFran.Models;
using GardenCentreAppFran.Data;
using System.IO;

namespace GardenCentreAppFran
{
    public partial class ShoppingPage : ContentPage
    {
        private readonly ShoppingCart _cart = new ShoppingCart();
        private readonly DatabaseHelper _database;
        private readonly string _customerName;
        private readonly bool _isCorporate;
        private ObservableCollection<GroupedProductList> _groupedProducts = new ObservableCollection<GroupedProductList>();
        private ObservableCollection<CartItem> _cartProducts = new ObservableCollection<CartItem>();
        private ObservableCollection<string> _previousPurchases = new ObservableCollection<string>();

        public ShoppingPage(string customerName, bool isCorporate)
        {
            InitializeComponent();
            _customerName = customerName;
            _isCorporate = isCorporate;

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gardenCentre.db3");
            _database = new DatabaseHelper(dbPath);

            
            LoadProducts();

            
            cartCollectionView.ItemsSource = _cartProducts;
            previousPurchasesListView.ItemsSource = _previousPurchases;

            if (isCorporate)
                LoadPreviousPurchases();
        }

        private void LoadProducts()
        {
            List<Product> products = new List<Product>
            {
                new Product { Name = "Rose Bush", Price = 12.99, Category = "Plants" },
                new Product { Name = "Lavender Plant", Price = 8.50, Category = "Plants" },
                new Product { Name = "Garden Trowel", Price = 15.00, Category = "Tools" },
                new Product { Name = "Pruning Shears", Price = 20.50, Category = "Tools" },
                new Product { Name = "Organic Fertiliser", Price = 25.00, Category = "Garden Care" },
                new Product { Name = "Insect Repellent Spray", Price = 18.75, Category = "Garden Care" }
            };

            _groupedProducts = new ObservableCollection<GroupedProductList>(
                products.GroupBy(p => p.Category ?? "Uncategorized")
                .Select(g => new GroupedProductList(g.Key, g.ToList()))
            );

            
            productCollectionView.ItemsSource = _groupedProducts;
        }

        private async void LoadPreviousPurchases()
        {
            var client = await _database.GetCorporateClientAsync(_customerName, "");
            if (client != null && !string.IsNullOrEmpty(client.Purchases))
            {
                var purchases = client.Purchases.Split(';').ToList();
                foreach (var purchase in purchases)
                    _previousPurchases.Add(purchase);
            }
        }

        private void OnAddProduct(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Product selectedProduct)
            {
                _cart.AddProduct(selectedProduct);

                var existingCartItem = _cartProducts.FirstOrDefault(c => c.Name == selectedProduct.Name);
                if (existingCartItem != null)
                {
                    existingCartItem.Quantity++;
                    existingCartItem.TotalPrice += selectedProduct.Price;
                }
                else
                {
                    _cartProducts.Add(new CartItem
                    {
                        Name = selectedProduct.Name,
                        Price = selectedProduct.Price,
                        Quantity = 1,
                        TotalPrice = selectedProduct.Price
                    });
                }

                UpdateCart();
            }
        }

        private void OnRemoveProduct(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Product selectedProduct)
            {
                var existingCartItem = _cartProducts.FirstOrDefault(c => c.Name == selectedProduct.Name);
                if (existingCartItem != null)
                {
                    if (existingCartItem.Quantity > 1)
                    {
                        existingCartItem.Quantity--;
                        existingCartItem.TotalPrice -= selectedProduct.Price;
                    }
                    else
                    {
                        _cartProducts.Remove(existingCartItem);
                    }

                    _cart.RemoveProduct(selectedProduct);
                    UpdateCart();
                }
            }
        }

        private void UpdateCart()
        {
            double total = _cart.GetTotalPrice();
            totalPriceLabel.Text = $"Total: €{total:F2}";
            checkoutButton.IsEnabled = _cartProducts.Count > 0;

            
            cartCollectionView.ItemsSource = null;
            cartCollectionView.ItemsSource = _cartProducts;
        }

        private async void OnCheckout(object sender, EventArgs e)
        {
            double total = _cart.GetTotalPrice();
            string summary = string.Join("\n", _cartProducts.Select(p => $"{p.Name} x{p.Quantity}: €{p.TotalPrice:F2}"));

            await DisplayAlert("Checkout Summary", $"{summary}\n\nTotal: €{total:F2}", "OK");

            if (_isCorporate)
            {
                var client = await _database.GetCorporateClientAsync(_customerName, "");
                if (client != null)
                {
                    client.Purchases += string.Join(";", _cartProducts.Select(p => $"{p.Name} x{p.Quantity} (€{p.TotalPrice:F2})")) + ";";
                    await _database.UpdateCorporateClientAsync(client);
                }
            }

            _cart.ClearCart();
            _cartProducts.Clear();
            UpdateCart();
        }
    }
}
