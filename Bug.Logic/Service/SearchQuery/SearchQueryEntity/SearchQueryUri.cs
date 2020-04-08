using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryUri : SearchQueryBase
  {
    #region Constructor
    public SearchQueryUri(SearchParameter SearchParameter, Bug.Common.Enums.ResourceType ResourceContext, string RawValue)
      : base(SearchParameter, ResourceContext, RawValue)
    {
      this.SearchParamTypeId = Bug.Common.Enums.SearchParamType.Uri;
      this.ValueList = new List<SearchQueryUriValue>();
    }
    #endregion
    public List<SearchQueryUriValue> ValueList { get; set; }

    public override object CloneDeep()
    {
      var Clone = new SearchQueryUri(this as SearchParameter, this.ResourceContext, this.RawValue);
      base.CloneDeep(Clone);
      Clone.ValueList = new List<SearchQueryUriValue>();
      Clone.ValueList.AddRange(this.ValueList);
      return Clone;
    }

    public override void ParseValue(string Values)
    {
      this.IsValid = true;
      this.ValueList.Clear();
      foreach (string Value in Values.Split(OrDelimiter))
      {
        //var SearchQueryUriValue = new SearchQueryUriValue();
        if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
        {
          bool? IsMissing = SearchQueryUriValue.ParseModifierEqualToMissing(Value);
          if (IsMissing.HasValue)
          {
            this.ValueList.Add(new SearchQueryUriValue(IsMissing.Value, null));
          }
          else
          {
            this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet is value was expected to be true or false yet found '{Value}'. ";
            this.IsValid = false;
            break;
          }
        }
        else
        {
          if (Uri.TryCreate(Value.Trim(), UriKind.RelativeOrAbsolute, out Uri? TempUri))
          {
            this.ValueList.Add(new SearchQueryUriValue(false, TempUri));
          }
          else
          {
            this.InvalidMessage = $"Unable to parse the given URI search parameter string of : {Value.Trim()}";
            this.IsValid = false;
            break;
          }
        }
      }

      if (ValueList.Count > 1)
      {
        this.HasLogicalOrProperties = true;
      }

      if (this.ValueList.Count == 0)
      {
        this.InvalidMessage = $"Unable to parse any values into a {this.GetType().Name} from the string: {Values}.";
        this.IsValid = false;
      }
    }
    //public override bool ParseValue(string Values)
    //{
    //  this.ValueList.Clear();
    //  foreach (string Value in Values.Split(OrDelimiter))
    //  {
    //    //var SearchQueryUriValue = new SearchQueryUriValue();
    //    if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
    //    {
    //      bool? IsMissing = SearchQueryUriValue.ParseModifierEqualToMissing(Value);
    //      if (IsMissing.HasValue)
    //      {            
    //        this.ValueList.Add(new SearchQueryUriValue(IsMissing.Value, null));
    //      }
    //      else
    //      {
    //        this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet is value was expected to be true or false yet found '{Value}'. ";
    //        return false;
    //      }
    //    }
    //    else
    //    {          
    //      if (Uri.TryCreate(Value.Trim(), UriKind.RelativeOrAbsolute, out Uri? TempUri))
    //      {            
    //        this.ValueList.Add(new SearchQueryUriValue(false, TempUri));
    //      }
    //      else
    //      {
    //        this.InvalidMessage = $"Unable to parse the given URI search parameter string of : {Value.Trim()}";
    //        return false;
    //      }
    //    }
    //  }
    //  if (this.ValueList.Count() > 1)
    //    this.HasLogicalOrProperties = true;
    //  if (this.ValueList.Count > 0)
    //  {
    //    return true;
    //  }
    //  else
    //  {
    //    return false;
    //  }
    //}
    

  }
}
