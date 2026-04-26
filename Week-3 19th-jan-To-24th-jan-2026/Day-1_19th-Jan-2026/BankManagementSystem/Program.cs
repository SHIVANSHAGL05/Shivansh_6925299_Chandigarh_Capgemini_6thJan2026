namespace BankManagementSystem
{
    internal class Program
    {
        class BankAccount
        {
            protected int accNo;
            protected double balance;

            public BankAccount(int accNo, double balance)
            {
                this.accNo = accNo;
                this.balance = balance;
            }

            public void Deposit(double amt) => balance += amt;
            public void Withdraw(double amt) { if (amt <= balance) balance -= amt; }
            public virtual void Display() => Console.WriteLine(accNo + " " + balance);
        }

        class SavingsAccount : BankAccount
        {
            public SavingsAccount(int a, double b) : base(a, b) { }
            public void CalculateInterest() => Console.WriteLine(balance * 0.05);
        }

        class CheckingAccount : BankAccount
        {
            public CheckingAccount(int a, double b) : base(a, b) { }
        }

        static void Main(string[] args)
        {
            SavingsAccount s = new SavingsAccount(101, 5000);
            s.Deposit(2000);
            s.CalculateInterest();
            s.Display();

        }
    }
}
