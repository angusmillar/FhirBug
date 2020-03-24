using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirIndexReferenceSetterSupportFactory
  {
    IStu3ReferenceSetter GetStu3();
    IR4ReferenceSetter GetR4();
  }
}