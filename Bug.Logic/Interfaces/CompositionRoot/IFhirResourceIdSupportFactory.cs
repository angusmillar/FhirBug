
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirResourceIdSupportFactory
  {
    R4Fhir.IFhirResourceIdSupport GetR4();
    Stu3Fhir.IFhirResourceIdSupport GetStu3();
  }
}