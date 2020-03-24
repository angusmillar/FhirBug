using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirIndexStringSetterSupportFactory : IFhirIndexStringSetterSupportFactory
  {
    private readonly Container _container;
    public FhirIndexStringSetterSupportFactory(Container container)
    {
      this._container = container;
    }
    public IStu3StringSetter GetStu3()
    {
      return (IStu3StringSetter)_container.GetInstance(typeof(IStu3StringSetter));
    }

    public IR4StringSetter GetR4()
    {
      return (IR4StringSetter)_container.GetInstance(typeof(IR4StringSetter));
    }
  }
}
