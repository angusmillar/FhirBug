using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.Enums
{
  public enum FhirFormatType
  {
    [EnumInfo("json", "application/fhir+xml" )]
    json,
    [EnumInfo("xml", "application/fhir+json")]
    xml    
  };
}
