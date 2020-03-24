using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirIndexDateTimeSetterSupportFactory
  {
    IStu3DateTimeSetter GetStu3();
    IR4DateTimeSetter GetR4();
  }
}