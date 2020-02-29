using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using Bug.Logic.DomainModel.Projection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IResourceStoreRepository : IRepository<ResourceStore>
  {
    Task<ResourceStore?> GetCurrentAsync(FhirMajorVersion fhirMajorVersion, string resourceName, string resourceId);
    Task<ResourceStore?> GetCurrentMetaAsync(FhirMajorVersion fhirMajorVersion, string ResourceName, string resourceId);
    void UpdateIsCurrent(ResourceStore resourceStore);
  }
}
