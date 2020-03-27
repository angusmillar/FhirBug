using System.Collections.Generic;
using System.Text;
using Moq;
using System;
using Xunit;
using Bug.Logic.UriSupport;
using Bug.Common.ApplicationConfig;
using Bug.Common.Enums;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.ResourceSupport;
using Bug.Stu3Fhir.ResourceSupport;
using Bug.Test.Logic.MockSupport;
using Bug.Common.Interfaces;

namespace Bug.Test.Logic
{
  public class FhirUriFactoryTest
  {
    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "11")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "", "")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlRemote, "Patient", "10", "11")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlRemote, "Patient", "1132b5d1-10c6-4293-a0e3-7bccb1462e3a", "11")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "", "Patient", "1132b5d1-10c6-4293-a0e3-7bccb1462e3a", "11")]

    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "11")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "", "")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlRemote, "Patient", "10", "11")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlRemote, "Patient", "1132b5d1-10c6-4293-a0e3-7bccb1462e3a", "11")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "", "Patient", "1132b5d1-10c6-4293-a0e3-7bccb1462e3a", "11")]
    public void TestFhirUriHistory(FhirVersion fhirVersion, string serversBase, string requestBase, string resourceName, string resourceId, string versionId)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });

      string RequestUrl;
      if (!string.IsNullOrWhiteSpace(versionId))
      {
        RequestUrl = $"{resourceName}/{resourceId}/_history/{versionId}";
      }
      else
      {
        if (!string.IsNullOrWhiteSpace(resourceId))
        {
          RequestUrl = $"{resourceName}/{resourceId}";
        }
        else
        {
          RequestUrl = $"{resourceName}";
        }
      }
      if (!string.IsNullOrWhiteSpace(requestBase))
      {
        RequestUrl = $"{requestBase}/{RequestUrl}";
      }

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          Assert.Equal(resourceName, IFhirUri.ResourseName);

          if (!string.IsNullOrWhiteSpace(resourceId))
          {
            if (resourceId.StartsWith('#'))
            {
              Assert.True(IFhirUri.IsContained);
              Assert.Equal(resourceId.TrimStart('#'), IFhirUri.ResourceId);
            }
            else
            {
              Assert.False(IFhirUri.IsContained);
              Assert.Equal(resourceId, IFhirUri.ResourceId);
            }

          }
          else
          {
            Assert.Equal(string.Empty, IFhirUri.ResourceId);
          }

          if (!string.IsNullOrWhiteSpace(versionId))
          {
            Assert.True(IFhirUri.IsHistoryReferance);
            Assert.Equal(versionId, IFhirUri.VersionId);
          }
          else
          {
            Assert.False(IFhirUri.IsHistoryReferance);
            Assert.Equal(string.Empty, IFhirUri.VersionId);
          }

          if (serversBase == requestBase)
          {
            Assert.Null(IFhirUri.PrimaryServiceRootRemote);
            Assert.True(IFhirUri.IsRelativeToServer);
            Assert.Equal(new Uri(requestBase), IFhirUri.UriPrimaryServiceRoot);
          }
          else
          {
            if (!string.IsNullOrWhiteSpace(requestBase))
            {
              Assert.NotNull(IFhirUri.PrimaryServiceRootRemote);
              Assert.False(IFhirUri.IsRelativeToServer);
              Assert.Equal(new Uri(requestBase), IFhirUri.PrimaryServiceRootRemote);
            }
            else
            {
              Assert.Null(IFhirUri.PrimaryServiceRootRemote);
              Assert.True(IFhirUri.IsRelativeToServer);
            }
          }

          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsOperation);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsUrn);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(string.Empty, IFhirUri.OperationName);
          Assert.Null(IFhirUri.OperationType);
          Assert.Equal(string.Empty, IFhirUri.Query);
          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]
    [InlineData(FhirVersion.R4, "Patient", "10", "11")]
    [InlineData(FhirVersion.Stu3, "Patient", "10", "11")]
    public void TestFhirUriCanonical(FhirVersion fhirVersion, string resourceName, string resourceId, string CanonicalVersionId)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(TestData.BaseUrlServer, new string[] { resourceName });

      
      string RequestUrl = $"{resourceName}/{resourceId}|{CanonicalVersionId}";
      
            //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          Assert.Equal(resourceName, IFhirUri.ResourseName);
          Assert.Equal(resourceId, IFhirUri.ResourceId);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Equal(CanonicalVersionId, IFhirUri.CanonicalVersionId);

          Assert.Equal(new Uri(TestData.BaseUrlServer), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsOperation);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsUrn);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(string.Empty, IFhirUri.OperationName);
          Assert.Null(IFhirUri.OperationType);
          Assert.Equal(string.Empty, IFhirUri.Query);
          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlRemote)]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlRemote)]
    public void TestFhirUriBaseHistory(FhirVersion fhirVersion, string serversBase, string requestBase)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { "Patient" });

      string RequestUrl = $"{requestBase}/_history";

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          Assert.True(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.ResourseName);
          Assert.False(IFhirUri.IsContained);
          Assert.Equal(string.Empty, IFhirUri.ResourceId);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Equal(new Uri(TestData.BaseUrlRemote), IFhirUri.PrimaryServiceRootRemote);
          Assert.False(IFhirUri.IsRelativeToServer);
          Assert.Equal(new Uri(requestBase), IFhirUri.UriPrimaryServiceRoot);
          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsOperation);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsUrn);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(string.Empty, IFhirUri.OperationName);
          Assert.Null(IFhirUri.OperationType);
          Assert.Equal(string.Empty, IFhirUri.Query);
          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "10")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "#10")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "1132b5d1-10c6-4293-a0e3-7bccb1462e3a")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "#1132b5d1-10c6-4293-a0e3-7bccb1462e3a")]

    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "10")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "#10")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "1132b5d1-10c6-4293-a0e3-7bccb1462e3a")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "#1132b5d1-10c6-4293-a0e3-7bccb1462e3a")]
    public void TestFhirUri_ResourceIdOnly(FhirVersion fhirVersion, string serversBase, string resourceId)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { });

      string RequestUrl = resourceId;

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {

          if (resourceId.StartsWith('#'))
            Assert.True(IFhirUri.IsContained);
          else
            Assert.False(IFhirUri.IsContained);

          Assert.Equal(string.Empty, IFhirUri.ResourseName);
          Assert.False(IFhirUri.IsRelativeToServer);
          Assert.Equal(resourceId.TrimStart('#'), IFhirUri.ResourceId);
          Assert.Null(IFhirUri.UriPrimaryServiceRoot);
          Assert.False(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Null(IFhirUri.PrimaryServiceRootRemote);
          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsOperation);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsUrn);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(string.Empty, IFhirUri.OperationName);
          Assert.Null(IFhirUri.OperationType);
          Assert.Equal(string.Empty, IFhirUri.Query);
          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "Patient", "#10")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "Patient", "10")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "", "#10")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "", "10")]

    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "Patient", "#10")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "Patient", "10")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "", "#10")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "", "10")]
    public void TestFhirUri_Contained(FhirVersion fhirVersion, string serversBase, string resourceName, string resourceId)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });

      string RequestUrl;
      if (!string.IsNullOrWhiteSpace(resourceName))
      {
        RequestUrl = $"{resourceName}/{resourceId}";
      }
      else
      {
        RequestUrl = $"{resourceId}";
      }


      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {

          if (!string.IsNullOrWhiteSpace(resourceName))
          {
            Assert.Equal(resourceName, IFhirUri.ResourseName);
          }
          else
          {
            Assert.Equal(string.Empty, IFhirUri.ResourseName);
          }

          if (!string.IsNullOrWhiteSpace(resourceId))
          {
            if (resourceId.StartsWith('#'))
            {
              Assert.True(IFhirUri.IsContained);
              //False as it is relative to the resource not the server
              Assert.False(IFhirUri.IsRelativeToServer);
              Assert.Equal(resourceId.TrimStart('#'), IFhirUri.ResourceId);
              Assert.Null(IFhirUri.UriPrimaryServiceRoot);
            }
            else
            {
              Assert.False(IFhirUri.IsContained);
              Assert.Equal(resourceId, IFhirUri.ResourceId);
              if (string.IsNullOrWhiteSpace(resourceName))
              {
                Assert.False(IFhirUri.IsRelativeToServer);
                Assert.Null(IFhirUri.UriPrimaryServiceRoot);
              }
              else
              {
                Assert.True(IFhirUri.IsRelativeToServer);
                Assert.Equal(new Uri(serversBase), IFhirUri.UriPrimaryServiceRoot);
              }
            }
          }

          Assert.False(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Null(IFhirUri.PrimaryServiceRootRemote);


          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsOperation);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsUrn);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(string.Empty, IFhirUri.OperationName);
          Assert.Null(IFhirUri.OperationType);
          Assert.Equal(string.Empty, IFhirUri.Query);
          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]    
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "PatientStu3", "PatientR4", "10")]    
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "PatientR4", "PatientStu3", "10")]
    public void TestFhirUri_ResourceNameValidForFhirVersion(FhirVersion fhirVersion, string serversBase, string resourceNameR4, string resourceNameStu3, string resourceId)
    {
      // Prepare
      Mock<IServiceBaseUrl> IServiceBaseUrlMock = IServiceBaseUrl_MockFactory.Get(serversBase, serversBase);
      Mock<IR4ValidateResourceName> IR4ValidateResourceNameMock = IR4ValidateResourceName_MockFactory.Get(new string[] { resourceNameR4 });
      Mock<IStu3ValidateResourceName> IStu3ValidateResourceNameMock = IStu3ValidateResourceName_MockFactory.Get(new string[] { resourceNameStu3 });
      Mock<IValidateResourceNameFactory> IValidateResourceNameFactoryMock = IValidateResourceNameFactory_MockFactory.Get(IStu3ValidateResourceNameMock.Object, IR4ValidateResourceNameMock.Object);

      FhirUriFactory FhirUriFactory = new FhirUriFactory(IServiceBaseUrlMock.Object, IValidateResourceNameFactoryMock.Object);

      string RequestUrl;
      if (fhirVersion == FhirVersion.Stu3)
      {
        RequestUrl = $"{resourceNameStu3}/{resourceId}";
      }
      else
      {
        RequestUrl = $"{resourceNameR4}/{resourceId}";
      }
      

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          if (fhirVersion == FhirVersion.Stu3)
          {
            Assert.Equal(resourceNameStu3, IFhirUri.ResourseName);            
          }
          else
          {
            Assert.Equal(resourceNameR4, IFhirUri.ResourseName);
          }
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.False(IFhirUri.IsContained);
          Assert.Equal(resourceId.TrimStart('#'), IFhirUri.ResourceId);
          Assert.True(IFhirUri.IsRelativeToServer);
          Assert.Equal(new Uri(serversBase), IFhirUri.UriPrimaryServiceRoot);
          Assert.False(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Null(IFhirUri.PrimaryServiceRootRemote);
          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsOperation);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsUrn);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          
          Assert.Equal(string.Empty, IFhirUri.OperationName);
          Assert.Null(IFhirUri.OperationType);
          Assert.Equal(string.Empty, IFhirUri.Query);
          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);

        }
        else
        {
          Assert.Equal("some error message", ErrorMessage);
        }
      }
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "MyOperation", "query1=one,query2=two")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "MyOperation", "query1=one,query2=two")]
    public void TestFhirUri_OperationBase(FhirVersion fhirVersion, string serversBase, string requestBase, string operationName, string query)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { });

      string RequestUrl = $"{requestBase}/${operationName}?{query}";

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          Assert.Equal(operationName, IFhirUri.OperationName);
          Assert.Equal(OperationScope.Base, IFhirUri.OperationType);
          Assert.Equal(string.Empty, IFhirUri.ResourseName);
          Assert.Equal(string.Empty, IFhirUri.ResourceId);
          Assert.False(IFhirUri.IsContained);
          Assert.True(IFhirUri.IsRelativeToServer);
          Assert.Equal(new Uri(serversBase), IFhirUri.UriPrimaryServiceRoot);
          Assert.False(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Null(IFhirUri.PrimaryServiceRootRemote);
          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsUrn);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(query, IFhirUri.Query);
          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "MyOperation", "query1=one,query2=two")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "MyOperation", "query1=one,query2=two")]
    public void TestFhirUri_OperationResource(FhirVersion fhirVersion, string serversBase, string requestBase, string resourceName, string operationName, string query)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });

      string RequestUrl = $"{requestBase}/{resourceName}/${operationName}?{query}";

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          Assert.Equal(operationName, IFhirUri.OperationName);
          Assert.Equal(OperationScope.Resource, IFhirUri.OperationType);
          Assert.Equal(resourceName, IFhirUri.ResourseName);
          Assert.Equal(string.Empty, IFhirUri.ResourceId);
          Assert.False(IFhirUri.IsContained);
          Assert.True(IFhirUri.IsRelativeToServer);
          Assert.Equal(new Uri(serversBase), IFhirUri.UriPrimaryServiceRoot);
          Assert.False(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Null(IFhirUri.PrimaryServiceRootRemote);
          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsUrn);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(query, IFhirUri.Query);
          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "MyOperation", "query1=one,query2=two")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "MyOperation", "query1=one,query2=two")]
    public void TestFhirUri_OperationResourceInstance(FhirVersion fhirVersion, string serversBase, string requestBase, string resourceName, string resourceId, string operationName, string query)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });

      string RequestUrl = $"{requestBase}/{resourceName}/{resourceId}/${operationName}?{query}";

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          Assert.Equal(operationName, IFhirUri.OperationName);
          Assert.Equal(OperationScope.Instance, IFhirUri.OperationType);
          Assert.Equal(resourceName, IFhirUri.ResourseName);
          Assert.Equal(resourceId, IFhirUri.ResourceId);
          Assert.False(IFhirUri.IsContained);
          Assert.True(IFhirUri.IsRelativeToServer);
          Assert.Equal(new Uri(serversBase), IFhirUri.UriPrimaryServiceRoot);
          Assert.False(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Null(IFhirUri.PrimaryServiceRootRemote);
          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsUrn);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(query, IFhirUri.Query);
          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "_search")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "_search")]
    public void TestFhirUri_FormData(FhirVersion fhirVersion, string serversBase, string requestBase, string resourceName, string search)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });

      string RequestUrl = $"{requestBase}/{resourceName}/{search}";

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          Assert.Equal(string.Empty, IFhirUri.OperationName);
          Assert.Null(IFhirUri.OperationType);
          Assert.Equal(resourceName, IFhirUri.ResourseName);
          Assert.Equal(string.Empty, IFhirUri.ResourceId);
          Assert.False(IFhirUri.IsContained);
          Assert.True(IFhirUri.IsRelativeToServer);
          Assert.Equal(new Uri(serversBase), IFhirUri.UriPrimaryServiceRoot);
          Assert.False(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Null(IFhirUri.PrimaryServiceRootRemote);
          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsUrn);
          Assert.True(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(string.Empty, IFhirUri.Query);
          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient")]
    public void TestFhirUri_RubbishOnTheEnd(FhirVersion fhirVersion, string serversBase, string requestBase, string resourceName)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });

      string RequestUrl = $"{requestBase}/{resourceName}/10/Rubbish";

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        Assert.False(true);
      }
      else
      {
        Assert.True(!string.IsNullOrWhiteSpace(ErrorMessage));
        Assert.Null(IFhirUri);
      }
      
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "Patient", "urn:uuid:61ebe359-bfdc-4613-8bf2-c5e300945f0a")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "Patient", "urn:uuid:61ebe359-bfdc-4613-8bf2-c5e300945f0a")]
    public void TestFhirUri_urn_uuid(FhirVersion fhirVersion, string serversBase, string resourceName, string uuid)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });

      string RequestUrl = uuid;

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          Assert.Equal(string.Empty, IFhirUri.OperationName);
          Assert.Null(IFhirUri.OperationType);
          Assert.Equal(string.Empty, IFhirUri.ResourseName);
          Assert.Equal(string.Empty, IFhirUri.ResourceId);
          Assert.False(IFhirUri.IsContained);
          Assert.False(IFhirUri.IsRelativeToServer);
          Assert.Null(IFhirUri.UriPrimaryServiceRoot);
          Assert.False(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Null(IFhirUri.PrimaryServiceRootRemote);
          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(string.Empty, IFhirUri.Query);
          Assert.Equal(UrnType.uuid, IFhirUri.UrnType);
          Assert.Equal(uuid, IFhirUri.Urn);
          Assert.True(IFhirUri.IsUrn);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "Patient", "urn:oid:1.2.36.1.2001.1001.101")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "Patient", "urn:oid:1.2.36.1.2001.1001.101")]
    public void TestFhirUri_urn_oid(FhirVersion fhirVersion, string serversBase, string resourceName, string uuid)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });

      string RequestUrl = uuid;

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          Assert.Equal(string.Empty, IFhirUri.OperationName);
          Assert.Null(IFhirUri.OperationType);
          Assert.Equal(string.Empty, IFhirUri.ResourseName);
          Assert.Equal(string.Empty, IFhirUri.ResourceId);
          Assert.False(IFhirUri.IsContained);
          Assert.False(IFhirUri.IsRelativeToServer);
          Assert.Null(IFhirUri.UriPrimaryServiceRoot);
          Assert.False(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Null(IFhirUri.PrimaryServiceRootRemote);
          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsCompartment);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(string.Empty, IFhirUri.CompartmentalisedResourseName);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);
          Assert.Equal(string.Empty, IFhirUri.Query);
          Assert.Equal(UrnType.oid, IFhirUri.UrnType);
          Assert.Equal(uuid, IFhirUri.Urn);
          Assert.True(IFhirUri.IsUrn);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);
        }
      }
      else
      {
        Assert.Equal("some error message", ErrorMessage);
      }
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "Patient", "urn:oid:1.2.36.1.ABC.2001.1001.101")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "Patient", "urn:oid:1.2.36.1.ABC.2001.1001.101")]
    public void TestFhirUri__urn_oid_invalid(FhirVersion fhirVersion, string serversBase, string resourceName, string uuid)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });

      string RequestUrl = uuid;

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        Assert.False(true);        
      }
      else
      {        
        Assert.True(!string.IsNullOrWhiteSpace(ErrorMessage));
        Assert.Null(IFhirUri);
      }
      
      
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, "Patient", "urn:uuid:61ebe359-XXXX-4613-8bf2-c5e300945f0a")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, "Patient", "urn:uuid:61ebe359-XXXX-4613-8bf2-c5e300945f0a")]
    public void TestFhirUri__urn_uuid_invalid(FhirVersion fhirVersion, string serversBase, string resourceName, string uuid)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });

      string RequestUrl = uuid;

      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        Assert.False(true);
      }
      else
      {
        Assert.True(!string.IsNullOrWhiteSpace(ErrorMessage));
        Assert.Null(IFhirUri);
      }
     
    }

    [Theory]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "Encounter", "")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "Encounter", "")]
    //Incorrect Compartment as it is not a known ResourceType
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "Unknown", "")]
    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "Unknown", "")]

    [InlineData(FhirVersion.R4, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "Unknown", "query_one=one, query_two=two")]
    [InlineData(FhirVersion.Stu3, TestData.BaseUrlServer, TestData.BaseUrlServer, "Patient", "10", "Unknown", "query_one=one, query_two=two")]
    public void TestFhirUri_Compartment(FhirVersion fhirVersion, string serversBase, string requestBase, string resourceName, string resourceId, string compartmentName, string query)
    {
      // Prepare
      //hit: the resource is unknown because we do not pass it into the GetFhirUriFactory below, we only pass in the resourceName and not the compatmentName 
      FhirUriFactory FhirUriFactory;
      if (compartmentName == "Unknown")
      {
        FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName });
      }
      else
      {
        FhirUriFactory = GetFhirUriFactory(serversBase, new string[] { resourceName, compartmentName });
      }

      string RequestUrl;
      if (string.IsNullOrWhiteSpace(query))
      {
        RequestUrl = $"{requestBase}/{resourceName}/{resourceId}/{compartmentName}";
      }
      else
      {
        RequestUrl = $"{requestBase}/{resourceName}/{resourceId}/{compartmentName}?{query}";
      }


      //Act
      if (FhirUriFactory.TryParse(RequestUrl, fhirVersion, out IFhirUri? IFhirUri, out string ErrorMessage))
      {
        //Assert
        if (IFhirUri is object)
        {
          Assert.Equal(string.Empty, IFhirUri.OperationName);
          Assert.Null(IFhirUri.OperationType);
          Assert.Equal(resourceName, IFhirUri.ResourseName);
          Assert.Equal(resourceId, IFhirUri.ResourceId);
          Assert.False(IFhirUri.IsContained);
          Assert.True(IFhirUri.IsRelativeToServer);
          Assert.Equal(new Uri(serversBase), IFhirUri.UriPrimaryServiceRoot);
          Assert.False(IFhirUri.IsHistoryReferance);
          Assert.Equal(string.Empty, IFhirUri.VersionId);
          Assert.Null(IFhirUri.PrimaryServiceRootRemote);
          Assert.Equal(new Uri(serversBase), IFhirUri.PrimaryServiceRootServers);
          Assert.True(IFhirUri.IsCompartment);
          Assert.Equal(compartmentName, IFhirUri.CompartmentalisedResourseName);
          Assert.False(IFhirUri.ErrorInParseing);
          Assert.False(IFhirUri.IsMetaData);
          Assert.False(IFhirUri.IsUrn);
          Assert.False(IFhirUri.IsFormDataSearch);
          Assert.Equal(RequestUrl, IFhirUri.OriginalString);
          Assert.Equal(compartmentName, IFhirUri.CompartmentalisedResourseName);
          Assert.True(IFhirUri.IsCompartment);
          Assert.Equal(fhirVersion, IFhirUri.FhirVersion);

          Assert.Equal(string.Empty, IFhirUri.Urn);
          Assert.Null(IFhirUri.UrnType);
          Assert.Equal(string.Empty, IFhirUri.ParseErrorMessage);

          if (string.IsNullOrWhiteSpace(query))
            Assert.Equal(string.Empty, IFhirUri.Query);
          else
            Assert.Equal(query, IFhirUri.Query);
        }
      }
      else
      {
        if (string.IsNullOrWhiteSpace(query))
          Assert.Equal("The URI has extra unknown content near the end of : 'Unknown'. The full URI was: 'http://base/stuff/Patient/10/Unknown'", ErrorMessage);
        else
          Assert.Equal("The URI has extra unknown content near the end of : 'Unknown'. The full URI was: 'http://base/stuff/Patient/10/Unknown?query_one=one, query_two=two'", ErrorMessage);
      }
    }


    private static FhirUriFactory GetFhirUriFactory(string ServersBaseServiceRoot, string[] validResourceNameList)
    {
      Mock<IServiceBaseUrl> IServiceBaseUrlMock = IServiceBaseUrl_MockFactory.Get(ServersBaseServiceRoot, ServersBaseServiceRoot);
      Mock<IR4ValidateResourceName> IR4ValidateResourceNameMock = IR4ValidateResourceName_MockFactory.Get(validResourceNameList);
      Mock<IStu3ValidateResourceName> IStu3ValidateResourceNameMock = IStu3ValidateResourceName_MockFactory.Get(validResourceNameList);
      Mock<IValidateResourceNameFactory> IValidateResourceNameFactoryMock = IValidateResourceNameFactory_MockFactory.Get(IStu3ValidateResourceNameMock.Object, IR4ValidateResourceNameMock.Object);

      FhirUriFactory FhirUriFactory = new FhirUriFactory(IServiceBaseUrlMock.Object, IValidateResourceNameFactoryMock.Object);
      return FhirUriFactory;
    }

   

  }
}
