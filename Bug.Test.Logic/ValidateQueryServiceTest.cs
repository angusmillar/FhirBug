﻿using System.Collections.Generic;
using System.Text;
using Moq;
using System;
using Xunit;
using Bug.Logic.UriSupport;
using Bug.Common.ApplicationConfig;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.ResourceSupport;
using Bug.Stu3Fhir.ResourceSupport;
using Bug.Test.Logic.MockSupport;
using Bug.Logic.Service.ValidatorService;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Create;
using Bug.Logic.Query.FhirApi.Update;
using Bug.Logic.Query.FhirApi.Read;
using Bug.Logic.Query.FhirApi.Delete;
using Bug.Logic.Query.FhirApi.VRead;
using Bug.Logic.Query.FhirApi.HistoryBase;
using Bug.Logic.Query.FhirApi.HistoryInstance;
using Bug.Logic.Query.FhirApi.HistoryResource;

namespace Bug.Test.Logic
{
  public class ValidateQueryServiceTest
  {
    [Theory]
    [InlineData(Common.Enums.FhirVersion.Stu3, "Patient", "")]
    [InlineData(Common.Enums.FhirVersion.R4, "Patient", "")]
    public void CreateQueryTest(Common.Enums.FhirVersion fhirVersion, string resourceName, string resourceId)
    {
      //Setup
      var IOperationOutcomeSupportMock = IOperationOutcomeSupport_MockFactory.Get();
      var IFhirResourceNameSupportMock = IFhirResourceNameSupport_MockFactory.Get(resourceName);
      var IFhirResourceIdSupportMock = IFhirResourceIdSupport_MockFactory.Get(resourceId);      

      FhirUriFactory FhirUriFactory = GetFhirUriFactory(resourceName);

      ValidateQueryService ValidateQueryService = new ValidateQueryService(IOperationOutcomeSupportMock.Object, IFhirResourceNameSupportMock.Object, IFhirResourceIdSupportMock.Object, FhirUriFactory);

      var CreateQuery = new CreateQuery(
        HttpVerb.POST,
        fhirVersion,
        new Uri($"{TestData.BaseUrlServer}/{resourceName}"),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), //RequestQuery
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), //Headers
        resourceName,
        new Common.FhirTools.FhirResource(fhirVersion));

      //Act
      bool Result = ValidateQueryService.IsValid(CreateQuery, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome);

