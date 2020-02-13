using Hl7.Fhir.Model;

namespace Bug.Stu3Fhir.OperationOutCome
{
  public interface IStu3OperationOutComeSupport
  {
    OperationOutcome GetError(string[] errorMessageList);
    OperationOutcome GetFatal(string[] errorMessageList);
    OperationOutcome GetInformation(string[] errorMessageList);
    OperationOutcome GetWarning(string[] errorMessageList);
  }
}