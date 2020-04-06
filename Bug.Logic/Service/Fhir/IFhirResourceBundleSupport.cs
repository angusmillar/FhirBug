using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.FhirTools.Bundle;

namespace Bug.Logic.Service.Fhir
{
  public interface IFhirResourceBundleSupport
  {
    Common.FhirTools.FhirResource GetFhirResource(Common.Enums.FhirVersion fhirMajorVersion, BundleModel bundleModel);
  }
}