using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IResourceStoreRepository : IRepository<ResourceStore>
  {
    Task<ResourceStore?> GetCurrentAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId);
    Task<IList<ResourceStore>> GetResourceHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType);
    Task<ResourceStore?> GetVersionAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId, int versionId);
    Task<ResourceStore?> GetCurrentMetaAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId);
    Task<IList<ResourceStore>> GetInstanceHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId);
    Task<IList<ResourceStore>> GetBaseHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion);
    Task<bool> ReferentialIntegrityCheckAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceTypeId, string resourceId, int? versionId = null);
    public Task UpdateCurrentAsync(ResourceStore resourceStore);
  }
}
