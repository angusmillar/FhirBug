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
    Task<ResourceStore> GetCurrentAsync(string fhirId);
    Task<ResourceStore> GetCurrentNoBlobAsync(string fhirId);
  }
}
