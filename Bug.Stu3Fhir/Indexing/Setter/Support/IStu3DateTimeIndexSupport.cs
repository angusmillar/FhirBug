using Bug.Common.Dto.Indexing;
using Hl7.Fhir.Model;

namespace Bug.Stu3Fhir.Indexing.Setter.Support
{
  public interface IStu3DateTimeIndexSupport
  {
    IndexDateTime? GetDateTimeIndex(Date value, int searchParameterId);
    IndexDateTime? GetDateTimeIndex(FhirDateTime value, int searchParameterId);
    IndexDateTime? GetDateTimeIndex(Instant value, int searchParameterId);
    IndexDateTime? GetDateTimeIndex(Period value, int searchParameterId);
    IndexDateTime? GetDateTimeIndex(Timing Timing, int searchParameterId);
  }
}