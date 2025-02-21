using Phonecase.Models;

namespace Phonecase.Repositories
{
    public interface IManagementRepository
    {
        Task<IEnumerable<CaseManufacturer>> GetAllCompanyAsync();
        Task<CaseManufacturer> CreateCompanyAsync(CaseManufacturer entity);
        Task<CaseManufacturer?> DeleteCompanyAsync(int id);


        Task<IEnumerable<PhoneModel>> GetAllModelAsync();
        Task<PhoneModel> CreateModelAsync(PhoneModel model);

    }
}
