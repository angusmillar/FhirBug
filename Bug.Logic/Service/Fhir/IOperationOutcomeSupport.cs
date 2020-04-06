using Bug.Common.Enums;
using Bug.Common.FhirTools;

namespace Bug.Logic.Service.Fhir
{
  public interface IOperationOutcomeSupport
  {
    Common.FhirTools.FhirResource GetError(Common.Enums.FhirVersion fhirMajorVersion, string[] errorMessages);
    Common.FhirTools.FhirResource GetFatal(Common.Enums.FhirVersion fhirMajorVersion, string[] errorMessages);
    Common.FhirTools.FhirResource GetInformation(Common.Enums.FhirVersion fhirMajorVersion, string[] errorMessages);
    Common.FhirTools.FhirResource GetWarning(Common.Enums.FhirVersion fhirMajorVersion, string[] errorMessages);
  }
}