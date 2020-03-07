using Bug.Common.FhirTools.Bundle;
using Bug.Logic.DomainModel;
using System.Collections.Generic;

namespace Bug.Logic.Service.FhirResourceService
{
  public interface IHistoryBundleService
  {
    BundleModel GetHistoryBundleModel(IList<ResourceStore> ResourceStoreList);
  }
}