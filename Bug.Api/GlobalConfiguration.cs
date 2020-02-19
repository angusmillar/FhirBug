using Bug.Common.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Api
{
  public class GlobalConfiguration
  {
    public bool ConfigurationIsValid { get; private set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Must be either application/fhir+json or application/fhir+xml")]
    public string DefaultFhirFormat { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Must be set to the Service Base URL e.g : https://AcmeHealth.com/something/someotherthing")]
    public string ServiceBaseURL { get; set; }

    public List<string> ValidateConfig()
    {
      var ErrorList = new List<string>();
      if (DefaultFhirFormat.Equals("application/fhir+json", StringComparison.CurrentCultureIgnoreCase) || DefaultFhirFormat.Equals("application/fhir+xml", StringComparison.CurrentCultureIgnoreCase))
      {
        this.DefaultFhirFormat = this.DefaultFhirFormat.ToLower(CultureInfo.CurrentCulture);
      }
      else
      {
        ErrorList.Add("The setting DefaultFhirFormat must equal either application/fhir+json or application/fhir+xml");
      }

      if (Uri.TryCreate(this.ServiceBaseURL, UriKind.Absolute, out Uri TempUri))
      {
        if (this.ServiceBaseURL.EndsWith(EndpointPath.Stu3Fhir, StringComparison.CurrentCultureIgnoreCase) || this.ServiceBaseURL.EndsWith(EndpointPath.R4Fhir, StringComparison.CurrentCultureIgnoreCase))
        {
          ErrorList.Add("The setting ServiceBaseURL must no end in either 'stu3/fhir' or 'r4/fhir', the server will append these segments appropriately for you.");
        }
      }
      else
      {
        ErrorList.Add("The setting ServiceBaseURL must be and absolute Uri.");
      }

      if (ErrorList.Count > 0)
        ConfigurationIsValid = false;
      else
        ConfigurationIsValid = true;

      return ErrorList;
    }
  }
}
