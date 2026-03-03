using System.Text.RegularExpressions;

namespace InvoiceNumberUpdate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter current invoice number:");
            string invoice = Console.ReadLine();

            Console.WriteLine("Enter increment value:");
            int inc = int.Parse(Console.ReadLine());

            Match m = Regex.Match(invoice, @"\d+");

            int number = int.Parse(m.Value);
            int newNumber = number + inc;

            string newInvoice = Regex.Replace(invoice, @"\d+", newNumber.ToString());

            Console.WriteLine("Updated Invoice Number: " + newInvoice);
        }
    }
}
