using Bug.Common.Enums;
using Bug.Common.FhirTools;

namespace Bug.Logic.Service
{
  public interface IOperationOutcomeSupport
  {
    FhirResource GetError(FhirVersion fhirMajorVersion, string[] errorMessages);
    FhirResource GetFatal(FhirVersion fhirMajorVersion, string[] errorMessages);
    FhirResource GetInformation(FhirVersion fhirMajorVersion, string[] errorMessages);
    FhirResource GetWarning(FhirVersion fhirMajorVersion, string[] errorMessages);
  }
}