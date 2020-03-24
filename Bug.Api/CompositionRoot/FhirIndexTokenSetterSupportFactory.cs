using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirIndexTokenSetterSupportFactory : IFhirIndexTokenSetterSupportFactory
  {
    private readonly Container _container;
    public FhirIndexTokenSetterSupportFactory(Container container)
    {
      this._container = container;
    }
    public IStu3TokenSetter GetStu3()
    {
      return (IStu3TokenSetter)_container.GetInstance(typeof(IStu3TokenSetter));
    }

    public IR4TokenSetter GetR4()
    {
      return (IR4TokenSetter)_container.GetInstance(typeof(IR4TokenSetter));
    }
  }
}
