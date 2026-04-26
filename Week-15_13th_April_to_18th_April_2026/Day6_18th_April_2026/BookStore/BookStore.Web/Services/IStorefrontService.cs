using BookStore.Web.Models.ViewModels;

namespace BookStore.Web.Services;

public interface IStorefrontService
{
    HomeDashboardViewModel GetHomeDashboard();
    BookListPageViewModel GetBooks(string? category, string? searchTerm, string? sortBy);
    BookCardViewModel? GetBookById(int id);
    CartPageViewModel GetCart();
    AdminDashboardViewModel GetAdminDashboard();
    IReadOnlyList<InventoryItemViewModel> GetInventory();
    IReadOnlyList<OrderSummaryViewModel> GetOrders();
    IReadOnlyList<OrderSummaryViewModel> GetOrdersForUser(string email);
    void PlaceOrder(string customerName, string customerEmail, decimal totalAmount);
}
