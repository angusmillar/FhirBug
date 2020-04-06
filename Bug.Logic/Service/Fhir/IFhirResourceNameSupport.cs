using Bug.Common.FhirTools;
using System;

namespace Bug.Logic.Service.Fhir
{
  public interface IFhirResourceNameSupport
  {
    string GetName(FhirResource fhirResource);
  }
}