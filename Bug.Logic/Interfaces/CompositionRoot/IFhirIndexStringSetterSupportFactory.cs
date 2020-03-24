using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirIndexStringSetterSupportFactory
  {
    IStu3StringSetter GetStu3();
    IR4StringSetter GetR4();
  }
}