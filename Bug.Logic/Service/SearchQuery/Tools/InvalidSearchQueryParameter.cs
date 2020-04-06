using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.Tools
{
  public class InvalidSearchQueryParameter
  {
    public string Name { get; set; }
    public string Value { get; set; }
    public string ErrorMessage { get; set; }
    public InvalidSearchQueryParameter(string Name, string Value, string ErrorMessage)
    {
      this.Name = Name;
      this.Value = Value;
      this.ErrorMessage = ErrorMessage;
    }
    public InvalidSearchQueryParameter(string RawParameter, string ErrorMessage)
    {
      string[] Spit = RawParameter.Split('=');
      this.Name = Spit[0];
      if (Spit.Length > 1)
        this.Value = Spit[1];
      else
        this.Value = string.Empty;
      this.ErrorMessage = ErrorMessage;
    }
  }
}
