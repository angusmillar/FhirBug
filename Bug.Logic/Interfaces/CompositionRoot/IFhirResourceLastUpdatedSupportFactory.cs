
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirResourceLastUpdatedSupportFactory
  {
    R4Fhir.ResourceSupport.IFhirResourceLastUpdatedSupport GetR4();
    Stu3Fhir.ResourceSupport.IFhirResourceLastUpdatedSupport GetStu3();
  }
}