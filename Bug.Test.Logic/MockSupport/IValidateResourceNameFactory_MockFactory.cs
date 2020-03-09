using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.ResourceSupport;
using Bug.Stu3Fhir.ResourceSupport;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Test.Logic.MockSupport
{
  public static class IValidateResourceNameFactory_MockFactory
  {
    public static Mock<IValidateResourceNameFactory> Get(IStu3ValidateResourceName IStu3ValidateResourceName, IR4ValidateResourceName IR4ValidateResourceName)
    {
      Mock<IValidateResourceNameFactory> IValidateResourceNameFactoryMock = new Mock<IValidateResourceNameFactory>();
      IValidateResourceNameFactoryMock.Setup(x => x.GetStu3()).Returns(IStu3ValidateResourceName);
      IValidateResourceNameFactoryMock.Setup(x => x.GetR4()).Returns(IR4ValidateResourceName);
      return IValidateResourceNameFactoryMock;
    }
  }
}
