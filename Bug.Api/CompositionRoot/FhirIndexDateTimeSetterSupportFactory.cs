using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirIndexDateTimeSetterSupportFactory : IFhirIndexDateTimeSetterSupportFactory
  {
    private readonly Container _container;
    public FhirIndexDateTimeSetterSupportFactory(Container container)
    {
      this._container = container;
    }
    public IStu3DateTimeSetter GetStu3()
    {
      return (IStu3DateTimeSetter)_container.GetInstance(typeof(IStu3DateTimeSetter));
    }

    public IR4DateTimeSetter GetR4()
    {
      return (IR4DateTimeSetter)_container.GetInstance(typeof(IR4DateTimeSetter));
    }
  }
}
