using Phonecase.Models;

namespace Phonecase.Repositories
{
    public interface ICaseManufacturerRepository
    {
        Task<IEnumerable<CaseManufacturer>> GetAllAsync();
        Task<CaseManufacturer> CreateAsync(CaseManufacturer entity);
        Task<CaseManufacturer?> DeleteAsync(int id);
    }
}
