using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirIndexTokenSetterSupportFactory
  {
    IStu3TokenSetter GetStu3();
    IR4TokenSetter GetR4();
  }
}