using Bug.Common.FhirTools;
using System.Collections.Generic;

namespace Bug.Stu3Fhir.ResourceSupport
{
  public interface IStu3ContainedResourceDictionary
  {
    public IList<FhirContainedResource> GetContainedResourceDictionary(IFhirResourceStu3 fhirResource);
  }
}