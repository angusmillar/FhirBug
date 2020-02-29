using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Common.Enums;

namespace Bug.Logic.Service
{
  public class OperationOutcomeSupport : IOperationOutcomeSupport
  {
    private readonly IOperationOutcomeSupportFactory IOperationOutcomeSupportFactory;
    public OperationOutcomeSupport(IOperationOutcomeSupportFactory IOperationOutcomeSupportFactory)
    {
      this.IOperationOutcomeSupportFactory = IOperationOutcomeSupportFactory;
    }

    public FhirResource GetError(FhirMajorVersion fhirMajorVersion, string[] errorMessages)
    {
      switch (fhirMajorVersion)
      {
        case Common.Enums.FhirMajorVersion.Stu3:
          var Stu3Tool = IOperationOutcomeSupportFactory.GetStu3();
          var Stu3FhirResource = new FhirResource(fhirMajorVersion);
          Stu3FhirResource.Stu3 = Stu3Tool.GetError(errorMessages);
          return Stu3FhirResource;
        case Common.Enums.FhirMajorVersion.R4:
          var R4Tool = IOperationOutcomeSupportFactory.GetR4();
          var R4FhirResource = new FhirResource(fhirMajorVersion);
          R4FhirResource.R4 = R4Tool.GetError(errorMessages);
          return R4FhirResource;
        default:
          throw new FhirVersionFatalException(fhirMajorVersion);
      }
    }

    public FhirResource GetFatal(FhirMajorVersion fhirMajorVersion, string[] errorMessages)
    {
      switch (fhirMajorVersion)
      {
        case Common.Enums.FhirMajorVersion.Stu3:
          var Stu3Tool = IOperationOutcomeSupportFactory.GetStu3();
          var Stu3FhirResource = new FhirResource(fhirMajorVersion);
          Stu3FhirResource.Stu3 = Stu3Tool.GetFatal(errorMessages);
          return Stu3FhirResource;
        case Common.Enums.FhirMajorVersion.R4:
          var R4Tool = IOperationOutcomeSupportFactory.GetR4();
          var R4FhirResource = new FhirResource(fhirMajorVersion);
          R4FhirResource.R4 = R4Tool.GetFatal(errorMessages);
          return R4FhirResource;
        default:
          throw new FhirVersionFatalException(fhirMajorVersion);
      }
    }

    public FhirResource GetInformation(FhirMajorVersion fhirMajorVersion, string[] errorMessages)
    {
      switch (fhirMajorVersion)
      {
        case Common.Enums.FhirMajorVersion.Stu3:
          var Stu3Tool = IOperationOutcomeSupportFactory.GetStu3();
          var Stu3FhirResource = new FhirResource(fhirMajorVersion);
          Stu3FhirResource.Stu3 = Stu3Tool.GetInformation(errorMessages);
          return Stu3FhirResource;
        case Common.Enums.FhirMajorVersion.R4:
          var R4Tool = IOperationOutcomeSupportFactory.GetR4();
          var R4FhirResource = new FhirResource(fhirMajorVersion);
          R4FhirResource.R4 = R4Tool.GetInformation(errorMessages);
          return R4FhirResource;
        default:
          throw new FhirVersionFatalException(fhirMajorVersion);
      }
    }

    public FhirResource GetWarning(FhirMajorVersion fhirMajorVersion, string[] errorMessages)
    {
      switch (fhirMajorVersion)
      {
        case Common.Enums.FhirMajorVersion.Stu3:
          var Stu3Tool = IOperationOutcomeSupportFactory.GetStu3();
          var Stu3FhirResource = new FhirResource(fhirMajorVersion);
          Stu3FhirResource.Stu3 = Stu3Tool.GetWarning(errorMessages);
          return Stu3FhirResource;
        case Common.Enums.FhirMajorVersion.R4:
          var R4Tool = IOperationOutcomeSupportFactory.GetR4();
          var R4FhirResource = new FhirResource(fhirMajorVersion);
          R4FhirResource.R4 = R4Tool.GetWarning(errorMessages);
          return R4FhirResource;
        default:
          throw new FhirVersionFatalException(fhirMajorVersion);
      }
    }


  }
}
