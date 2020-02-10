using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IResourceStoreRepository : IRepository<ResourceStore>
  {
    ResourceStore GetByFhirId(string fhirId);
  }
}
