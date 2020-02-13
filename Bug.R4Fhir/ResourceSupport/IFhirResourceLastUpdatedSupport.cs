using System;

namespace Bug.R4Fhir.ResourceSupport
{
  public interface IFhirResourceLastUpdatedSupport
  {
    DateTimeOffset? GetLastUpdated(object resource);
    void SetLastUpdated(DateTimeOffset dateTimeOffset, object resource);
  }
}