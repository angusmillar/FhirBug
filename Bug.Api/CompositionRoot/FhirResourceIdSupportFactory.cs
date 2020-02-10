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
    public Bug.Stu3Fhir.IFhirResourceIdSupport GetStu3()
    {
      return (Bug.Stu3Fhir.IFhirResourceIdSupport)_container.GetInstance(typeof(Bug.Stu3Fhir.IFhirResourceIdSupport));
    }
    public Bug.R4Fhir.IFhirResourceIdSupport GetR4()
    {
      return (Bug.R4Fhir.IFhirResourceIdSupport)_container.GetInstance(typeof(Bug.R4Fhir.IFhirResourceIdSupport));
    }
  }
}
