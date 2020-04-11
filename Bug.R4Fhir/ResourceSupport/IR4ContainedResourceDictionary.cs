using Bug.Common.FhirTools;
using System.Collections.Generic;

namespace Bug.R4Fhir.ResourceSupport
{
  public interface IR4ContainedResourceDictionary
  {
    IList<FhirContainedResource> GetContainedResourceDictionary(IFhirResourceR4 fhirResource);
  }
}