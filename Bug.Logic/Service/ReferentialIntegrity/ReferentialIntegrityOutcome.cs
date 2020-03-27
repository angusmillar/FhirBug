using Bug.Common.FhirTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.ReferentialIntegrity
{
  public class ReferentialIntegrityOutcome
  {
    public ReferentialIntegrityOutcome()
    {
      this.IsError = false;
    }

    public FhirResource? FhirResource { get; set; }
    public bool IsError { get; set; }

  }
}
