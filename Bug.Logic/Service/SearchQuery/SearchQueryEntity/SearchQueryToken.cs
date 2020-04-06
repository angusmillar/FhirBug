using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System.Collections.Generic;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryToken : SearchQueryBase
  {
    protected const char TokenDelimiter = '|';

    #region Constructor
    public SearchQueryToken(SearchParameter SearchParameter, Bug.Common.Enums.ResourceType ResourceContext, string RawValue)
      : base(SearchParameter, ResourceContext, RawValue)
    {
      this.SearchParamTypeId = Bug.Common.Enums.SearchParamType.Token;
      this.ValueList = new List<SearchQueryTokenValue>();
    }
    #endregion

    public List<SearchQueryTokenValue> ValueList { get; set; }

    public override object CloneDeep()
    {
      var Clone = new SearchQueryToken(this as SearchParameter, this.ResourceContext, this.RawValue);
      base.CloneDeep(Clone);
      Clone.ValueList = new List<SearchQueryTokenValue>();
      Clone.ValueList.AddRange(this.ValueList);
      return Clone;
    }
    public override bool TryParseValue(string Values)
    {
      this.ValueList = new List<SearchQueryTokenValue>();
      foreach (var Value in Values.Split(OrDelimiter))
      {
        //var DtoSearchParameterNumber = new SearchQueryTokenValue();
        if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
        {
          bool? IsMissing = SearchQueryTokenValue.ParseModifierEqualToMissing(Value);
          if (IsMissing.HasValue)
          {
            this.ValueList.Add(new SearchQueryTokenValue(IsMissing.Value, null, null, null));
          }
          else
          {
            this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet is value was expected to be true or false yet found '{Value}'. ";
            return false;
          }
        }
        else
        {
          if (Value.Contains(TokenDelimiter))
          {
            string[] CodeSystemSplit = Value.Split(TokenDelimiter);
            string Code = CodeSystemSplit[1].Trim();
            string System = CodeSystemSplit[0].Trim();
            SearchQueryTokenValue.TokenSearchType? TokenSearchType;
            if (string.IsNullOrEmpty(Code) && string.IsNullOrEmpty(System))
            {
              this.InvalidMessage = $"Unable to parse the Token search parameter value of '{Value}' as there was no Code and System separated by a '{TokenDelimiter}' delimiter";
              return false;
            }
            else if (string.IsNullOrEmpty(System))
            {
              TokenSearchType = SearchQueryTokenValue.TokenSearchType.MatchCodeWithNullSystem;
            }
            else if (string.IsNullOrEmpty(Code))
            {
              TokenSearchType = SearchQueryTokenValue.TokenSearchType.MatchSystemOnly;
            }
            else
            {
              TokenSearchType = SearchQueryTokenValue.TokenSearchType.MatchCodeAndSystem;
            }
            this.ValueList.Add(new SearchQueryTokenValue(false, TokenSearchType, Code, System));
          }
          else
          {
            SearchQueryTokenValue.TokenSearchType? TokenSearchType = SearchQueryTokenValue.TokenSearchType.MatchCodeOnly;
            string Code = Value.Trim();
            if (string.IsNullOrEmpty(Code))
            {
              this.InvalidMessage = $"Unable to parse the Token search parameter value of '{Value}' as there was no Code found.";
              return false;
            }
            this.ValueList.Add(new SearchQueryTokenValue(false, TokenSearchType, Code, null));
          }
        }
      }
      if (this.ValueList.Count == 0)
        return false;
      else
        return true;
    }
   

  }
}
