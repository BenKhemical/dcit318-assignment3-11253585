// ========== QUESTION 1 ==========
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Bank Transfer: Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Mobile Money: Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Crypto Wallet: Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
            Console.WriteLine("Insufficient funds");
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction Applied. New Balance: {Balance:C}");
        }
    }
}

public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        var account = new SavingsAccount("ACC123", 1000);

        var t1 = new Transaction(1, DateTime.Now, 100, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 200, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 300, "Entertainment");

        new MobileMoneyProcessor().Process(t1);
        account.ApplyTransaction(t1);

        new BankTransferProcessor().Process(t2);
        account.ApplyTransaction(t2);

        new CryptoWalletProcessor().Process(t3);
        account.ApplyTransaction(t3);

        _transactions.AddRange(new[] { t1, t2, t3 });
    }
}


// ========== QUESTION 2 ==========

public class Repository<T>
{
    private List<T> items = new();

    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => items;
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null) return items.Remove(item);
        return false;
    }
}

public class Patient
{
    public int Id;
    public string Name;
    public int Age;
    public string Gender;

    public Patient(int id, string name, int age, string gender)
    {
        Id = id; Name = name; Age = age; Gender = gender;
    }
}

public class Prescription
{
    public int Id;
    public int PatientId;
    public string MedicationName;
    public DateTime DateIssued;

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id; PatientId = patientId; MedicationName = medicationName; DateIssued = dateIssued;
    }
}

public class HealthSystemApp
{
  private Repository<Patient> _patientRepo = new();
  private Repository<Prescription> _prescriptionRepo = new();
  private Dictionary<int, List<Prescription>> _prescriptionMap = new();

  public void SeedData()
  {
    _patientRepo.Add(new Patient(1, "John Doe", 30, "Male"));
    _patientRepo.Add(new Patient(2, "Jane Smith", 25, "Female"));

    _prescriptionRepo.Add(new Prescription(1, 1, "Paracetamol", DateTime.Today));
    _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Today));
    _prescriptionRepo.Add(new Prescription(3, 2, "Amoxicillin", DateTime.Today));
  }

  public void BuildPrescriptionMap()
  {
    foreach (var p in _prescriptionRepo.GetAll())
    {
      if (!_prescriptionMap.ContainsKey(p.PatientId))
        _prescriptionMap[p.PatientId] = new List<Prescription>();
      _prescriptionMap[p.PatientId].Add(p);
    }
  }

  public void PrintAllPatients()
  {
    foreach (var p in _patientRepo.GetAll())
      Console.WriteLine($"{p.Id}: {p.Name}, {p.Age}, {p.Gender}");
  }

  public void PrintPrescriptionsForPatient(int id)
  {
    if (_prescriptionMap.ContainsKey(id))
    {
      foreach (var pres in _prescriptionMap[id])
        Console.WriteLine($"{pres.MedicationName} issued on {pres.DateIssued}");
    }
    else Console.WriteLine("No prescriptions found.");
  }
}

// ========== QUESTION 3 ==========

public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
    }
}

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException("Item with the same ID already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException("Item not found.");
        return _items[id];
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException("Item not found for removal.");
    }

    public List<T> GetAllItems() => new(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");
        GetItemById(id).Quantity = newQuantity;
    }
}

public class WareHouseManager
{
    public InventoryRepository<ElectronicItem> _electronics = new();
    public InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Phone", 15, "Samsung", 12));

        _groceries.AddItem(new GroceryItem(1, "Milk", 20, DateTime.Today.AddDays(7)));
        _groceries.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Today.AddDays(2)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
            Console.WriteLine($"{item.Id} - {item.Name} - Qty: {item.Quantity}");
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            item.Quantity += quantity;
        }
        catch (Exception e) { Console.WriteLine(e.Message); }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
        }
        catch (Exception e) { Console.WriteLine(e.Message); }
    }
}



// ========== QUESTION 4 ==========

public class Student
{
    public int Id;
    public string FullName;
    public int Score;

