using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;

namespace Bug.R4Fhir.ResourceSupport
{
  public interface IFhirResourceIdSupport
  {
    string GetFhirId(object resource);
    string SetFhirId(string id, object resource);

  }
}