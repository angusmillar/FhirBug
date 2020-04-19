using Bug.Common.DatabaseTools;
using Bug.Common.Enums;
using Bug.Common.StringTools;
using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using Bug.Test.Logic.MockSupport;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bug.Test.Logic
{

  public class SearchQueryFactoryTest
  {
    private Mock<ISearchParameterCache>? ISearchParameterCacheMock;
    private ISearchQueryFactory? SearchQueryFactory;

    [Theory]
    //String
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Patient, Common.Enums.SearchParamType.String, "family", "Millar")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Patient, Common.Enums.SearchParamType.String, "family", "Millar")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Patient, Common.Enums.SearchParamType.String, "family", "Millar,Smith")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Patient, Common.Enums.SearchParamType.String, "family", "Millar,Smith")]
    
    //Reference
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", "Patient/11")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", TestData.BaseUrlRemote + "/Patient/11")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", TestData.BaseUrlRemote + "/Patient/11")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", TestData.BaseUrlServer + "/Patient/11")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", TestData.BaseUrlServer + "/Patient/11")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "encounter", "11")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "encounter", "22")]

    //Token
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Token, "code", "system|code")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Token, "code", "system|code")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Token, "code", "code")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Token, "code", "system|")]

    //Composite
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Composite, "code-value-concept", "SystemOne|CodeOne$SystemTwo|CodeTwo")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Composite, "code-value-concept", "SystemOne|CodeOne$SystemTwo|CodeTwo")]
    
    //Quantity
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Quantity, "value-quantity", "5.4|http://unitsofmeasure.org|mg")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Quantity, "value-quantity", "5.4|http://unitsofmeasure.org|mg")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Quantity, "value-quantity", "5.40e-3|http://unitsofmeasure.org|g")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Quantity, "value-quantity", "5.40e-3|http://unitsofmeasure.org|g")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Quantity, "value-quantity", "5.40e+10|http://unitsofmeasure.org|g")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Quantity, "value-quantity", "5.40e+10|http://unitsofmeasure.org|g")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Quantity, "value-quantity", "-5.40e+10|http://unitsofmeasure.org|g")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Quantity, "value-quantity", "-5.40e+10|http://unitsofmeasure.org|g")]
    
    //Date
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020-04")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020-04")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020-04-07")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020-04-07")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "gt2020-04-07")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "gt2020-04-07")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020-04-07T10:00")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020-04-07T10:00")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020-04-07T10:00:10")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020-04-07T10:00:10")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020-04-07T10:00:10+05:00")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Date, "date", "2020-04-07T10:00:10+05:00")]

    //Number
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.RiskAssessment, Common.Enums.SearchParamType.Number, "probability", "100")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.RiskAssessment, Common.Enums.SearchParamType.Number, "probability", "100.00")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.RiskAssessment, Common.Enums.SearchParamType.Number, "probability", "lt100")]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.RiskAssessment, Common.Enums.SearchParamType.Number, "probability", "gt100")]
    //Uri
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.CodeSystem, Common.Enums.SearchParamType.Uri, "url", "http://somerandom/url/to/somewhere")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.CodeSystem, Common.Enums.SearchParamType.Uri, "url", "http://somerandom/url/to/somewhere")]

    public void TestQueryTypesPositive(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType ResourceContext, Common.Enums.SearchParamType SearchParamType, string searchParameterName, string queryValue)
    {
      //Prepare
      Setup();
      List<SearchParameter> SearchParameterListForResource = ISearchParameterCacheMock!.Object.GetForIndexingAsync(fhirVersion, ResourceContext).Result;
      SearchParameter SearchParameter = SearchParameterListForResource.SingleOrDefault(x => x.Name == searchParameterName);

      var Parameter = new KeyValuePair<string, StringValues>(searchParameterName, new StringValues(queryValue));

      //Act
      IList<ISearchQueryBase> ISearchQueryBaseList = SearchQueryFactory!.Create(ResourceContext, SearchParameter, Parameter, false).Result;

      //Assert
      Assert.Equal(1, ISearchQueryBaseList.Count);
      ISearchQueryBase SearchQueryResult = ISearchQueryBaseList[0];
      Assert.Null(SearchQueryResult.ChainedSearchParameter);
      if (SearchParamType == Common.Enums.SearchParamType.Composite)
      {
        Assert.NotNull(SearchQueryResult.ComponentList);
        //We are only assuming all Composite have only two components here, which is not always true
        Assert.Equal(2, SearchQueryResult.ComponentList.Count);
      }
      else
      {
        Assert.Equal(0, SearchQueryResult.ComponentList.Count);
      }

      Assert.Equal(fhirVersion, SearchQueryResult.FhirVersionId);
      Assert.Equal(queryValue.Contains(','), SearchQueryResult.HasLogicalOrProperties);
      Assert.Equal(string.Empty, SearchQueryResult.InvalidMessage);
      Assert.True(SearchQueryResult.IsValid);
      Assert.Null(SearchQueryResult.Modifier);
      Assert.Equal(searchParameterName, SearchQueryResult.Name);
      Assert.Equal($"{searchParameterName}={queryValue}", SearchQueryResult.RawValue);
      Assert.Equal(ResourceContext, SearchQueryResult.ResourceContext);
      Assert.True(SearchQueryResult.ResourceTypeList.Count > 0);
      Assert.Contains(SearchQueryResult.ResourceTypeList.ToArray(), x => x.ResourceTypeId == ResourceContext);
      Assert.Equal(SearchParamType, SearchQueryResult.SearchParamTypeId);
      if (SearchParamType == Common.Enums.SearchParamType.Reference)
      {
        Assert.True(SearchQueryResult.TargetResourceTypeList.Count > 0);
      }
      else
      {
        Assert.True(SearchQueryResult.TargetResourceTypeList.Count == 0);
      }
      Assert.Equal(string.Empty, SearchQueryResult.TypeModifierResource);


      if (SearchQueryResult is SearchQueryComposite SearchQueryComposite)
      {
        Assert.Equal(2, SearchQueryResult.ComponentList.Count);
        Assert.True(SearchQueryComposite.ValueList.Count == 1);
        Assert.True(SearchQueryComposite.ValueList[0].SearchQueryBaseList.Count == 2);
        if (SearchQueryComposite.ValueList[0].SearchQueryBaseList[0] is SearchQueryToken SearchQueryToken)
        {
          Assert.True(SearchQueryToken.ValueList.Count == 1);
          Assert.Equal(queryValue.Split('$')[0].Split('|')[0], SearchQueryToken.ValueList[0].System);
          Assert.Equal(queryValue.Split('$')[0].Split('|')[1], SearchQueryToken.ValueList[0].Code);
        }
      }
      else if (SearchQueryResult is SearchQueryString SearchQueryString)
      {
        if (queryValue.Contains(','))
        {
          Assert.Equal(StringSupport.ToLowerTrimRemoveDiacriticsTruncate(queryValue.Split(',')[0], DatabaseMetaData.FieldLength.StringMaxLength), SearchQueryString.ValueList[0].Value);
          Assert.Equal(StringSupport.ToLowerTrimRemoveDiacriticsTruncate(queryValue.Split(',')[1], DatabaseMetaData.FieldLength.StringMaxLength), SearchQueryString.ValueList[1].Value);
        }
        else
        {
          Assert.Equal(StringSupport.ToLowerTrimRemoveDiacriticsTruncate(queryValue, DatabaseMetaData.FieldLength.StringMaxLength), SearchQueryString.ValueList[0].Value);
        }

      }
      else if (SearchQueryResult is SearchQueryReference SearchQueryReference)
      {
        Assert.NotNull(SearchQueryReference.ValueList[0].FhirUri);
        if (queryValue.StartsWith(TestData.BaseUrlServer))
        {
          Assert.Equal(Common.Enums.ResourceType.Patient.GetCode(), SearchQueryReference.ValueList[0].FhirUri!.ResourseName);
          Assert.Equal("11", SearchQueryReference.ValueList[0].FhirUri!.ResourceId);
          Assert.True(SearchQueryReference.ValueList[0].FhirUri!.IsRelativeToServer);
        }
        else if (queryValue.StartsWith(TestData.BaseUrlRemote))
        {
          Assert.Equal(Common.Enums.ResourceType.Patient.GetCode(), SearchQueryReference.ValueList[0].FhirUri!.ResourseName);
          Assert.Equal("11", SearchQueryReference.ValueList[0].FhirUri!.ResourceId);
          Assert.False(SearchQueryReference.ValueList[0].FhirUri!.IsRelativeToServer);
        }
        else if (queryValue.Contains('/'))
        {
          Assert.Equal(queryValue.Split('/')[0], SearchQueryReference.ValueList[0].FhirUri!.ResourseName);
          Assert.Equal(queryValue.Split('/')[1], SearchQueryReference.ValueList[0].FhirUri!.ResourceId);
        }
        else
        {
          Assert.Equal(Common.Enums.ResourceType.Encounter.GetCode(), SearchQueryReference.ValueList[0].FhirUri!.ResourseName);
          Assert.Equal(queryValue, SearchQueryReference.ValueList[0].FhirUri!.ResourceId);
        }
      }
      else if (SearchQueryResult is SearchQueryToken SearchQueryToken)
      {
        if (queryValue.StartsWith("code"))
        {
          Assert.Null(SearchQueryToken.ValueList[0].System);
          Assert.Equal(queryValue, SearchQueryToken.ValueList[0].Code);
        }
        else if (queryValue.EndsWith("|"))
        {
          Assert.Null(SearchQueryToken.ValueList[0].Code);
          Assert.Equal(queryValue.TrimEnd('|'), SearchQueryToken.ValueList[0].System);
        }
        else
        {
          Assert.Equal(queryValue.Split('|')[0], SearchQueryToken.ValueList[0].System);
          Assert.Equal(queryValue.Split('|')[1], SearchQueryToken.ValueList[0].Code);
          
        }

      }
      else if (SearchQueryResult is SearchQueryQuantity SearchQueryQuantity)
      {
        Assert.Equal(queryValue.Split('|')[1], SearchQueryQuantity.ValueList[0].System);
        Assert.Equal(queryValue.Split('|')[2], SearchQueryQuantity.ValueList[0].Code);
        if (queryValue.Split('|')[0].Contains("e+10") && queryValue.Split('|')[0].StartsWith('-'))
        {
          Assert.Equal(-54000000000m, SearchQueryQuantity.ValueList[0].Value);
          Assert.Equal(0, SearchQueryQuantity.ValueList[0].Scale);
          Assert.Equal(11, SearchQueryQuantity.ValueList[0].Precision);
        }
        else if (queryValue.Split('|')[0].Contains("e-3"))
        {
          Assert.Equal(0.00540m, SearchQueryQuantity.ValueList[0].Value);
          Assert.Equal(5, SearchQueryQuantity.ValueList[0].Scale);
          Assert.Equal(5, SearchQueryQuantity.ValueList[0].Precision);
        }
        else if (queryValue.Split('|')[0].Contains("e+10"))
        {
          Assert.Equal(54000000000m, SearchQueryQuantity.ValueList[0].Value);
          Assert.Equal(0, SearchQueryQuantity.ValueList[0].Scale);
          Assert.Equal(11, SearchQueryQuantity.ValueList[0].Precision);
        }
        else
        {
          Assert.Equal(Decimal.Parse(queryValue.Split('|')[0]), SearchQueryQuantity.ValueList[0].Value);
          Assert.Equal(1, SearchQueryQuantity.ValueList[0].Scale);
          Assert.Equal(2, SearchQueryQuantity.ValueList[0].Precision);
        }

      }
      else if (SearchQueryResult is SearchQueryNumber SearchQueryNumber)
      {
        if (queryValue.StartsWith("gt"))
        {
          Assert.Equal(100, SearchQueryNumber.ValueList[0].Value);
          Assert.Equal(0, SearchQueryNumber.ValueList[0].Scale);
          Assert.Equal(3, SearchQueryNumber.ValueList[0].Precision);
          Assert.Equal(SearchComparator.Gt, SearchQueryNumber.ValueList[0].Prefix);
        }
        else if (queryValue.StartsWith("lt"))
        {
          Assert.Equal(100, SearchQueryNumber.ValueList[0].Value);
          Assert.Equal(0, SearchQueryNumber.ValueList[0].Scale);
          Assert.Equal(3, SearchQueryNumber.ValueList[0].Precision);
          Assert.Equal(SearchComparator.Lt, SearchQueryNumber.ValueList[0].Prefix);
        }
        else if (queryValue == "100")
        {
          Assert.Equal(100, SearchQueryNumber.ValueList[0].Value);
          Assert.Equal(0, SearchQueryNumber.ValueList[0].Scale);
          Assert.Equal(3, SearchQueryNumber.ValueList[0].Precision);
          Assert.Null(SearchQueryNumber.ValueList[0].Prefix);
        }
        else if (queryValue == "100.00")
        {
          Assert.Equal(100.00m, SearchQueryNumber.ValueList[0].Value);
          Assert.Equal(2, SearchQueryNumber.ValueList[0].Scale);
          Assert.Equal(5, SearchQueryNumber.ValueList[0].Precision);
          Assert.Null(SearchQueryNumber.ValueList[0].Prefix);
        }

      }
      else if (SearchQueryResult is SearchQueryDateTime SearchQueryDateTime)
      {
        if (queryValue.StartsWith("gt"))
        {
          var testDate = new DateTimeOffset(new DateTime(2020, 04, 07), TimeSpan.FromHours(10));
          Assert.Equal(testDate.ToUniversalTime().DateTime, SearchQueryDateTime.ValueList[0].Value);
          Assert.Equal(DateTimePrecision.Day, SearchQueryDateTime.ValueList[0].Precision);
          Assert.Equal(SearchComparator.Gt, SearchQueryDateTime.ValueList[0].Prefix);
        }
        else if (queryValue.Length == 16)
        {
          var testDate = new DateTimeOffset(new DateTime(2020, 04, 07, 10, 00, 00), TimeSpan.FromHours(10));
          Assert.Equal(testDate.ToUniversalTime().DateTime, SearchQueryDateTime.ValueList[0].Value);
          Assert.Equal(DateTimePrecision.HourMin, SearchQueryDateTime.ValueList[0].Precision);
        }
        else if (queryValue.Length == 4)
        {
          var testDate = new DateTimeOffset(new DateTime(2020, 01, 01), TimeSpan.FromHours(10));
          Assert.Equal(testDate.ToUniversalTime().DateTime, SearchQueryDateTime.ValueList[0].Value);
          Assert.Equal(DateTimePrecision.Year, SearchQueryDateTime.ValueList[0].Precision);
        }
        else if (queryValue.Length == 7)
        {
          var testDate = new DateTimeOffset(new DateTime(2020, 04, 01), TimeSpan.FromHours(10));
          Assert.Equal(testDate.ToUniversalTime().DateTime, SearchQueryDateTime.ValueList[0].Value);
          Assert.Equal(DateTimePrecision.Month, SearchQueryDateTime.ValueList[0].Precision);
        }
        else if (queryValue.Length == 10)
        {
          var testDate = new DateTimeOffset(new DateTime(2020, 04, 07), TimeSpan.FromHours(10));
          Assert.Equal(testDate.ToUniversalTime().DateTime, SearchQueryDateTime.ValueList[0].Value);
          Assert.Equal(DateTimePrecision.Day, SearchQueryDateTime.ValueList[0].Precision);
        }
        else if (queryValue.Length == 19)
        {
          var testDate = new DateTimeOffset(new DateTime(2020, 04, 07, 10, 00, 10), TimeSpan.FromHours(10));
          Assert.Equal(testDate.ToUniversalTime().DateTime, SearchQueryDateTime.ValueList[0].Value);
          Assert.Equal(DateTimePrecision.Sec, SearchQueryDateTime.ValueList[0].Precision);
        }
        else if (queryValue.Length == 25)
        {
          var testDate = new DateTimeOffset(new DateTime(2020, 04, 07, 10, 00, 10), TimeSpan.FromHours(5));
          Assert.Equal(testDate.ToUniversalTime().DateTime, SearchQueryDateTime.ValueList[0].Value);
          Assert.Equal(DateTimePrecision.Sec, SearchQueryDateTime.ValueList[0].Precision);
        }
      }
      else if (SearchQueryResult is SearchQueryUri SearchQueryUri)
      {
        Assert.Equal(new Uri(queryValue), SearchQueryUri.ValueList[0].Value);
      }
      else
      {
        Assert.Null(SearchQueryResult.ComponentList);
      }
    }

    [Fact]
    public void TestCompositeType()
    {
      //Prepare
      Setup();
      List<SearchParameter> SearchParameterListForResource = ISearchParameterCacheMock!.Object.GetForIndexingAsync(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation).Result;
      SearchParameter SearchParameter = SearchParameterListForResource.SingleOrDefault(x => x.Name == "code-value-concept");

      var Parameter = new KeyValuePair<string, StringValues>("code-value-concept", new StringValues("SystemOne|CodeOne$SystemTwo|CodeTwo"));

      //Act
      IList<ISearchQueryBase> ISearchQueryBaseList = SearchQueryFactory!.Create(Common.Enums.ResourceType.Observation, SearchParameter, Parameter, false).Result;

      //Assert
      Assert.Equal(1, ISearchQueryBaseList.Count);
      ISearchQueryBase SearchQueryResult = ISearchQueryBaseList[0];
      Assert.Null(SearchQueryResult.ChainedSearchParameter);
      Assert.NotNull(SearchQueryResult.ComponentList);
      Assert.Equal(2, SearchQueryResult.ComponentList.Count);
      Assert.Equal(Bug.Common.Enums.FhirVersion.R4, SearchQueryResult.FhirVersionId);
      Assert.False(SearchQueryResult.HasLogicalOrProperties);
      Assert.Equal(string.Empty, SearchQueryResult.InvalidMessage);
      Assert.True(SearchQueryResult.IsValid);
      Assert.Null(SearchQueryResult.Modifier);
      Assert.Equal("code-value-concept", SearchQueryResult.Name);
      Assert.Equal("code-value-concept=SystemOne|CodeOne$SystemTwo|CodeTwo", SearchQueryResult.RawValue);
      Assert.Equal(Common.Enums.ResourceType.Observation, SearchQueryResult.ResourceContext);
      Assert.Equal(1, SearchQueryResult.ResourceTypeList.Count);
      Assert.Equal(Common.Enums.ResourceType.Observation, SearchQueryResult.ResourceTypeList.ToArray()[0].ResourceTypeId);
      Assert.Equal(Common.Enums.SearchParamType.Composite, SearchQueryResult.SearchParamTypeId);
      Assert.Equal(0, SearchQueryResult.TargetResourceTypeList.Count);
      Assert.Equal(string.Empty, SearchQueryResult.TypeModifierResource);
      Assert.Equal("http://hl7.org/fhir/SearchParameter/Observation-code-value-concept", SearchQueryResult.Url);
    }

    private void Setup()
    {
      var SearchParameterListStu3 = Support.SearchParameterData.GetSearchParameterList(Common.Enums.FhirVersion.Stu3);
      var SearchParameterListR4 = Support.SearchParameterData.GetSearchParameterList(Common.Enums.FhirVersion.R4);
      ISearchParameterCacheMock = ISearchParameterCache_MockFactory.Get(SearchParameterListStu3, SearchParameterListR4);
      SearchQueryFactory = SearchQueryFactory_Factory.Get(ISearchParameterCacheMock.Object);     
    }

  }
}
