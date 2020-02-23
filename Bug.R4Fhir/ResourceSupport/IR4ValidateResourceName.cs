using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace Bug.R4Fhir.ResourceSupport
{
  public interface IR4ValidateResourceName
  {
    bool IsKnownResource(string ResourceName);    
  }
}