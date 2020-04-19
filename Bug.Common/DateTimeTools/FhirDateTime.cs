using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Common.DateTimeTools
{
  public class FhirDateTime
  {
    public FhirDateTime(DateTime DateTime, DateTimePrecision Precision)
    {
      this.DateTime = DateTime;
      this.Precision = Precision;
    }

    public DateTime DateTime { get; set; }
    public DateTimePrecision Precision { get; set; }
  }
}
