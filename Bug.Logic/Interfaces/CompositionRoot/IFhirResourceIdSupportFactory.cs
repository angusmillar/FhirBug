
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirResourceIdSupportFactory
  {
    R4Fhir.ResourceSupport.IFhirResourceIdSupport GetR4();
    Stu3Fhir.ResourceSupport.IFhirResourceIdSupport GetStu3();
  }
}