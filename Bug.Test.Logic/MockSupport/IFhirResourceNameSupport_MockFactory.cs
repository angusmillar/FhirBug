using Bug.Common.FhirTools;
using Bug.Logic.Service.Fhir;
using Moq;

namespace Bug.Test.Logic.MockSupport
{
  public static class IFhirResourceNameSupport_MockFactory
  {
    public static Mock<IFhirResourceNameSupport> Get(string resourceName)
    {
      Mock<IFhirResourceNameSupport> IFhirResourceNameSupportMock = new Mock<IFhirResourceNameSupport>();
      IFhirResourceNameSupportMock.Setup(x => x.GetName(It.IsAny<FhirResource>())).Returns(resourceName);      
      return IFhirResourceNameSupportMock;
    }
  }
}
