using Bug.Logic.Interfaces.CompositionRoot;
using Bug.R4Fhir.ResourceSupport;
using Bug.Stu3Fhir.ResourceSupport;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirResourceBundleSupportFactory : IFhirResourceBundleSupportFactory
  {
    private readonly Container _container;
    public FhirResourceBundleSupportFactory(Container container)
    {
      this._container = container;
    }
    public IR4BundleSupport GetR4()
    {
      return (IR4BundleSupport)_container.GetInstance(typeof(IR4BundleSupport));
    }

    public IStu3BundleSupport GetStu3()
    {
      return (IStu3BundleSupport)_container.GetInstance(typeof(IStu3BundleSupport));
    }
  }
}
