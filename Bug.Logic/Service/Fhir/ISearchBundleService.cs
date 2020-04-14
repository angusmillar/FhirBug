using Bug.Common.FhirTools.Bundle;
using Bug.Logic.DomainModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Logic.Service.Fhir
{
  public interface ISearchBundleService
  {
    Task<BundleModel> GetSearchBundleModel(IList<ResourceStore> ResourceStoreList);
  }
}