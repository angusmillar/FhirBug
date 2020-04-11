using Bug.Common.FhirTools;
using System.Collections.Generic;

namespace Bug.Logic.Service.Fhir
{
  public interface IFhirResourceContainedSupport
  {
    IList<FhirContainedResource> GetContainedResourceDictionary(FhirResource fhirResource);
  }
}