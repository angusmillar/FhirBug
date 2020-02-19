using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bug.Common.FhirTools
{
  public class OidSupport
  {
    public const string PATTERN = @"urn:oid:[0-2](\.(0|[1-9][0-9]*))+";
    public static bool IsValidValue(string value)
    {
      return Regex.IsMatch(value, "^" + OidSupport.PATTERN + "$", RegexOptions.Singleline);
    }
  }
}
