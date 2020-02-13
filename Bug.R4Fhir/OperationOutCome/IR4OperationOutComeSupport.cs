using Hl7.Fhir.Model;

namespace Bug.R4Fhir.OperationOutCome
{
  public interface IR4OperationOutComeSupport
  {
    OperationOutcome GetError(string[] errorMessageList);
    OperationOutcome GetFatal(string[] errorMessageList);
    OperationOutcome GetInformation(string[] errorMessageList);
    OperationOutcome GetWarning(string[] errorMessageList);
  }
}