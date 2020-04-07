using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.ApplicationConfig;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.UriSupport;
using Bug.R4Fhir.ResourceSupport;
using Bug.Stu3Fhir.ResourceSupport;
using Moq;

namespace Bug.Test.Logic.MockSupport
{
  public static class FhirUriFactory_Factory
  {
    public static FhirUriFactory Get(string ServersBaseServiceRoot, string[] validResourceNameList)
    {
      Mock<IServiceBaseUrlConfi> IServiceBaseUrlMock = IServiceBaseUrl_MockFactory.Get(ServersBaseServiceRoot, ServersBaseServiceRoot);
      Mock<IR4ValidateResourceName> IR4ValidateResourceNameMock = IR4ValidateResourceName_MockFactory.Get(validResourceNameList);
      Mock<IStu3ValidateResourceName> IStu3ValidateResourceNameMock = IStu3ValidateResourceName_MockFactory.Get(validResourceNameList);
      Mock<IValidateResourceNameFactory> IValidateResourceNameFactoryMock = IValidateResourceNameFactory_MockFactory.Get(IStu3ValidateResourceNameMock.Object, IR4ValidateResourceNameMock.Object);

      FhirUriFactory FhirUriFactory = new FhirUriFactory(IServiceBaseUrlMock.Object, IValidateResourceNameFactoryMock.Object);
      return FhirUriFactory;
    }
  }
}
