
// ========== QUESTION 1 ==========
using System;
using System.Collections.Generic;

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

public class Program
{
    public static void Main(string[] args)
    {
        new FinanceApp().Run();  // Example: Run Question 1
    }
}