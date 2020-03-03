using Bug.Common.FhirTools;
using Bug.Common.FhirTools.Bundle;

namespace Bug.R4Fhir.ResourceSupport
{
  public interface IR4BundleSupport
  {
    FhirResource GetFhirResource(BundleModel bundleModel);
  }
}