
namespace Bug.Logic.Interfaces.CompositionRoot
{
  public interface IValidateResourceNameFactory
  {
    R4Fhir.ResourceSupport.IValidateResourceName GetR4();
    Stu3Fhir.ResourceSupport.IValidateResourceName GetStu3();
  }
}