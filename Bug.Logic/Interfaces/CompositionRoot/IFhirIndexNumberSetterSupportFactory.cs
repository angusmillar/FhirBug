using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirIndexNumberSetterSupportFactory
  {
    IStu3NumberSetter GetStu3();
    IR4NumberSetter GetR4();
  }
}