using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;
namespace Bug.Logic
{
  public class UpdateResourceCommand
  {
    public Uri RequestUri { get; set; }
    public object Resource { get; set; }
    public FhirMajorVersion FhirMajorVersion { get; set; }

  }
}
