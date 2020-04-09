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
using Bug.Logic.Service.SearchQuery.ChainQuery;
using Bug.Common.Enums;

namespace Bug.Test.Logic
{
  public class SearchQueryServiceTest
  {
    [Theory]
    [InlineData(FhirVersion.R4, ResourceType.Observation, "subject.organizationXX.name", "acmehealth")]
    [InlineData(FhirVersion.Stu3, ResourceType.Observation, "subject.organizationXX.name", "acmehealth")]
    [InlineData(FhirVersion.Stu3, ResourceType.Observation, "subjectXX.organization.name", "acmehealth")]    
    [InlineData(FhirVersion.Stu3, ResourceType.Observation, "subject.organization.nameXX", "acmehealth")]
    [InlineData(FhirVersion.Stu3, ResourceType.Observation, "subject..organization.nameXX", "acmehealth")]
    [InlineData(FhirVersion.Stu3, ResourceType.Observation, "subject.organization..nameXX", "acmehealth")]
    [InlineData(FhirVersion.Stu3, ResourceType.Observation, "subject:PatientXXX.organization..nameXX", "acmehealth")]
    [InlineData(FhirVersion.Stu3, ResourceType.Observation, "subject:Patient.organization:OrganizationXXX.name", "acmehealth")]

    public void ChainQueryNegative(FhirVersion fhirVersion, ResourceType resourceContext, string parameterName, string parameterValue)
    {
      //Prepare
      SearchQueryService SearchQueryService = SetupSearchQueryService();

      Dictionary<string, StringValues> QueryDictonary = new Dictionary<string, StringValues>
      {
        { parameterName, new StringValues(parameterValue) },
      };

      FhirSearchQuery FhirSearchQuery = new FhirSearchQuery();
      FhirSearchQuery.Parse(QueryDictonary);

      //Act
      ISerachQueryServiceOutcome Outcome = SearchQueryService.Process(fhirVersion, resourceContext, FhirSearchQuery).Result;

      //Assert
      Assert.NotNull(Outcome);
      Assert.True(Outcome.InvalidSearchQueryList.Count > 0);
      Assert.Null(Outcome.CountRequested);
      Assert.Equal(0, Outcome.SearchQueryList.Count);
      Assert.Equal(Common.Enums.ResourceType.Observation, Outcome.ResourceContext);

      foreach (var Error in Outcome.InvalidSearchQueryList)
      {
        Assert.Equal(parameterName, Error.Name);
        Assert.Equal(parameterValue, Error.Value);
        Assert.True(Error.ErrorMessage.Length > 5);
      }
    }

