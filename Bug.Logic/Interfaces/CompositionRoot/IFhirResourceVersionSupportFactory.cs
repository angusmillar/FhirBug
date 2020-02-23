
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirResourceVersionSupportFactory
  {
    R4Fhir.ResourceSupport.IR4FhirResourceVersionSupport GetR4();
    Stu3Fhir.ResourceSupport.IStu3FhirResourceVersionSupport GetStu3();
  }
}