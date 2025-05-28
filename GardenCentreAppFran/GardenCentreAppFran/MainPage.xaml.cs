using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Linq;
using GardenCentreAppFran.Data;

namespace GardenCentreAppFran
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseHelper _database;

        public MainPage()
        {
            InitializeComponent();

            
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gardenCentre.db3");
            _database = new DatabaseHelper(dbPath);
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string name = nameEntry.Text?.Trim() ?? "";  
            string phone = phoneEntry.Text?.Trim() ?? ""; 
            bool isCorporate = corporateCheckBox.IsChecked;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            {
                await DisplayAlert("Error", "Please enter your name and phone number.", "OK");
                return;
            }

            if (isCorporate)
            {
                var existingClients = await _database.GetCorporateClientsAsync();

                
                var existingClient = existingClients.FirstOrDefault(c => c.Name == name && c.PhoneNumber == phone);
                if (existingClient != null)
                {
                    
                    await Navigation.PushAsync(new ShoppingPage(name, isCorporate));
                    return;
                }

                if (existingClients.Count >= 2)
                {
                    await DisplayAlert("Error", "Only 2 corporate clients are allowed.", "OK");
                    return;
                }

                
                await _database.AddCorporateClientAsync(new CorporateClient { Name = name, PhoneNumber = phone, Purchases = "" });
            }

            await Navigation.PushAsync(new ShoppingPage(name, isCorporate));
        }
    }
}