    [Theory]
    [InlineData(FhirVersion.R4, ResourceType.Observation, "subject.organization.name", "acmehealth")]
    [InlineData(FhirVersion.R4, ResourceType.Observation, "subject:Patient.organization.name", "acmehealth")]
    [InlineData(FhirVersion.R4, ResourceType.Observation, "subject:Patient.organization:Organization.name", "acmehealth")]
    [InlineData(FhirVersion.R4, ResourceType.Observation, "subject.organization:Organization.name", "acmehealth")]
    public void DoubleChainQueryPositive(FhirVersion fhirVersion, ResourceType resourceContext, string parameterName, string parameterValue)
    {
      //Prepare
      SearchQueryService SearchQueryService = SetupSearchQueryService();

      Dictionary<string, StringValues> QueryDictonary = new Dictionary<string, StringValues>
      {
        { parameterName, new StringValues(parameterValue) },
      };

      FhirSearchQuery FhirSearchQuery = new FhirSearchQuery();
      FhirSearchQuery.Parse(QueryDictonary);

      //Act
      ISerachQueryServiceOutcome Outcome = SearchQueryService.Process(fhirVersion, resourceContext, FhirSearchQuery).Result;

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

    [Theory]
    //_include:recurse=Observation:subject:Patient"
    [InlineData(FhirVersion.R4, ResourceType.Observation, IncludeType.Include, "recurse", ResourceType.Observation, "subject", ResourceType.Patient)]
    //_include:iterate=Observation:subject:Patient"
    [InlineData(FhirVersion.Stu3, ResourceType.Observation, IncludeType.Include, "iterate", ResourceType.Observation, "subject", ResourceType.Patient)]
    //_revinclude:recurse=Observation:subject:Patient"
    [InlineData(FhirVersion.R4, ResourceType.Observation, IncludeType.Revinclude, "recurse", ResourceType.Observation, "subject", ResourceType.Patient)]
    //_revinclude:iterate=Observation:subject:Patient"
    [InlineData(FhirVersion.Stu3, ResourceType.Observation, IncludeType.Revinclude, "iterate", ResourceType.Observation, "subject", ResourceType.Patient)]
    //_include:recurse=Observation:subject"
    [InlineData(FhirVersion.R4, ResourceType.Observation, IncludeType.Include, "recurse", ResourceType.Observation, "subject", null)]
    //_revinclude:iterate=Observation:subject"
    [InlineData(FhirVersion.Stu3, ResourceType.Observation, IncludeType.Revinclude, "iterate", ResourceType.Observation, "subject", null)]
    public void IncludeQueryPositive(FhirVersion fhirVersion, ResourceType resourceContext, IncludeType parameterInclude, string isRecurseIterate, ResourceType sourceResource, string searchParameterName, ResourceType? modifierResource)
    {
      //Prepare
      SearchQueryService SearchQueryService = SetupSearchQueryService();
      Dictionary<string, StringValues> QueryDictonary; 
      if (!string.IsNullOrWhiteSpace(isRecurseIterate))
      {
        if (modifierResource.HasValue)
        {
          QueryDictonary = new Dictionary<string, StringValues>
          {
            { $"{parameterInclude.GetCode()}:{isRecurseIterate}", new StringValues($"{sourceResource.GetCode()}:{searchParameterName}:{modifierResource.GetCode()}") },
          };
        }
        else
        {
          QueryDictonary = new Dictionary<string, StringValues>
          {
            { $"{parameterInclude.GetCode()}:{isRecurseIterate}", new StringValues($"{sourceResource.GetCode()}:{searchParameterName}") },
          };
        }
        
      }
      else
      {
        if (modifierResource.HasValue)
        {
          QueryDictonary = new Dictionary<string, StringValues>
          {
            { $"{parameterInclude.GetCode()}", new StringValues($"{sourceResource.GetCode()}:{searchParameterName}:{modifierResource.GetCode()}") },
          };
        }
        else
        {
          QueryDictonary = new Dictionary<string, StringValues>
          {
            { $"{parameterInclude.GetCode()}", new StringValues($"{sourceResource.GetCode()}:{searchParameterName}") },
          };
        }
         
      }

      FhirSearchQuery FhirSearchQuery = new FhirSearchQuery();
      FhirSearchQuery.Parse(QueryDictonary);

      //Act
      ISerachQueryServiceOutcome Outcome = SearchQueryService.Process(fhirVersion, resourceContext, FhirSearchQuery).Result;

      //Assert
      Assert.NotNull(Outcome);
      Assert.Equal(0, Outcome.InvalidSearchQueryList.Count);
      Assert.Null(Outcome.CountRequested);
      Assert.Equal(0, Outcome.SearchQueryList.Count);
      Assert.Equal(Common.Enums.ResourceType.Observation, Outcome.ResourceContext);
      Assert.Equal(1, Outcome.IncludeList.Count);

      var IncludeItem = Outcome.IncludeList[0];

      if (isRecurseIterate == "recurse")
      {
        Assert.True(IncludeItem.IsRecurse);
        Assert.True(IncludeItem.IsRecurseIterate);
      }
      else
      {
        Assert.False(IncludeItem.IsRecurse);
      }
      if (isRecurseIterate == "iterate")
      {
        Assert.True(IncludeItem.IsIterate);
        Assert.True(IncludeItem.IsRecurseIterate);
      }
      else
      {
        Assert.False(IncludeItem.IsIterate);
      }
      Assert.Equal(parameterInclude, IncludeItem.Type);
      Assert.Equal(sourceResource, IncludeItem.SourceResourceType);
      if (modifierResource.HasValue)
      {
        Assert.Equal(modifierResource, IncludeItem.SearchParameterTargetResourceType);
      }
      else
      {
        Assert.Null(IncludeItem.SearchParameterTargetResourceType);
      }
      
      Assert.Single(IncludeItem.SearchParameterList);
      var SearchParameter = IncludeItem.SearchParameterList[0];
      Assert.Equal(searchParameterName, SearchParameter.Name);

    }

    private SearchQueryService SetupSearchQueryService()
    {
      ISearchParameterCache ISearchParameterCache = SetupSearchParameterCache();
      SearchQueryFactory SearchQueryFactory = SetupSearchQueryFactory(ISearchParameterCache);      
      IResourceTypeSupport IResourceTypeSupport = new ResourceTypeSupport();
      var IKnownResourceMock = IKnownResource_MockFactory.Get();
      IChainQueryProcessingService IChainQueryProcessingService = new ChainQueryProcessingService(IResourceTypeSupport, IKnownResourceMock.Object, ISearchParameterCache, SearchQueryFactory);
      SearchQueryService SearchQueryService = new SearchQueryService(ISearchParameterCache, SearchQueryFactory, IResourceTypeSupport, IKnownResourceMock.Object, IChainQueryProcessingService);
      return SearchQueryService;
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
