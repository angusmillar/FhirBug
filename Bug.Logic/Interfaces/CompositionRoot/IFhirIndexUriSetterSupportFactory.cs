using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirIndexUriSetterSupportFactory
  {
    IStu3UriSetter GetStu3();
    IR4UriSetter GetR4();
  }
}