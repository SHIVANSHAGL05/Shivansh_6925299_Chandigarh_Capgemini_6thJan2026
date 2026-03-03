using System.Text.RegularExpressions;

namespace LocationCodeUpdateInvoice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter current invoice number:");
            string invoice = Console.ReadLine();

            Console.WriteLine("Enter new location code:");
            string newLoc = Console.ReadLine();

            string updatedInvoice = Regex.Replace(
                invoice,
                @"CAP-([A-Z]+)-",
                "CAP-" + newLoc + "-"
            );

            Console.WriteLine("Updated Invoice Number: " + updatedInvoice);
        }
    }
}
