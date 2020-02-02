using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Api
{
  public class GlobalConfiguration
  {
    [Required(AllowEmptyStrings = false, ErrorMessage = "Must be either application/fhir+json or application/fhir+xml")]
    public string DefaultFhirFormat { get; set; }
        
  }
}