      //Assert
      Assert.True(Result);
      Assert.Null(IsNotValidOperationOutCome);
    }

    [Theory]
    [InlineData(Common.Enums.FhirVersion.Stu3, "Patient", "10")]
    [InlineData(Common.Enums.FhirVersion.R4, "Patient", "10")]
    public void UpdateQueryTest(Common.Enums.FhirVersion fhirVersion, string resourceName, string resourceId)
    {
      //Setup
      var IOperationOutcomeSupportMock = IOperationOutcomeSupport_MockFactory.Get();
      var IFhirResourceNameSupportMock = IFhirResourceNameSupport_MockFactory.Get(resourceName);
      var IFhirResourceIdSupportMock = IFhirResourceIdSupport_MockFactory.Get(resourceId);

      FhirUriFactory FhirUriFactory = GetFhirUriFactory(resourceName);

      ValidateQueryService ValidateQueryService = new ValidateQueryService(IOperationOutcomeSupportMock.Object, IFhirResourceNameSupportMock.Object, IFhirResourceIdSupportMock.Object, FhirUriFactory);

      var UpdateQuery = new UpdateQuery(
        HttpVerb.PUT,
        fhirVersion,
        new Uri($"{TestData.BaseUrlServer}/{resourceName}/{resourceId}"),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        resourceName,
        resourceId,
        new Common.FhirTools.FhirResource(fhirVersion));

      //Act
      bool Result = ValidateQueryService.IsValid(UpdateQuery, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome);

      //Assert
      Assert.True(Result);
      Assert.Null(IsNotValidOperationOutCome);
    }

    [Theory]
    [InlineData(Common.Enums.FhirVersion.Stu3, "Patient", "10")]
    [InlineData(Common.Enums.FhirVersion.R4, "Patient", "10")]
    public void ReadQueryTest(Common.Enums.FhirVersion fhirVersion, string resourceName, string resourceId)
    {
      //Setup
      var IOperationOutcomeSupportMock = IOperationOutcomeSupport_MockFactory.Get();
      var IFhirResourceNameSupportMock = IFhirResourceNameSupport_MockFactory.Get(resourceName);
      var IFhirResourceIdSupportMock = IFhirResourceIdSupport_MockFactory.Get(resourceId);

      FhirUriFactory FhirUriFactory = GetFhirUriFactory(resourceName);

      ValidateQueryService ValidateQueryService = new ValidateQueryService(IOperationOutcomeSupportMock.Object, IFhirResourceNameSupportMock.Object, IFhirResourceIdSupportMock.Object, FhirUriFactory);

      var ReadQuery = new ReadQuery(
        HttpVerb.GET,
        fhirVersion,
        new Uri($"{TestData.BaseUrlServer}/{resourceName}/{resourceId}"),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        resourceName,
        resourceId);

      //Act
      bool Result = ValidateQueryService.IsValid(ReadQuery, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome);

      //Assert
      Assert.True(Result);
      Assert.Null(IsNotValidOperationOutCome);
    }

    [Theory]
    [InlineData(Common.Enums.FhirVersion.Stu3, "Patient", "10")]
    [InlineData(Common.Enums.FhirVersion.R4, "Patient", "10")]
    public void DeleteQueryTest(Common.Enums.FhirVersion fhirVersion, string resourceName, string resourceId)
    {
      //Setup
      var IOperationOutcomeSupportMock = IOperationOutcomeSupport_MockFactory.Get();
      var IFhirResourceNameSupportMock = IFhirResourceNameSupport_MockFactory.Get(resourceName);
      var IFhirResourceIdSupportMock = IFhirResourceIdSupport_MockFactory.Get(resourceId);

      FhirUriFactory FhirUriFactory = GetFhirUriFactory(resourceName);

      ValidateQueryService ValidateQueryService = new ValidateQueryService(IOperationOutcomeSupportMock.Object, IFhirResourceNameSupportMock.Object, IFhirResourceIdSupportMock.Object, FhirUriFactory);

      var DeleteQuery = new DeleteQuery(
        HttpVerb.GET,
        fhirVersion,
        new Uri($"{TestData.BaseUrlServer}/{resourceName}/{resourceId}"),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        resourceName,
        resourceId);

      //Act
      bool Result = ValidateQueryService.IsValid(DeleteQuery, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome);

      //Assert
      Assert.True(Result);
      Assert.Null(IsNotValidOperationOutCome);
    }

    [Theory]
    [InlineData(Common.Enums.FhirVersion.Stu3, "Patient", "10", 15)]
    [InlineData(Common.Enums.FhirVersion.R4, "Patient", "10", 15)]
    public void VReadQueryTest(Common.Enums.FhirVersion fhirVersion, string resourceName, string resourceId, int versionId)
    {
      //Setup
      var IOperationOutcomeSupportMock = IOperationOutcomeSupport_MockFactory.Get();
      var IFhirResourceNameSupportMock = IFhirResourceNameSupport_MockFactory.Get(resourceName);
      var IFhirResourceIdSupportMock = IFhirResourceIdSupport_MockFactory.Get(resourceId);

      FhirUriFactory FhirUriFactory = GetFhirUriFactory(resourceName);

      ValidateQueryService ValidateQueryService = new ValidateQueryService(IOperationOutcomeSupportMock.Object, IFhirResourceNameSupportMock.Object, IFhirResourceIdSupportMock.Object, FhirUriFactory);

      var VReadQuery = new VReadQuery(
        HttpVerb.GET,
        fhirVersion,
        new Uri($"{TestData.BaseUrlServer}/{resourceName}/{resourceId}/_history/{versionId.ToString()}"),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        resourceName,
        resourceId,
        versionId);

      //Act
      bool Result = ValidateQueryService.IsValid(VReadQuery, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome);

      //Assert
      Assert.True(Result);
      Assert.Null(IsNotValidOperationOutCome);
    }

    [Theory]
    [InlineData(Common.Enums.FhirVersion.Stu3)]
    [InlineData(Common.Enums.FhirVersion.R4)]
    public void HistoryBaseQueryTest(Common.Enums.FhirVersion fhirVersion)
    {
      //Setup
      var IOperationOutcomeSupportMock = IOperationOutcomeSupport_MockFactory.Get();
      var IFhirResourceNameSupportMock = IFhirResourceNameSupport_MockFactory.Get(string.Empty);
      var IFhirResourceIdSupportMock = IFhirResourceIdSupport_MockFactory.Get(string.Empty);

      FhirUriFactory FhirUriFactory = GetFhirUriFactory(string.Empty);

      ValidateQueryService ValidateQueryService = new ValidateQueryService(IOperationOutcomeSupportMock.Object, IFhirResourceNameSupportMock.Object, IFhirResourceIdSupportMock.Object, FhirUriFactory);

      var HistoryBaseQuery = new HistoryBaseQuery(
        HttpVerb.GET,
        fhirVersion,
        new Uri($"{TestData.BaseUrlServer}/_history"),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
        );

      //Act
      bool Result = ValidateQueryService.IsValid(HistoryBaseQuery, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome);

      //Assert
      Assert.True(Result);
      Assert.Null(IsNotValidOperationOutCome);
    }

    [Theory]
    [InlineData(Common.Enums.FhirVersion.Stu3, "Patient", "10")]
    [InlineData(Common.Enums.FhirVersion.R4, "Patient", "10")]
    public void HistoryInstanceQueryTest(Common.Enums.FhirVersion fhirVersion, string resourceName, string resourceId)
    {
      //Setup
      var IOperationOutcomeSupportMock = IOperationOutcomeSupport_MockFactory.Get();
      var IFhirResourceNameSupportMock = IFhirResourceNameSupport_MockFactory.Get(resourceName);
      var IFhirResourceIdSupportMock = IFhirResourceIdSupport_MockFactory.Get(resourceId);

      FhirUriFactory FhirUriFactory = GetFhirUriFactory(resourceName);

      ValidateQueryService ValidateQueryService = new ValidateQueryService(IOperationOutcomeSupportMock.Object, IFhirResourceNameSupportMock.Object, IFhirResourceIdSupportMock.Object, FhirUriFactory);

      var HistoryInstanceQuery = new HistoryInstanceQuery(
        HttpVerb.GET,
        fhirVersion,
        new Uri($"{TestData.BaseUrlServer}/{resourceName}/{resourceId}/_history"),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        resourceName,
        resourceId);

      //Act
      bool Result = ValidateQueryService.IsValid(HistoryInstanceQuery, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome);

      //Assert
      Assert.True(Result);
      Assert.Null(IsNotValidOperationOutCome);
    }

    [Theory]
    [InlineData(Common.Enums.FhirVersion.Stu3, "Patient")]
    [InlineData(Common.Enums.FhirVersion.R4, "Patient")]
    public void HistoryResourceQueryTest(Common.Enums.FhirVersion fhirVersion, string resourceName)
    {      
      //Setup
      var IOperationOutcomeSupportMock = IOperationOutcomeSupport_MockFactory.Get();
      var IFhirResourceNameSupportMock = IFhirResourceNameSupport_MockFactory.Get(resourceName);
      var IFhirResourceIdSupportMock = IFhirResourceIdSupport_MockFactory.Get(string.Empty);

      FhirUriFactory FhirUriFactory = GetFhirUriFactory(resourceName);

      ValidateQueryService ValidateQueryService = new ValidateQueryService(IOperationOutcomeSupportMock.Object, IFhirResourceNameSupportMock.Object, IFhirResourceIdSupportMock.Object, FhirUriFactory);

      var HistoryResourceQuery = new HistoryResourceQuery(
        HttpVerb.GET,
        fhirVersion,
        new Uri($"{TestData.BaseUrlServer}/{resourceName}/_history"),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
        resourceName);

      //Act, 
      bool Result = ValidateQueryService.IsValid(HistoryResourceQuery, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome);

      //Assert
      Assert.True(Result);
      Assert.Null(IsNotValidOperationOutCome);
    }

    private static FhirUriFactory GetFhirUriFactory(string resourceName)
    {
      var ValidResourceNames = new string[] { resourceName };
      Mock<IServiceBaseUrlConfi> IServiceBaseUrlMock = IServiceBaseUrl_MockFactory.Get(TestData.BaseUrlServer, TestData.BaseUrlServer);
      Mock<IR4ValidateResourceName> IR4ValidateResourceNameMock = IR4ValidateResourceName_MockFactory.Get(ValidResourceNames);
      Mock<IStu3ValidateResourceName> IStu3ValidateResourceNameMock = IStu3ValidateResourceName_MockFactory.Get(ValidResourceNames);
      Mock<IValidateResourceNameFactory> IValidateResourceNameFactoryMock = IValidateResourceNameFactory_MockFactory.Get(IStu3ValidateResourceNameMock.Object, IR4ValidateResourceNameMock.Object);
      FhirUriFactory FhirUriFactory = new FhirUriFactory(IServiceBaseUrlMock.Object, IValidateResourceNameFactoryMock.Object);
      return FhirUriFactory;
    }
  }
}
