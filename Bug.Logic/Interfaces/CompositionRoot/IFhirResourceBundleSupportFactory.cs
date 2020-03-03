using Bug.R4Fhir.ResourceSupport;
using Bug.Stu3Fhir.ResourceSupport;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirResourceBundleSupportFactory
  {
    IR4BundleSupport GetR4();
    IStu3BundleSupport GetStu3();
  }
}