extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;

namespace Bug.Common.FhirTools
{
  public interface IFhirResourceStu3
  {
    Stu3Model.Resource Stu3 { get; set; }
  }
}