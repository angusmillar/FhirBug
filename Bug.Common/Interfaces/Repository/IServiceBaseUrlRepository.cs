using Bug.Common.Interfaces.DomainModel;
using System.Threading.Tasks;

namespace Bug.Common.Interfaces.Repository
{
  public interface IServiceBaseUrlRepository
  {
    Task<IServiceBaseUrl?> GetBy(Bug.Common.Enums.FhirVersion fhirVersion, string url);
    Task<IServiceBaseUrl?> GetPrimary(Bug.Common.Enums.FhirVersion fhirVersion);
    Task<IServiceBaseUrl> AddAsync(Bug.Common.Enums.FhirVersion fhirVersion, string url, bool IsPrimary);
    Task SaveChangesAsync();
  }
}