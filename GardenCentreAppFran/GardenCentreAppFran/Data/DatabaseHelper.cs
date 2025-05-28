using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace GardenCentreAppFran.Data
{
    public class CorporateClient
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Purchases { get; set; } = string.Empty; 
    }

    public class DatabaseHelper
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseHelper(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<CorporateClient>().Wait();
        }

        public Task<List<CorporateClient>> GetCorporateClientsAsync()
        {
            return _database.Table<CorporateClient>().ToListAsync();
        }

        public async Task<CorporateClient?> GetCorporateClientAsync(string name, string phoneNumber)
        {
            var clients = await _database.Table<CorporateClient>().Where(c => c.Name == name && c.PhoneNumber == phoneNumber).ToListAsync();
            return clients.FirstOrDefault();
        }

        public Task<int> AddCorporateClientAsync(CorporateClient client)
        {
            return _database.InsertAsync(client);
        }

        public Task<int> UpdateCorporateClientAsync(CorporateClient client)
        {
            return _database.UpdateAsync(client);
        }
    }
}
