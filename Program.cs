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
    }
}
