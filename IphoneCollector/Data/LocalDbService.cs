
using SQLite;

namespace IphoneCollector.Data
{
    public class LocalDbService
    {
        private const string DB_NAME = "IphoneCollector.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDbService()
        {
            try
            {
                string dbPath = GetDatabasePath(DB_NAME);

                _connection = new SQLiteAsyncConnection(dbPath);

                _ = InitializeDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalDbService Constructor Error]: {ex.Message}");
            }
        }

        private static string GetDatabasePath(string dbName)
        {
            // LocalAppData gives you:
            // - Windows: C:\Users\<user>\AppData\Local\IphoneCollector
            // - macOS: /Users/<user>/Library/Application Support\IphoneCollector

            string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // Put your app folder under LocalAppData, e.g., IphoneCollector
            string appDbFolder = Path.Combine(baseFolder, "IphoneCollector");

            if (!Directory.Exists(appDbFolder))
                Directory.CreateDirectory(appDbFolder);

            return Path.Combine(appDbFolder, dbName);
        }


        private async Task InitializeDatabase()
        {
            try
            {
                await _connection.CreateTableAsync<Case>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }
        public async Task CreateCase(Case newCase)
        {
            try
            {
                await _connection.InsertAsync(newCase);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting case: {ex.Message}");
            }
        }

        public async Task<List<Case>> GetCases()
        {
            try
            {
                return await _connection.Table<Case>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving cases: {ex.Message}");
                return new List<Case>();
            }
        }
        public async Task<Case> GetById(int id)
        {
            try
            {
                return await _connection.Table<Case>().FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving case by ID: {ex.Message}");
                return null;
            }
        }

        public async Task Delete(Case _case)
        {
            try
            {
                await _connection.DeleteAsync(_case);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting case: {ex.Message}");
            }
        }
    }
}
