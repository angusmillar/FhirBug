using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace Bug.Stu3Fhir.ResourceSupport
{
  public interface IStu3ValidateResourceName
  {
    bool IsKnownResource(string ResourceName); 
  }
}