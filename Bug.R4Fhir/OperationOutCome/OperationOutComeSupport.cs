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
      return GetOpOutCome(errorMessageList, OperationOutcome.IssueSeverity.Fatal, OperationOutcome.IssueType.Exception);
    }

    public OperationOutcome GetError(string[] errorMessageList)
    {
      return GetOpOutCome(errorMessageList, OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.Processing);
    }

    public OperationOutcome GetWarning(string[] errorMessageList)
    {
      return GetOpOutCome(errorMessageList, OperationOutcome.IssueSeverity.Warning, OperationOutcome.IssueType.Informational);
    }

    public OperationOutcome GetInformation(string[] errorMessageList)
    {
      return GetOpOutCome(errorMessageList, OperationOutcome.IssueSeverity.Information, OperationOutcome.IssueType.Informational);
    }

    private OperationOutcome GetOpOutCome(string[] errorMessageList, OperationOutcome.IssueSeverity issueSeverity, OperationOutcome.IssueType issueType)
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
        Issue.Code = issueType;
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
