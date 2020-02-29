
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirResourceNameSupportFactory
  {
    R4Fhir.ResourceSupport.IR4FhirResourceNameSupport GetR4();
    Stu3Fhir.ResourceSupport.IStu3FhirResourceNameSupport GetStu3();
  }
}