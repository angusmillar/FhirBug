using Bug.R4Fhir.Indexing.Setter;
using Bug.Stu3Fhir.Indexing.Setter;

namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IFhirIndexQuantitySetterSupportFactory
  {
    IStu3QuantitySetter GetStu3();
    IR4QuantitySetter GetR4();
  }
}