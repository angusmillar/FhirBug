using System;
using System.Collections.Generic;
using System.Net;

namespace Bug.Common.Exceptions
{
  public class FhirInfoException : FhirException
  {
    public FhirInfoException(HttpStatusCode httpStatusCode, string message)
      : base(httpStatusCode, message) { }
    public FhirInfoException(HttpStatusCode httpStatusCode, string message, Exception innerException)
      : base(httpStatusCode, message, innerException) { }
    public FhirInfoException(HttpStatusCode httpStatusCode, string[] messageList)
      : base(httpStatusCode, messageList) { }
    public FhirInfoException(HttpStatusCode httpStatusCode, string[] messageList, Exception innerException)
      : base(httpStatusCode, messageList, innerException) { }
  }
}
