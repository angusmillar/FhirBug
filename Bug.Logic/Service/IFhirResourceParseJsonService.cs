using Bug.Common.Enums;
using Bug.Common.FhirTools;

namespace Bug.Logic.Service
{
  public interface IFhirResourceParseJsonService
  {
    FhirResource ParseJson(FhirVersion fhirMajorVersion, string jsonResource);
  }
}