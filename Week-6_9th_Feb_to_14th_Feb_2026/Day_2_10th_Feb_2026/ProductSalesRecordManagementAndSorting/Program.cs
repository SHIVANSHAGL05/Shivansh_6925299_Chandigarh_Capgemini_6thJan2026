using System.Collections.Generic;

namespace ProductSalesRecordManagementAndSorting
{
    struct Product
    {
        public string product_ID;
        public int total_sales_amount;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> store = new Dictionary<string, int>();

            Console.WriteLine("Enter product records (type END to stop):");

            while (true)
            {
                string input = Console.ReadLine();

                if (input.ToUpper() == "END")
                    break;

                string[] parts = input.Split('-');

                string id = parts[0];
                int amount = int.Parse(parts[1]);

                if (store.ContainsKey(id))
                {
                    if (amount > store[id])
                        store[id] = amount;
                }
                else
                {
                    store[id] = amount;
                }
            }

            List<Product> products = new List<Product>();

            foreach (var item in store)
            {
                Product p;
                p.product_ID = item.Key;
                p.total_sales_amount = item.Value;
                products.Add(p);
            }

            products.Sort((a, b) => b.total_sales_amount.CompareTo(a.total_sales_amount));

            Console.WriteLine("\nOutput:");

            foreach (var p in products)
            {
                Console.WriteLine(p.product_ID + "-" + p.total_sales_amount);
            }
            Console.ReadLine();
        }
    }
}

/*
P1001 - 200
P1002 - 150
P1003 - 300
P1001 - 100
P1002 - 200
P1003 - 50
P1001 - 50
P1002 - 250
END


OUTPUT
P1003-300
P1002-250
P1001-200

*/