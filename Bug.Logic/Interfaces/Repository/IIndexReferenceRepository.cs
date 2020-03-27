using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IIndexReferenceRepository : IRepository<IndexReference>
  {
    Task<bool> AnyAsync(Common.Enums.FhirVersion fhirVersion, int serviceBaseUrlId, Common.Enums.ResourceType resourceTypeId, string resourceId, string? versionId = null);
  }
}