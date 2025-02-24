using Microsoft.AspNetCore.Mvc;
using Phonecase.Models;

namespace Phonecase.Repositories
{
    public interface IHistoryRepository
    {
        Task<IEnumerable<Purchase>> GetPurchaseHistoryAsync(int vendorId, DateTime startdate);
        Task<Purchase?> GetPurchaseByIdAsync(int id);
        Task<Purchase?> EditPurchaseAsync(int id, int quantity, decimal unitPrice);
        Task<IEnumerable<Payment>> GetPaymentHistoryAsync(int vendorId, DateTime startdate);
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<Payment?> EditPaymentAsync(int id, decimal amount);
    }
}
