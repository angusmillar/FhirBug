using Bug.Logic.CacheService;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using Bug.Test.Logic.MockSupport;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Bug.Logic.Service.SearchQuery;
using Bug.Logic.Service.SearchQuery.Tools;
using Microsoft.Extensions.Primitives;
using Bug.Common.FhirTools;

namespace Bug.Test.Logic
{
  public class SearchQueryServiceTest
  {
    [Fact]
    public void One()
    {
      //Prepare
      ISearchParameterCache ISearchParameterCache = SetupSearchParameterCache();
      SearchQueryFactory SearchQueryFactory = SetupSearchQueryFactory(ISearchParameterCache);
      var IOperationOutcomeSupportMock = IOperationOutcomeSupport_MockFactory.Get();
      IResourceTypeSupport IResourceTypeSupport = new ResourceTypeSupport();
      var IKnownResourceMock = IKnownResource_MockFactory.Get();
      SearchQueryService SearchQueryService = new SearchQueryService(ISearchParameterCache, SearchQueryFactory, IOperationOutcomeSupportMock.Object, IResourceTypeSupport, IKnownResourceMock.Object);

      Dictionary<string, StringValues> QueryDictonary = new Dictionary<string, StringValues>
      {
        { "subject:Patient.organization.name", new StringValues("acmehealth") },        
      };

      FhirSearchQuery FhirSearchQuery = new FhirSearchQuery();
      FhirSearchQuery.Parse(QueryDictonary);

      //Act
      SerachQueryServiceOutcome Outcome = SearchQueryService.Process(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, FhirSearchQuery).Result;

      //Assert
      Assert.NotNull(Outcome);
      Assert.Equal(0, Outcome.InvalidSearchQueryList.Count);
      Assert.Null(Outcome.CountRequested);
      Assert.Equal(1, Outcome.SearchQueryList.Count);
      Assert.Equal(Common.Enums.ResourceType.Observation, Outcome.ResourceContext);

      var ChainSearchQuery = Outcome.SearchQueryList[0];
      Assert.NotNull(ChainSearchQuery);

      Assert.Equal("subject", ChainSearchQuery.Name);
      Assert.NotNull(ChainSearchQuery.ChainedSearchParameter);
      Assert.Equal("organization", ChainSearchQuery.ChainedSearchParameter!.Name);
      Assert.NotNull(ChainSearchQuery.ChainedSearchParameter!.ChainedSearchParameter);
      Assert.Equal("name", ChainSearchQuery.ChainedSearchParameter!.ChainedSearchParameter!.Name);
      if (ChainSearchQuery.ChainedSearchParameter!.ChainedSearchParameter is SearchQueryString SearchQueryString)
      {
        Assert.Single(SearchQueryString.ValueList);
        Assert.Equal("acmehealth", SearchQueryString.ValueList[0].Value);
      }

    }

    private SearchQueryFactory SetupSearchQueryFactory(ISearchParameterCache ISearchParameterCache)
    {
      return SearchQueryFactory_Factory.Get(ISearchParameterCache);
    }

    private ISearchParameterCache SetupSearchParameterCache()
    {
      var SearchParameterListStu3 = Support.SearchParameterData.GetSearchParameterList(Common.Enums.FhirVersion.Stu3);
      var SearchParameterListR4 = Support.SearchParameterData.GetSearchParameterList(Common.Enums.FhirVersion.R4);
      return ISearchParameterCache_MockFactory.Get(SearchParameterListStu3, SearchParameterListR4).Object;
    }
  }
}
