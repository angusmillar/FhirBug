using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Moq;
using System;
using Xunit;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using Bug.Logic.UriSupport;
using Bug.Common.ApplicationConfig;
using Bug.R4Fhir.ResourceSupport;
using Bug.Stu3Fhir.ResourceSupport;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Test.Logic.MockSupport;
using Bug.Common.FhirTools;
using Bug.Logic.CacheService;
using Bug.Logic.Service.Fhir;
using System.Threading.Tasks;
using Bug.Logic.DomainModel;
using Microsoft.Extensions.Primitives;
using Bug.Common.DatabaseTools;
using Bug.Common.StringTools;
using Bug.Common.Enums;

namespace Bug.Test.Logic
{
  public class SearchQueryFactoryTest
  {
    private Mock<ISearchParameterCache>? ISearchParameterCacheMock;
    private ISearchQueryFactory? SearchQueryFactory;

    [Theory]
    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Patient, Common.Enums.SearchParamType.String, "family", "Millar")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Patient, Common.Enums.SearchParamType.String, "family", "Millar")]

    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Patient, Common.Enums.SearchParamType.String, "family", "Millar,Smith")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Patient, Common.Enums.SearchParamType.String, "family", "Millar,Smith")]

    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", "Patient/11")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", "Device/22")]

    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", TestData.BaseUrlRemote + "/Patient/11")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", TestData.BaseUrlRemote + "/Patient/11")]

    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", TestData.BaseUrlServer + "/Patient/11")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "subject", TestData.BaseUrlServer + "/Patient/11")]

    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "encounter", "11")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Reference, "encounter", "22")]

    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Token, "code", "System|Code")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Token, "code", "System|Code")]

    [InlineData(Common.Enums.FhirVersion.R4, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Composite, "code-value-concept", "SystemOne|CodeOne$SystemTwo|CodeTwo")]
    [InlineData(Common.Enums.FhirVersion.Stu3, Common.Enums.ResourceType.Observation, Common.Enums.SearchParamType.Composite, "code-value-concept", "SystemOne|CodeOne$SystemTwo|CodeTwo")]

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
        Assert.Equal(queryValue.Split('|')[0], SearchQueryToken.ValueList[0].System);
        Assert.Equal(queryValue.Split('|')[1], SearchQueryToken.ValueList[0].Code);
      }
      else
      {
        Assert.Null(SearchQueryResult.ComponentList);
      }
    }

    [Theory]
    [InlineData("11")]

    public void TestCompositeType(string Thing)
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
      Bug.Common.Interfaces.IFhirUriFactory IFhirUriFactory = GetFhirUriFactory(TestData.BaseUrlServer, new string[]
      {
      Common.Enums.ResourceType.Observation.GetCode(),
      Common.Enums.ResourceType.Patient.GetCode(),
      Common.Enums.ResourceType.Device.GetCode(),
      Common.Enums.ResourceType.Encounter.GetCode()
      });

      IResourceTypeSupport IResourceTypeSupport = new ResourceTypeSupport();
      ISearchParameterCacheMock = ISearchParameterCache_MockFactory.Get(GetSearchParameterList(Common.Enums.FhirVersion.Stu3), GetSearchParameterList(Common.Enums.FhirVersion.R4));
      Mock<IKnownResource> IKnownResourceMock = IKnownResource_MockFactory.Get();
      SearchQueryFactory = new SearchQueryFactory(IFhirUriFactory, IResourceTypeSupport, ISearchParameterCacheMock.Object, IKnownResourceMock.Object);
    }

    private static FhirUriFactory GetFhirUriFactory(string ServersBaseServiceRoot, string[] validResourceNameList)
    {
      Mock<IServiceBaseUrlConfi> IServiceBaseUrlMock = IServiceBaseUrl_MockFactory.Get(ServersBaseServiceRoot, ServersBaseServiceRoot);
      Mock<IR4ValidateResourceName> IR4ValidateResourceNameMock = IR4ValidateResourceName_MockFactory.Get(validResourceNameList);
      Mock<IStu3ValidateResourceName> IStu3ValidateResourceNameMock = IStu3ValidateResourceName_MockFactory.Get(validResourceNameList);
      Mock<IValidateResourceNameFactory> IValidateResourceNameFactoryMock = IValidateResourceNameFactory_MockFactory.Get(IStu3ValidateResourceNameMock.Object, IR4ValidateResourceNameMock.Object);

      FhirUriFactory FhirUriFactory = new FhirUriFactory(IServiceBaseUrlMock.Object, IValidateResourceNameFactoryMock.Object);
      return FhirUriFactory;
    }

    private static List<SearchParameter> GetSearchParameterList(Bug.Common.Enums.FhirVersion fhirVersion)
    {
      return new List<SearchParameter>()
        {
          new SearchParameter()
          {
             Id = 1,
             Name = "code",
             Description = "Bla bla bla",
             FhirPath = "AllergyIntolerance.code | AllergyIntolerance.reaction.substance | Condition.code | (DeviceRequest.code as CodeableConcept) | DiagnosticReport.code | FamilyMemberHistory.condition.code | List.code | Medication.code | (MedicationAdministration.medication as CodeableConcept) | (MedicationDispense.medication as CodeableConcept) | (MedicationRequest.medication as CodeableConcept) | (MedicationStatement.medication as CodeableConcept) | Observation.code | Procedure.code | ServiceRequest.code",
             Url = "http://hl7.org/fhir/SearchParameter/clinical-code",
             SearchParamTypeId = Common.Enums.SearchParamType.Token,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.DiagnosticReport},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Medication}
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             { 
               //new SearchParameterTargetResourceType() { ResourceTypeId = Common.Enums.ResourceType.Organization } 
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "Expression", Definition= "Url Definition" } 
             },
             Created = System.DateTime.Now,
             Updated = System.DateTime.Now
          },
          new SearchParameter()
          {
             Id = 2,
             Name = "value-concept",
             Description = "Bla bla bla",
             FhirPath = "(Observation.value as CodeableConcept)",
             Url = "http://hl7.org/fhir/SearchParameter/Observation-value-concept",
             SearchParamTypeId = Common.Enums.SearchParamType.Token,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             { 
               //new SearchParameterTargetResourceType() { ResourceTypeId = Common.Enums.ResourceType.Organization } 
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "Expression", Definition= "Url Definition" } 
             },
             Created = System.DateTime.Now,
             Updated = System.DateTime.Now
          },
          new SearchParameter()
          {
             Id = 3,
             Name = "code-value-concept",
             Description = "Bla bla bla",
             FhirPath = "Observation",
             Url = "http://hl7.org/fhir/SearchParameter/Observation-code-value-concept",
             SearchParamTypeId = Common.Enums.SearchParamType.Composite,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             { 
               //new SearchParameterTargetResourceType() { ResourceTypeId = Common.Enums.ResourceType.Organization } 
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },
               new SearchParameterComponent() {  Id = 1,  Expression = "value.as(CodeableConcept)", Definition= "http://hl7.org/fhir/SearchParameter/Observation-value-concept" }
             },
             Created = System.DateTime.Now,
             Updated = System.DateTime.Now
          },
          new SearchParameter()
          {
             Id = 4,
             Name = "family",
             Description = "Bla bla bla",
             FhirPath = "Patient.name.family | Practitioner.name.family",
             Url = "http://hl7.org/fhir/SearchParameter/individual-family",
             SearchParamTypeId = Common.Enums.SearchParamType.String,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Patient},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Practitioner},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             { 
               //new SearchParameterTargetResourceType() { ResourceTypeId = Common.Enums.ResourceType.Organization } 
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },               
             },
             Created = System.DateTime.Now,
             Updated = System.DateTime.Now
          },
          new SearchParameter()
          {
             Id = 5,
             Name = "subject",
             Description = "Bla bla bla",
             FhirPath = "Observation.subject",
             Url = "http://hl7.org/fhir/SearchParameter/Observation-subject",
             SearchParamTypeId = Common.Enums.SearchParamType.Reference,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             {
               new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Group},
               new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Device},
               new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Patient},
               new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Location},
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },               
             },
             Created = System.DateTime.Now,
             Updated = System.DateTime.Now
          },
          new SearchParameter()
          {
             Id = 6,
             Name = "encounter",
             Description = "Bla bla bla",
             FhirPath = "Composition.encounter | DeviceRequest.encounter | DiagnosticReport.encounter | DocumentReference.context.encounter | Flag.encounter | List.encounter | NutritionOrder.encounter | Observation.encounter | Procedure.encounter | RiskAssessment.encounter | ServiceRequest.encounter | VisionPrescription.encounter",
             Url = "http://hl7.org/fhir/SearchParameter/clinical-encounter",
             SearchParamTypeId = Common.Enums.SearchParamType.Reference,
             FhirVersionId = fhirVersion,
             ResourceTypeList = new List<SearchParameterResourceType>()
             {
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Observation},
               new SearchParameterResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Procedure},
             },
             TargetResourceTypeList = new List<SearchParameterTargetResourceType>()
             {
               new SearchParameterTargetResourceType() { Id = 1, ResourceTypeId = Common.Enums.ResourceType.Encounter}
             },
             ComponentList = new List<SearchParameterComponent>()
             {
               //new SearchParameterComponent() {  Id = 1,  Expression = "code", Definition= "http://hl7.org/fhir/SearchParameter/clinical-code" },               
             },
             Created = System.DateTime.Now,
             Updated = System.DateTime.Now
          }
        };
    }

  }
}
