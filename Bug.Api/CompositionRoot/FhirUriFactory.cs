using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.UriSupport;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirUriFactory : IFhirUriFactory
  {
    private readonly Container _container;
    public FhirUriFactory(Container container)
    {
      this._container = container;
    }    
    public IFhirUri Get()
    {
      return (IFhirUri)_container.GetInstance(typeof(IFhirUri));
    }
  }
}
