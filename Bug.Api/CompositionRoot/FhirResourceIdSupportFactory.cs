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
    public Bug.Stu3Fhir.ResourceSupport.IFhirResourceIdSupport GetStu3()
    {
      return (Bug.Stu3Fhir.ResourceSupport.IFhirResourceIdSupport)_container.GetInstance(typeof(Bug.Stu3Fhir.ResourceSupport.IFhirResourceIdSupport));
    }
    public Bug.R4Fhir.ResourceSupport.IFhirResourceIdSupport GetR4()
    {
      return (Bug.R4Fhir.ResourceSupport.IFhirResourceIdSupport)_container.GetInstance(typeof(Bug.R4Fhir.ResourceSupport.IFhirResourceIdSupport));
    }
  }
}
