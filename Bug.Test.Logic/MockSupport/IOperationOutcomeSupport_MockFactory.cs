using Bug.Logic.Service.Fhir;
using Moq;

namespace Bug.Test.Logic.MockSupport
{
  public static class IOperationOutcomeSupport_MockFactory
  {
    public static Mock<IOperationOutcomeSupport> Get()
    {
      Mock<IOperationOutcomeSupport> IOperationOutcomeSupportMock = new Mock<IOperationOutcomeSupport>();
      IOperationOutcomeSupportMock.Setup(x => 
      x.GetInformation(Common.Enums.FhirVersion.Stu3, It.IsAny<string[]>()))
        .Returns(new Common.FhirTools.FhirResource(Common.Enums.FhirVersion.Stu3) { Stu3 = null });

      IOperationOutcomeSupportMock.Setup(x =>
      x.GetInformation(Common.Enums.FhirVersion.R4, It.IsAny<string[]>()))
        .Returns(new Common.FhirTools.FhirResource(Common.Enums.FhirVersion.R4) { R4 = null });

      return IOperationOutcomeSupportMock;
    }
  }
}
