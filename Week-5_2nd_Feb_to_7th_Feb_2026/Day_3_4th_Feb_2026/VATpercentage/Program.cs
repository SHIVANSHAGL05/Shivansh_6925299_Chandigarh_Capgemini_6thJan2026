namespace VATpercentage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter product code (M / V / C / D):");
            char product = char.ToUpper(Convert.ToChar(Console.ReadLine()));

            double vat = 0;

            switch (product)
            {
                case 'M':
                    vat = 5;
                    break;
                case 'V':
                    vat = 12;
                    break;
                case 'C':
                    vat = 6.25;
                    break;
                case 'D':
                    vat = 6;
                    break;
                default:
                    Console.WriteLine("Invalid product code");
                    return;
            }

            Console.WriteLine("VAT = " + vat + "%");
        }
    }
}
