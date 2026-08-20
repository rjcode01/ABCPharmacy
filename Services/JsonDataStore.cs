using System.Text.Json;
using ABCPharmacy.Models;

namespace ABCPharmacy.Services;

public class JsonDataStore
{
    private readonly string _dataDirectory;
    private readonly string _medicinesFile;
    private readonly string _salesFile;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };
    private readonly object _lock = new();

    public JsonDataStore(IWebHostEnvironment environment)
    {
        _dataDirectory = Path.Combine(environment.ContentRootPath, "Data");
        _medicinesFile = Path.Combine(_dataDirectory, "medicines.json");
        _salesFile = Path.Combine(_dataDirectory, "sales.json");
        Directory.CreateDirectory(_dataDirectory);
        EnsureFile(_medicinesFile, GetSeedMedicines());
        EnsureFile(_salesFile, new List<SaleRecord>());
    }

    public List<Medicine> GetMedicines()
    {
        lock (_lock) return Read<List<Medicine>>(_medicinesFile) ?? new();
    }

    public void SaveMedicines(List<Medicine> medicines)
    {
        lock (_lock) Write(_medicinesFile, medicines);
    }

    public List<SaleRecord> GetSales()
    {
        lock (_lock) return Read<List<SaleRecord>>(_salesFile) ?? new();
    }

    public void SaveSales(List<SaleRecord> sales)
    {
        lock (_lock) Write(_salesFile, sales);
    }

    private void EnsureFile<T>(string path, T value)
    {
        if (!File.Exists(path)) Write(path, value);
    }

    private T? Read<T>(string path)
    {
        if (!File.Exists(path)) return default;
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, _options);
    }

    private void Write<T>(string path, T value)
    {
        var temp = path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(value, _options));
        File.Move(temp, path, true);
    }

    private static List<Medicine> GetSeedMedicines() => new()
    {
        new() { Id = 1, FullName = "Paracetamol 500mg", Notes = "Pain and fever relief", ExpiryDate = DateTime.Today.AddDays(20), Quantity = 25, Price = 45.50m, Brand = "Cipla" },
        new() { Id = 2, FullName = "Amoxicillin 250mg", Notes = "Antibiotic", ExpiryDate = DateTime.Today.AddMonths(8), Quantity = 7, Price = 85.00m, Brand = "Abbott" },
        new() { Id = 3, FullName = "Cetirizine 10mg", Notes = "Allergy relief", ExpiryDate = DateTime.Today.AddYears(1), Quantity = 40, Price = 32.75m, Brand = "Dr. Reddy's" }
    };
}
