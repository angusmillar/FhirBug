using Bug.Common.FhirTools;
using System;

namespace Bug.R4Fhir.ResourceSupport
{
  public interface IR4FhirResourceLastUpdatedSupport
  {
    DateTimeOffset? GetLastUpdated(IFhirResourceR4 fhirResource);
    void SetLastUpdated(DateTimeOffset dateTimeOffset, IFhirResourceR4 fhirResource);
  }
}