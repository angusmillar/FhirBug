using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;
namespace Bug.Logic.Command.FhirApi
{
  public abstract class FhirApiCommand
  {
    public Uri RequestUri { get; set; }    
    public FhirMajorVersion FhirMajorVersion { get; set; }
    public string RequestHeaders { get; set; }

  }
}
