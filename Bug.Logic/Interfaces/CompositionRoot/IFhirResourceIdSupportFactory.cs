
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirResourceIdSupportFactory
  {
    R4Fhir.ResourceSupport.IStu3FhirResourceIdSupport GetR4();
    Stu3Fhir.ResourceSupport.IStu3FhirResourceIdSupport GetStu3();
  }
}