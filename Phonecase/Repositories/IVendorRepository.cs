using Phonecase.Models;

namespace Phonecase.Repositories {
    public interface IVendorRepository {
        Task<IEnumerable<Vendor>> GetVendorAsync();
        Task<Vendor> CreateVendorAsync(Vendor vendor);
        Task<Vendor?> DeleteVendorAsync(int id);
        Task<Vendor?> GetVendorByIdAsync(int id);
        Task<Vendor?> UpdateVendorAsync(Vendor vendor); 
        Task<IEnumerable<Purchase>> GetPurchaseHistoryByIdAsync(int id); 
        Task<IEnumerable<Payment>> GetPaymentHistoryByIdAsync(int id);
    }
}