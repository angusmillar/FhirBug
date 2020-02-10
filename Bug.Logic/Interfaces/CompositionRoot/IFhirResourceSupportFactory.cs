
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirResourceSupportFactory
  {
    R4Fhir.IFhirResourceSupport GetR4();
    Stu3Fhir.IFhirResourceSupport GetStu3();
  }
}