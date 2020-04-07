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

namespace Bug.Test.Logic
{
  public class SearchQueryServiceTest
  {
    [Theory]
    [InlineData("11")]
    public void One(string versionId)
    {
      //Prepare
      ISearchParameterCache ISearchParameterCache = SetupSearchParameterCache();
      SearchQueryFactory SearchQueryFactory = SetupSearchQueryFactory(ISearchParameterCache);
      var IOperationOutcomeSupportMock = IOperationOutcomeSupport_MockFactory.Get();
      SearchQueryService SearchQueryService = new SearchQueryService(ISearchParameterCache, SearchQueryFactory, IOperationOutcomeSupportMock.Object);

      Dictionary<string, StringValues> QueryDictonary = new Dictionary<string, StringValues>();
      QueryDictonary.Add("subject:Patient.family", new StringValues("millar"));
      QueryDictonary.Add("code", new StringValues("system|code"));

      FhirSearchQuery FhirSearchQuery = new FhirSearchQuery();
      FhirSearchQuery.Parse(QueryDictonary);

      //Act
      SerachQueryServiceOutcome Outcome = SearchQueryService.Process(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, FhirSearchQuery).Result;

      //Assert
      Assert.NotNull(Outcome);
      Assert.Equal(0, Outcome.InvalidSearchQueryList.Count);
      Assert.Null(Outcome.CountRequested);
      Assert.Equal(2, Outcome.SearchQueryList.Count);



      Assert.Equal(Common.Enums.ResourceType.Observation, Outcome.ResourceContext);
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
