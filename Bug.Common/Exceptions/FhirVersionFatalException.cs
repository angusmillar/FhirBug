using Bug.Common.Enums;
using System;
using System.Net;

namespace Bug.Common.Exceptions
{
  public class FhirVersionFatalException : FhirException
  {
    public FhirVersionFatalException(FhirMajorVersion fhirMajorVersion)
    {
      HttpStatusCode = HttpStatusCode.InternalServerError;
      MessageList = new string[] { $"A FhirMajorVersion was not handled by the server. The FhirMajorVersion value literal was: {fhirMajorVersion.GetLiteral()}" };
    }    
  }
}
