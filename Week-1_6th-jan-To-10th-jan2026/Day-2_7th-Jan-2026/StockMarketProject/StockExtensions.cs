namespace StockMarketProject
{
    static class StockExtensions
    {
        public static double TotalValue(this Stock stock)
        {
            return stock.GetStockPrice() * stock.GetStockQuantity();
        }
    }
}
