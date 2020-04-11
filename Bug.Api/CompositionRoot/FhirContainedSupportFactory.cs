using Bug.Logic.Interfaces.CompositionRoot;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirContainedSupportFactory : IFhirContainedSupportFactory
  {
    private readonly Container _container;
    public FhirContainedSupportFactory(Container container)
    {
      this._container = container;
    }
    public Bug.Stu3Fhir.ResourceSupport.IStu3ContainedResourceDictionary GetStu3()
    {
      return (Bug.Stu3Fhir.ResourceSupport.IStu3ContainedResourceDictionary)_container.GetInstance(typeof(Bug.Stu3Fhir.ResourceSupport.IStu3ContainedResourceDictionary));
    }
    public Bug.R4Fhir.ResourceSupport.IR4ContainedResourceDictionary GetR4()
    {
      return (Bug.R4Fhir.ResourceSupport.IR4ContainedResourceDictionary)_container.GetInstance(typeof(Bug.R4Fhir.ResourceSupport.IR4ContainedResourceDictionary));
    }
  }
}
