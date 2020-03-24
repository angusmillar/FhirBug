using Bug.Logic.Interfaces.CompositionRoot;
using SimpleInjector;

namespace Bug.Api.CompositionRoot
{
  public class FhirResourceLastUpdatedSupportFactory : IFhirResourceLastUpdatedSupportFactory
  {
    private readonly Container _container;
    public FhirResourceLastUpdatedSupportFactory(Container container)
    {
      this._container = container;
    }
    public Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceLastUpdatedSupport GetStu3()
    {
      return (Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceLastUpdatedSupport)_container.GetInstance(typeof(Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceLastUpdatedSupport));
    }
    public Bug.R4Fhir.ResourceSupport.IR4FhirResourceLastUpdatedSupport GetR4()
    {
      return (Bug.R4Fhir.ResourceSupport.IR4FhirResourceLastUpdatedSupport)_container.GetInstance(typeof(Bug.R4Fhir.ResourceSupport.IR4FhirResourceLastUpdatedSupport));
    }
  }
}
