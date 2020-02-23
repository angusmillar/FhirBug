extern alias R4;
using R4Model = R4.Hl7.Fhir.Model;

namespace Bug.Common.FhirTools
{
  public interface IFhirResourceR4
  {
    R4Model.Resource R4 { get; set; }
  }
}