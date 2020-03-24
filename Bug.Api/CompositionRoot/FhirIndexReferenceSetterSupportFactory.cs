using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirIndexReferenceSetterSupportFactory : IFhirIndexReferenceSetterSupportFactory
  {
    private readonly Container _container;
    public FhirIndexReferenceSetterSupportFactory(Container container)
    {
      this._container = container;
    }
    public IStu3ReferenceSetter GetStu3()
    {
      return (IStu3ReferenceSetter)_container.GetInstance(typeof(IStu3ReferenceSetter));
    }

    public IR4ReferenceSetter GetR4()
    {
      return (IR4ReferenceSetter)_container.GetInstance(typeof(IR4ReferenceSetter));
    }
  }
}
