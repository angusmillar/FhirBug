using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;

namespace Bug.Stu3Fhir.ResourceSupport
{
  public interface IFhirResourceLastUpdatedSupport
  {
    void SetLastUpdated(DateTimeOffset dateTimeOffset, object resource);    
    DateTimeOffset? GetLastUpdated(object resource);
  }
}