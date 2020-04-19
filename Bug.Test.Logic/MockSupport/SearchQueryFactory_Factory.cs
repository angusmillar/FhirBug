using Bug.Common.DateTimeTools;
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
      Mock<IKnownResource> IKnownResourceMock = IKnownResource_MockFactory.Get();
      IFhirDateTimeFactory IFhirDateTimeFactory = IFhirDateTimeFactory_Factory.Get(TimeSpan.FromHours(10));
      return new SearchQueryFactory(IFhirUriFactory, IResourceTypeSupport, ISearchParameterCache, IKnownResourceMock.Object, IFhirDateTimeFactory);
    }
  }
}
