using Bug.Common.Dto.Indexing;
using Hl7.Fhir.Model;

namespace Bug.R4Fhir.Indexing.Setter.Support
{
  public interface IR4DateTimeIndexSupport
  {
    IndexDateTime? GetDateTimeIndex(Date value, int searchParameterId);
    IndexDateTime? GetDateTimeIndex(FhirDateTime value, int searchParameterId);
    IndexDateTime? GetDateTimeIndex(Instant value, int searchParameterId);
    IndexDateTime? GetDateTimeIndex(Period value, int searchParameterId);
    IndexDateTime? GetDateTimeIndex(Timing Timing, int searchParameterId);
  }
}