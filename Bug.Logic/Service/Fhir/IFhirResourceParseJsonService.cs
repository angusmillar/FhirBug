using Bug.Common.Enums;
using Bug.Common.FhirTools;

namespace Bug.Logic.Service.Fhir
{
  public interface IFhirResourceParseJsonService
  {
    Common.FhirTools.FhirResource ParseJson(Common.Enums.FhirVersion fhirMajorVersion, string jsonResource);
  }
}