extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Bug.Common.Enums;

namespace Bug.Common.FhirTools
{
  public interface IFhirContainedResourceStu3
  {
    string ResourceId { get; }
    FhirVersion FhirVersion { get; }
    ResourceType ResourceType { get; }
    Stu3Model.Resource Stu3 { get; set; }
  }
}