    public string GetGrade() => Score switch
    {
        >= 80 => "A",
        >= 70 => "B",
        >= 60 => "C",
        >= 50 => "D",
        _ => "F"
    };
}

public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string path)
    {
        var students = new List<Student>();
        foreach (var line in File.ReadAllLines(path))
        {
            var parts = line.Split(',');
            if (parts.Length < 3)
                throw new MissingFieldException("Missing data in line: " + line);

            if (!int.TryParse(parts[0], out int id) || !int.TryParse(parts[2], out int score))
                throw new InvalidScoreFormatException("Invalid ID or score format: " + line);

            students.Add(new Student { Id = id, FullName = parts[1], Score = score });
        }
        return students;
    }

    public void WriteReportToFile(List<Student> students, string path)
    {
        using StreamWriter sw = new(path);
        foreach (var s in students)
            sw.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
    }
}

// ========== QUESTION 5 ==========

public interface IInventoryEntity
{
    int Id { get; }
}

public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item) => _log.Add(item);
    public List<T> GetAll() => _log;

    public void SaveToFile()
    {
        try
        {
            var lines = _log.Select(item => System.Text.Json.JsonSerializer.Serialize(item));
            File.WriteAllLines(_filePath, lines);
        }
        catch (Exception ex) { Console.WriteLine("Error saving: " + ex.Message); }
    }

    public void LoadFromFile()
    {
        try
        {
            _log = File.ReadAllLines(_filePath)
                .Select(line => System.Text.Json.JsonSerializer.Deserialize<T>(line)!)
                .ToList();
        }
        catch (Exception ex) { Console.WriteLine("Error loading: " + ex.Message); }
    }
}

public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger = new("inventory.txt");

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Table", 10, DateTime.Today));
        _logger.Add(new InventoryItem(2, "Chair", 20, DateTime.Today));
        _logger.Add(new InventoryItem(3, "Fan", 5, DateTime.Today));
    }

    public void SaveData() => _logger.SaveToFile();
    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
            Console.WriteLine($"{item.Id}: {item.Name} - Qty: {item.Quantity} - Date: {item.DateAdded:yyyy-MM-dd}");
    }
}


public class Program
{
  public static void Main(string[] args)
  {
    // Run Question 1
    new FinanceApp().Run();

    // Run Question 2
    Console.WriteLine("\n=== Question 2: Healthcare System ===");
    var app2 = new HealthSystemApp();
    app2.SeedData();
    app2.BuildPrescriptionMap();
    app2.PrintAllPatients();
    app2.PrintPrescriptionsForPatient(1);

    // ========== QUESTION 3 ==========
    Console.WriteLine("\n=== Question 3: Warehouse Inventory ===");
    var warehouse = new WareHouseManager();
    warehouse.SeedData();
    Console.WriteLine("\nGroceries:");
    warehouse.PrintAllItems(warehouse._groceries);
    Console.WriteLine("\nElectronics:");
    warehouse.PrintAllItems(warehouse._electronics);
    warehouse.RemoveItemById(warehouse._groceries, 99);
    warehouse.IncreaseStock(warehouse._electronics, 1, -10);

    // ========== QUESTION 4 ==========
    Console.WriteLine("\n=== Question 4: Grading System ===");
    var processor = new StudentResultProcessor();
    try
    {
      var students = processor.ReadStudentsFromFile("input.txt");  
      processor.WriteReportToFile(students, "report.txt");
      Console.WriteLine("Student report written to report.txt");
    }
    catch (FileNotFoundException) { Console.WriteLine("input.txt not found."); }
    catch (InvalidScoreFormatException e) { Console.WriteLine(e.Message); }
    catch (MissingFieldException e) { Console.WriteLine(e.Message); }
    catch (Exception e) { Console.WriteLine("Unexpected error: " + e.Message); }

     // ========== QUESTION 5 ==========
        Console.WriteLine("\n=== Question 5: Inventory Logger ===");
        var inventoryApp = new InventoryApp();
        inventoryApp.SeedSampleData();
        inventoryApp.SaveData();
        inventoryApp.LoadData();
        inventoryApp.PrintAllItems();
      
  }
}
