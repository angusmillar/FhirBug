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

namespace Bug.Test.Logic
{
  public class FhirUriFactoryTest
  {
    [Theory]
    [InlineData(FhirVersion.Stu3, "http://base/stu3", "http://base/stu3", "Patient", "10", "11")]    
    [InlineData(FhirVersion.Stu3, "http://base/stu3", "http://base/stu3", "Patient", "10", "")]    
    [InlineData(FhirVersion.Stu3, "http://base/stu3", "http://base/stu3", "Patient", "", "")]
    [InlineData(FhirVersion.Stu3, "http://base/stu3", "http://remoteBase/stu3", "Patient", "10", "11")]

    [InlineData(FhirVersion.R4, "http://base/r4", "http://base/r4", "Patient", "10", "11")]
    [InlineData(FhirVersion.R4, "http://base/r4", "http://base/r4", "Patient", "10", "")]    
    [InlineData(FhirVersion.R4, "http://base/r4", "http://base/r4", "Patient", "", "")]
    [InlineData(FhirVersion.R4, "http://base/r4", "http://remotebase/r4", "Patient", "10", "11")]
    public void TestFhirUriHistory(FhirVersion fhirVersion, string ServersBaseServiceRoot, string ReqestsBaseServiceRoot, string resourceName, string resourceId, string versionId)
    {
      // Prepare
      FhirUriFactory FhirUriFactory = GetFhirUriFactory(ServersBaseServiceRoot, resourceName);

      string RequestUrl = string.Empty;
      if (!string.IsNullOrWhiteSpace(versionId))
      {
        RequestUrl = $"{ReqestsBaseServiceRoot}/{resourceName}/{resourceId}/_history/{versionId}";
      }
      else
      {
        if (!string.IsNullOrWhiteSpace(resourceId))
        {
          RequestUrl = $"{ReqestsBaseServiceRoot}/{resourceName}/{resourceId}";          
        }
        else
        {
          RequestUrl = $"{ReqestsBaseServiceRoot}/{resourceName}";
        }        
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
          if (ServersBaseServiceRoot == ReqestsBaseServiceRoot)
          {
            Assert.Null(IFhirUri.PrimaryServiceRootRemote);
            Assert.True(IFhirUri.IsRelativeToServer);
            Assert.Equal(new Uri(ReqestsBaseServiceRoot), IFhirUri.UriPrimaryServiceRoot);            
          }
          else
          {
            Assert.NotNull(IFhirUri.PrimaryServiceRootRemote);
            Assert.False(IFhirUri.IsRelativeToServer);
            Assert.Equal(new Uri(ReqestsBaseServiceRoot), IFhirUri.PrimaryServiceRootRemote);            
          }
          Assert.Equal(new Uri(ServersBaseServiceRoot), IFhirUri.PrimaryServiceRootServers);

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


    private static FhirUriFactory GetFhirUriFactory(string ServersBaseServiceRoot, string ValidResourceName)
    {
      Mock<IServiceBaseUrl> IServiceBaseUrlMock = new Mock<IServiceBaseUrl>();
      IServiceBaseUrlMock.Setup(x =>
      x.Url(FhirVersion.R4)).Returns(new Uri(ServersBaseServiceRoot));
      IServiceBaseUrlMock.Setup(x =>
      x.Url(FhirVersion.Stu3)).Returns(new Uri(ServersBaseServiceRoot));

      Mock<IR4ValidateResourceName> IR4ValidateResourceNameMock = new Mock<IR4ValidateResourceName>();
      IR4ValidateResourceNameMock.Setup(x => x.IsKnownResource(ValidResourceName)).Returns(true);

      Mock<IStu3ValidateResourceName> IStu3ValidateResourceNameMock = new Mock<IStu3ValidateResourceName>();
      IStu3ValidateResourceNameMock.Setup(x => x.IsKnownResource(ValidResourceName)).Returns(true);

      Mock<IValidateResourceNameFactory> IValidateResourceNameFactoryMock = new Mock<IValidateResourceNameFactory>();
      IValidateResourceNameFactoryMock.Setup(x => x.GetStu3()).Returns(IStu3ValidateResourceNameMock.Object);
      IValidateResourceNameFactoryMock.Setup(x => x.GetR4()).Returns(IR4ValidateResourceNameMock.Object);

      FhirUriFactory FhirUriFactory = new FhirUriFactory(IServiceBaseUrlMock.Object, IValidateResourceNameFactoryMock.Object);
      return FhirUriFactory;
    }
  }
}
