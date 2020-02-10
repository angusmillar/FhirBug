using Bug.Logic.Interfaces.CompositionRoot;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirResourceSupportFactory : IFhirResourceSupportFactory
  {
    private readonly Container _container;
    public FhirResourceSupportFactory(Container container)
    {
      this._container = container;
    }
    public Bug.Stu3Fhir.IFhirResourceSupport GetStu3()
    {
      return (Bug.Stu3Fhir.IFhirResourceSupport)_container.GetInstance(typeof(Bug.Stu3Fhir.IFhirResourceSupport));
    }
    public Bug.R4Fhir.IFhirResourceSupport GetR4()
    {
      return (Bug.R4Fhir.IFhirResourceSupport)_container.GetInstance(typeof(Bug.R4Fhir.IFhirResourceSupport));
    }
  }
}
