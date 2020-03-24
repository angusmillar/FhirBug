using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirIndexNumberSetterSupportFactory : IFhirIndexNumberSetterSupportFactory
  {
    private readonly Container _container;
    public FhirIndexNumberSetterSupportFactory(Container container)
    {
      this._container = container;
    }
    public IStu3NumberSetter GetStu3()
    {
      return (IStu3NumberSetter)_container.GetInstance(typeof(IStu3NumberSetter));
    }

    public IR4NumberSetter GetR4()
    {
      return (IR4NumberSetter)_container.GetInstance(typeof(IR4NumberSetter));
    }
  }
}
