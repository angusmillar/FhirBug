using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.Indexing;
using Bug.Stu3Fhir.Indexing;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirTypedElementSupportFactory : IFhirTypedElementSupportFactory
  {
    private readonly Container _container;
    public FhirTypedElementSupportFactory(Container container)
    {
      this._container = container;
    }
    public IStu3TypedElementSupport GetStu3()
    {
      return (IStu3TypedElementSupport)_container.GetInstance(typeof(IStu3TypedElementSupport));
    }
    public IR4TypedElementSupport GetR4()
    {
      return (IR4TypedElementSupport)_container.GetInstance(typeof(IR4TypedElementSupport));
    }
    
  }
}
