
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirResourceVersionSupportFactory
  {
    R4Fhir.ResourceSupport.IFhirResourceVersionSupport GetR4();
    Stu3Fhir.ResourceSupport.IFhirResourceVersionSupport GetStu3();
  }
}