using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.UriSupport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public class FhirUriValidator : IFhirUriValidator
  {
    private readonly IFhirUriFactory IFhirUriFactory;
    private readonly IOperationOutcomeSupport IOperationOutcomeSupport;
    public FhirUriValidator(IFhirUriFactory IFhirUriFactory, IOperationOutcomeSupport IOperationOutcomeSupport)
    {
      this.IFhirUriFactory = IFhirUriFactory;
      this.IOperationOutcomeSupport = IOperationOutcomeSupport;
    }
    public bool IsValid(string Uri, FhirMajorVersion fhirMajorVersion, out IFhirUri? fhirUri, out FhirResource? operationOutcome)
    {
      fhirUri = IFhirUriFactory.Get();
      if (fhirUri.TryParse(Uri, fhirMajorVersion, out fhirUri))
      {
        operationOutcome = null;
        return true;
      }
      operationOutcome = IOperationOutcomeSupport.GetError(fhirMajorVersion, new string[] { fhirUri.ParseErrorMessage });
      fhirUri = null;
      return false;
    }
  }
}
