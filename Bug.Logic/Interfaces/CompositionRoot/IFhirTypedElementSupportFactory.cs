using Bug.R4Fhir.Indexing;
using Bug.Stu3Fhir.Indexing;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirTypedElementSupportFactory
  {
    IStu3TypedElementSupport GetStu3();
    IR4TypedElementSupport GetR4();
  }
}