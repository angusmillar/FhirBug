using System;
using System.Collections.Generic;
using System.Text;
using Bug.Common.Enums;

namespace Bug.Stu3Fhir.Enums
{
  public class SummaryTypeMap
  {
    private Dictionary<Bug.Common.Enums.SummaryType, Hl7.Fhir.Rest.SummaryType> Map;
    public SummaryTypeMap()
    {
      Map = new Dictionary<Common.Enums.SummaryType, Hl7.Fhir.Rest.SummaryType>();
      Map.Add(SummaryType.Count, Hl7.Fhir.Rest.SummaryType.Count);
      Map.Add(SummaryType.Data, Hl7.Fhir.Rest.SummaryType.Data);
      Map.Add(SummaryType.False, Hl7.Fhir.Rest.SummaryType.False);
      Map.Add(SummaryType.Text, Hl7.Fhir.Rest.SummaryType.Text);
      Map.Add(SummaryType.True, Hl7.Fhir.Rest.SummaryType.True);
    }

    public Hl7.Fhir.Rest.SummaryType Get(SummaryType summaryType)
    {
      if (Map.ContainsKey(summaryType))
      {
        return Map[summaryType];
      }
      else
      {
        string Message = $"Unable to convert summaryType enum to equivalent FHIR type{summaryType.GetCode()}";
        throw new Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, Message);
      }
    }
  }
}
