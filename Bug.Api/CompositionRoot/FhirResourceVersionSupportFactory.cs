using Bug.Logic.Interfaces.CompositionRoot;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirResourceVersionSupportFactory : IFhirResourceVersionSupportFactory
  {
    private readonly Container _container;
    public FhirResourceVersionSupportFactory(Container container)
    {
      this._container = container;
    }
    public Bug.Stu3Fhir.ResourceSupport.IFhirResourceVersionSupport GetStu3()
    {
      return (Bug.Stu3Fhir.ResourceSupport.IFhirResourceVersionSupport)_container.GetInstance(typeof(Bug.Stu3Fhir.ResourceSupport.IFhirResourceVersionSupport));
    }
    public Bug.R4Fhir.ResourceSupport.IFhirResourceVersionSupport GetR4()
    {
      return (Bug.R4Fhir.ResourceSupport.IFhirResourceVersionSupport)_container.GetInstance(typeof(Bug.R4Fhir.ResourceSupport.IFhirResourceVersionSupport));
    }
  }
}
