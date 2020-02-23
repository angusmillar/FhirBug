using Bug.Common.FhirTools;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;

namespace Bug.Stu3Fhir.ResourceSupport
{
  public interface IStu3FhirResourceIdSupport
  {
    string GetFhirId(IFhirResourceStu3 fhirResource);
    void SetFhirId(string id, IFhirResourceStu3 fhirResource);

  }
}