using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;

namespace Bug.R4Fhir.OperationOutCome
{
  public class OperationOutComeSupport : IR4OperationOutComeSupport
  {
    public OperationOutcome GetFatal(string[] errorMessageList)
    {
      return GetOpOutCome(errorMessageList, OperationOutcome.IssueSeverity.Fatal);
    }

    public OperationOutcome GetError(string[] errorMessageList)
    {
      return GetOpOutCome(errorMessageList, OperationOutcome.IssueSeverity.Error);
    }

    public OperationOutcome GetWarning(string[] errorMessageList)
    {
      return GetOpOutCome(errorMessageList, OperationOutcome.IssueSeverity.Warning);
    }

    public OperationOutcome GetInformation(string[] errorMessageList)
    {
      return GetOpOutCome(errorMessageList, OperationOutcome.IssueSeverity.Information);
    }

    private OperationOutcome GetOpOutCome(string[] errorMessageList, OperationOutcome.IssueSeverity issueSeverity)
    {
      var Opt = new OperationOutcome();
      Opt.Issue = new List<OperationOutcome.IssueComponent>();

      StringBuilder sb = new StringBuilder();
      sb.Append("<div xmlns=\"http://www.w3.org/1999/xhtml\">\n");
      int Counter = 1;
      foreach (string ErrorMsg in errorMessageList)
      {
        if (errorMessageList.Length == 1)
        {
          sb.Append($"  <p>{ErrorMsg}</p>\n");
        }
        else
        {
          sb.Append($"  <p> {Counter.ToString()}. {ErrorMsg}</p>\n");
        }

        var Issue = new OperationOutcome.IssueComponent();
        Issue.Severity = issueSeverity;
        Issue.Code = OperationOutcome.IssueType.Exception;
        Issue.Details = new CodeableConcept();
        Issue.Details.Text = ErrorMsg;
        Opt.Issue.Add(Issue);
        Counter++;
      }
      sb.Append("</div>");

      Opt.Text = new Narrative();
      Opt.Text.Div = sb.ToString();
      return Opt;
    }
  }
}
