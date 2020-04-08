using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.Fhir;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Test.Logic.MockSupport
{
  public static class ISearchParameterCache_MockFactory
  {
    public static Mock<ISearchParameterCache> Get(List<SearchParameter> Stu3SearchParameterList, List<SearchParameter> R4SearchParameterList)
    {
      Mock<ISearchParameterCache> ISearchParameterCacheMock = new Mock<ISearchParameterCache>();
     
      ISearchParameterCacheMock.Setup(x =>
        x.GetForIndexingAsync(Common.Enums.FhirVersion.Stu3, It.IsAny<Common.Enums.ResourceType>()))
          .Returns((Common.Enums.FhirVersion FhirVersion, Common.Enums.ResourceType ResourceType) => Task.FromResult(Stu3SearchParameterList.Where(c => c.ResourceTypeList.Any(f => f.ResourceTypeId == ResourceType)).ToList()));

      ISearchParameterCacheMock.Setup(x =>
        x.GetForIndexingAsync(Common.Enums.FhirVersion.R4, It.IsAny<Common.Enums.ResourceType>()))
          .Returns((Common.Enums.FhirVersion FhirVersion, Common.Enums.ResourceType ResourceType) => Task.FromResult(R4SearchParameterList.Where(c => c.ResourceTypeList.Any(f => f.ResourceTypeId == ResourceType)).ToList()));

      return ISearchParameterCacheMock;
    }
  }
}
