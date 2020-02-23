using Bug.Logic.Interfaces.CompositionRoot;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirResourceIdSupportFactory : IFhirResourceIdSupportFactory
  {
    private readonly Container _container;
    public FhirResourceIdSupportFactory(Container container)
    {
      this._container = container;
    }
    public Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceIdSupport GetStu3()
    {
      return (Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceIdSupport)_container.GetInstance(typeof(Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceIdSupport));
    }
    public Bug.R4Fhir.ResourceSupport.IStu3FhirResourceIdSupport GetR4()
    {
      return (Bug.R4Fhir.ResourceSupport.IStu3FhirResourceIdSupport)_container.GetInstance(typeof(Bug.R4Fhir.ResourceSupport.IStu3FhirResourceIdSupport));
    }
  }
}
