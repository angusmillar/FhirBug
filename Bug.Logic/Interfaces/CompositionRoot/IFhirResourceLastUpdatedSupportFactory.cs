
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirResourceLastUpdatedSupportFactory
  {
    R4Fhir.ResourceSupport.IStu3FhirResourceLastUpdatedSupport GetR4();
    Stu3Fhir.ResourceSupport.IStu3FhirResourceLastUpdatedSupport GetStu3();
  }
}