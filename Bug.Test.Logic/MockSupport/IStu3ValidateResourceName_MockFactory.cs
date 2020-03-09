using Bug.Stu3Fhir.ResourceSupport;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Test.Logic.MockSupport
{
  public static class IStu3ValidateResourceName_MockFactory
  {
    public static Mock<IStu3ValidateResourceName> Get(string[] validResourceNameList)
    {
      Mock<IStu3ValidateResourceName> IR4ValidateResourceNameMock = new Mock<IStu3ValidateResourceName>();
      foreach (string ResourceName in validResourceNameList)
        IR4ValidateResourceNameMock.Setup(x => x.IsKnownResource(ResourceName)).Returns(true);
      return IR4ValidateResourceNameMock;
    }
  }
}
