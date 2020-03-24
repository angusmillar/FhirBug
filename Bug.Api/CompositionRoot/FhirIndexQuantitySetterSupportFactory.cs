using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirIndexQuantitySetterSupportFactory : IFhirIndexQuantitySetterSupportFactory
  {
    private readonly Container _container;
    public FhirIndexQuantitySetterSupportFactory(Container container)
    {
      this._container = container;
    }
    public IStu3QuantitySetter GetStu3()
    {
      return (IStu3QuantitySetter)_container.GetInstance(typeof(IStu3QuantitySetter));
    }

    public IR4QuantitySetter GetR4()
    {
      return (IR4QuantitySetter)_container.GetInstance(typeof(IR4QuantitySetter));
    }
  }
}
