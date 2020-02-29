using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.UriSupport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public interface IFhirUriValidator
  {
    bool IsValid(string Uri, FhirMajorVersion fhirMajorVersion, out IFhirUri? fhirUri, out FhirResource? operationOutcome);
  }
}
