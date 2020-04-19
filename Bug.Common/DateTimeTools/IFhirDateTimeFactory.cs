using Hl7.Fhir.Model.Primitives;

namespace Bug.Common.DateTimeTools
{
  public interface IFhirDateTimeFactory
  {
    bool TryParse(PartialDateTime partialDateTime, out FhirDateTime? fhirDateTime, out string? errorMessage);
    bool TryParse(string fhirDateTimeString, out FhirDateTime? fhirDateTime, out string? errorMessage);
  }
}