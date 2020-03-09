using Bug.R4Fhir.ResourceSupport;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Test.Logic.MockSupport
{
  public static class IR4ValidateResourceName_MockFactory
  {
    public static Mock<IR4ValidateResourceName> Get(string[] validResourceNameList)
    {
      Mock<IR4ValidateResourceName> IR4ValidateResourceNameMock = new Mock<IR4ValidateResourceName>();
      foreach (string ResourceName in validResourceNameList)
        IR4ValidateResourceNameMock.Setup(x => x.IsKnownResource(ResourceName)).Returns(true);
      return IR4ValidateResourceNameMock;
    }
  }
}
