using Bug.Common.FhirTools;
using Bug.Common.FhirTools.Bundle;

namespace Bug.Stu3Fhir.ResourceSupport
{
  public interface IStu3BundleSupport
  {
    FhirResource GetFhirResource(BundleModel bundleModel);
  }
}