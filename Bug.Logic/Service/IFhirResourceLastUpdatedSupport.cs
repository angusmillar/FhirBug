using Bug.Common.FhirTools;
using System;

namespace Bug.Logic.Service
{
  public interface IFhirResourceLastUpdatedSupport
  {
    DateTimeOffset? GetLastUpdated(FhirResource fhirResource);
    void SetLastUpdated(FhirResource fhirResource, DateTime dateTime);
  }
}