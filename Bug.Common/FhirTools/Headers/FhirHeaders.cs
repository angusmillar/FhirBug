using Bug.Common.Enums;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bug.Common.FhirTools.Headers
{
  public class FhirHeaders : IFhirHeaders
  {
    public const string TermPrefer = "prefer";
    public Dictionary<string, StringValues> HeaderDictionary { get; set; }
    public PreferHandlingType PreferHandling { get; set; }   
    public FhirHeaders()
    {
      this.PreferHandling = PreferHandlingType.Lenient; //This is the default as per FHIR spec
      this.HeaderDictionary = new Dictionary<string, StringValues>();
    }

    public void Parse(Dictionary<string, StringValues> headerDictionary)
    {
      this.HeaderDictionary = headerDictionary;
      foreach (var Header in headerDictionary)
      {       
        ParsePrefer(Header);
      }

    }

    private bool ParsePrefer(KeyValuePair<string, StringValues> Header)
    {
      string TermHandling = "handling";
      if (Header.Key.Trim().Equals(TermPrefer, StringComparison.OrdinalIgnoreCase))
      {
        //We should not get many but if we do we will just use the last one
        foreach (string Value in Header.Value.Reverse())
        {
          if (Value.Equals($"{TermHandling}={PreferHandlingType.Strict.GetCode()}", StringComparison.OrdinalIgnoreCase))
          {
            this.PreferHandling = PreferHandlingType.Strict;
            return true;
          }
          else if (Value.Equals($"{TermHandling}={PreferHandlingType.Lenient.GetCode()}", StringComparison.OrdinalIgnoreCase))
          {
            this.PreferHandling = PreferHandlingType.Lenient;
            return true;
          }
        }
      }
      return false;
    }
  }
}
