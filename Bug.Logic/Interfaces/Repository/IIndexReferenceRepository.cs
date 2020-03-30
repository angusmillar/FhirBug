using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using Bug.Logic.DomainModel.Projection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IIndexReferenceRepository : IRepository<IndexReference>
  {
    Task<bool> AnyAsync(Common.Enums.FhirVersion fhirVersion, int serviceBaseUrlId, Common.Enums.ResourceType resourceTypeId, string resourceId, string? versionId = null);
    Task<List<ReferentialIntegrityQuery>> GetResourcesReferenced(Common.Enums.FhirVersion fhirVersion, int serviceBaseUrlId, Common.Enums.ResourceType resourceTypeId, string resourceId, string? versionId = null);
  }
}