
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirContainedSupportFactory
  {
    R4Fhir.ResourceSupport.IR4ContainedResourceDictionary GetR4();
    Stu3Fhir.ResourceSupport.IStu3ContainedResourceDictionary GetStu3();
  }
}