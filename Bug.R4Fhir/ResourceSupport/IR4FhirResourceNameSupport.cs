using Bug.Common.FhirTools;

namespace Bug.R4Fhir.ResourceSupport
{
  public interface IR4FhirResourceNameSupport
  {
    string GetName(IFhirResourceR4 fhirResource);    
  }
}