using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.ResourceSupport;
using Bug.Stu3Fhir.ResourceSupport;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirResourceNameSupportFactory : IFhirResourceNameSupportFactory
  {
    private readonly Container _container;
    public FhirResourceNameSupportFactory(Container container)
    {
      this._container = container;
    }
    public IR4FhirResourceNameSupport GetR4()
    {
      return (IR4FhirResourceNameSupport)_container.GetInstance(typeof(IR4FhirResourceNameSupport));
    }

    public IStu3FhirResourceNameSupport GetStu3()
    {
      return (IStu3FhirResourceNameSupport)_container.GetInstance(typeof(IStu3FhirResourceNameSupport));
    }
  }
}
