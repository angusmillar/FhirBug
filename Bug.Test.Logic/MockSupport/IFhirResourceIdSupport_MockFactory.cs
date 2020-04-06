using Bug.Common.FhirTools;
using Bug.Logic.Service.Fhir;
using Moq;

namespace Bug.Test.Logic.MockSupport
{
  public static class IFhirResourceIdSupport_MockFactory
  {
    public static Mock<IFhirResourceIdSupport> Get(string resourceId)
    {
      Mock<IFhirResourceIdSupport> IFhirResourceIdSupportMock = new Mock<IFhirResourceIdSupport>();
      IFhirResourceIdSupportMock.Setup(x => x.GetResourceId(It.IsAny<FhirResource>())).Returns(resourceId);
      IFhirResourceIdSupportMock.Setup(x => x.SetResourceId(It.IsAny<FhirResource>(), It.IsAny<string>()));
      return IFhirResourceIdSupportMock;
    }
  }
}
