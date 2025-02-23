using Phonecase.Models;

namespace Phonecase.Repositories
{
    public interface IManagementRepository
    {
        Task<IEnumerable<CaseManufacturer>> GetAllCompanyAsync();
        Task<CaseManufacturer> CreateCompanyAsync(CaseManufacturer caseManufacturer);
        Task<CaseManufacturer?> DeleteCompanyAsync(int id);
        Task<PhoneModel> CreateModelAsync(PhoneModel model);
        Task<IEnumerable<PhoneModel>> GetAllModelAsync();

    }
}
