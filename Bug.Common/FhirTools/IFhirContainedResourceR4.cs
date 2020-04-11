extern alias R4;
using R4Model = R4.Hl7.Fhir.Model;
using Bug.Common.Enums;

namespace Bug.Common.FhirTools
{
  public interface IFhirContainedResourceR4
  {
    string ResourceId { get;  }
    FhirVersion FhirVersion { get; }
    R4Model.Resource R4 { get; set; }
    ResourceType ResourceType { get; }
  }
}