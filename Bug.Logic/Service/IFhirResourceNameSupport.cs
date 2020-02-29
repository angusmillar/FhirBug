using Bug.Common.FhirTools;
using System;

namespace Bug.Logic.Service
{
  public interface IFhirResourceNameSupport
  {
    string GetName(FhirResource fhirResource);
  }
}