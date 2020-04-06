using Bug.Logic.Service.Fhir;
using Moq;

namespace Bug.Test.Logic.MockSupport
{
  public static class IKnownResource_MockFactory
  {
    public static Mock<IKnownResource> Get()
    {
      Mock<IKnownResource> IKnownResourceMock = new Mock<IKnownResource>();
      IKnownResourceMock.Setup(x => 
      x.IsKnownResource(Common.Enums.FhirVersion.Stu3, It.IsAny<string>()))
        .Returns(true);

      IKnownResourceMock.Setup(x =>
      x.IsKnownResource(Common.Enums.FhirVersion.R4, It.IsAny<string>()))
        .Returns(true);

      return IKnownResourceMock;
    }
  }
}
