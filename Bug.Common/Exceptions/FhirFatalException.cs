using System;
using System.Net;

namespace Bug.Common.Exceptions
{
  public class FhirFatalException : FhirException
  {
    public FhirFatalException(HttpStatusCode httpStatusCode, string message)
      : base(httpStatusCode, message) { }
    public FhirFatalException(HttpStatusCode httpStatusCode, string message, Exception innerException)
      : base(httpStatusCode, message, innerException) { }
    public FhirFatalException(HttpStatusCode httpStatusCode, string[] messageList)
      : base(httpStatusCode, messageList) { }
    public FhirFatalException(HttpStatusCode httpStatusCode, string[] messageList, Exception innerException)
      : base(httpStatusCode, messageList, innerException) { }
  }
}
