using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.CacheService;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Test.Logic.MockSupport
{
  public static class SearchQueryFactory_Factory
  {
    public static SearchQueryFactory Get(ISearchParameterCache ISearchParameterCache)
    {
      Bug.Common.Interfaces.IFhirUriFactory IFhirUriFactory = FhirUriFactory_Factory.Get(TestData.BaseUrlServer, new string[]
      {
        ResourceType.Observation.GetCode(),
        ResourceType.Patient.GetCode(),
        ResourceType.Device.GetCode(),
        ResourceType.Encounter.GetCode()
      });

      IResourceTypeSupport IResourceTypeSupport = new ResourceTypeSupport();
      var SearchParameterListStu3 = Support.SearchParameterData.GetSearchParameterList(FhirVersion.Stu3);
      var SearchParameterListR4 = Support.SearchParameterData.GetSearchParameterList(FhirVersion.R4);      
      Mock<IKnownResource> IKnownResourceMock = IKnownResource_MockFactory.Get();
      return new SearchQueryFactory(IFhirUriFactory, IResourceTypeSupport, ISearchParameterCache, IKnownResourceMock.Object);
    }
  }
}
