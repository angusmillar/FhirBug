using Bug.Common.FhirTools;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;

namespace Bug.R4Fhir.ResourceSupport
{
  public interface IStu3FhirResourceIdSupport
  {
    string GetFhirId(IFhirResourceR4 fhirResource);
    string SetFhirId(string id, IFhirResourceR4 fhirResource);

  }
}