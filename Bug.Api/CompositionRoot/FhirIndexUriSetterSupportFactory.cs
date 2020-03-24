using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirIndexUriSetterSupportFactory : IFhirIndexUriSetterSupportFactory
  {
    private readonly Container _container;
    public FhirIndexUriSetterSupportFactory(Container container)
    {
      this._container = container;
    }
    public IStu3UriSetter GetStu3()
    {
      return (IStu3UriSetter)_container.GetInstance(typeof(IStu3UriSetter));
    }

    public IR4UriSetter GetR4()
    {
      return (IR4UriSetter)_container.GetInstance(typeof(IR4UriSetter));
    }
  }
}
