using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service.Fhir;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bug.Test.Logic.MockSupport
{
  public static class ISearchParameterRepository_MockFactory
  {
    public static Mock<ISearchParameterRepository> Get()
    {
      Mock<ISearchParameterRepository> ISearchParameterRepositoryMock = new Mock<ISearchParameterRepository>();
      ISearchParameterRepositoryMock.Setup(x => 
      x.GetByCanonicalUrlAsync(Common.Enums.FhirVersion.Stu3, It.IsAny<Bug.Common.Enums.ResourceType>(), It.IsAny<string>()))
        .Returns((Common.Enums.FhirVersion FhirVersion, Common.Enums.ResourceType ResourceType, string CanonicalUrl) => Task.FromResult(
          new SearchParameter() 
          { 
            Url = CanonicalUrl, 
            FhirVersionId = FhirVersion, 
            ResourceTypeList = new List<SearchParameterResourceType>() { new SearchParameterResourceType() { ResourceTypeId = ResourceType } } 
          }));

      ISearchParameterRepositoryMock.Setup(x =>
      x.GetByCanonicalUrlAsync(Common.Enums.FhirVersion.R4, It.IsAny<Bug.Common.Enums.ResourceType>(), It.IsAny<string>()))
        .Returns((Common.Enums.FhirVersion FhirVersion, Common.Enums.ResourceType ResourceType, string CanonicalUrl) => Task.FromResult(
          new SearchParameter()
          {
            Url = CanonicalUrl,
            FhirVersionId = FhirVersion,
            ResourceTypeList = new List<SearchParameterResourceType>() { new SearchParameterResourceType() { ResourceTypeId = ResourceType } }
          }));
      return ISearchParameterRepositoryMock;
    }
  }
}
