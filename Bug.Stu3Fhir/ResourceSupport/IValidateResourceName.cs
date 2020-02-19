using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace Bug.Stu3Fhir.ResourceSupport
{
  public interface IValidateResourceName
  {
    bool IsKnownResource(string ResourceName); 
  }
}