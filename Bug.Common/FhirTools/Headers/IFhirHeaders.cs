using Bug.Common.Enums;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Bug.Common.FhirTools.Headers
{
  public interface IFhirHeaders
  {
    Dictionary<string, StringValues> HeaderDictionary { get; }
    PreferHandlingType PreferHandling { get; }    
  }
}