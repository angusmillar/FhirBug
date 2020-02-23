
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IValidateResourceNameFactory
  {
    R4Fhir.ResourceSupport.IR4ValidateResourceName GetR4();
    Stu3Fhir.ResourceSupport.IStu3ValidateResourceName GetStu3();
  }
}