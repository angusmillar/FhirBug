using System;
using System.Net;

namespace Bug.Common.Exceptions
{
  public abstract class FhirException : ApplicationException
  {
    public HttpStatusCode HttpStatusCode { get; }
    public string[] MessageList { get; }

    public FhirException(HttpStatusCode httpStatusCode, string message)
      : base(message)
    {
      HttpStatusCode = httpStatusCode;
      MessageList = new string[] { message };

    }
    public FhirException(HttpStatusCode httpStatusCode, string message, Exception innerException)
      : base(message, innerException)
    {
      HttpStatusCode = httpStatusCode;
      MessageList = new string[] { message };
    }
    public FhirException(HttpStatusCode httpStatusCode, string[] messageList)
  : base(string.Join(' ', messageList))
    {
      HttpStatusCode = httpStatusCode;
      MessageList = messageList;
    }
    public FhirException(HttpStatusCode httpStatusCode, string[] messageList, Exception innerException)
      : base(string.Join(' ', messageList), innerException)
    {
      HttpStatusCode = httpStatusCode;
      MessageList = messageList;
    }
  }
}
