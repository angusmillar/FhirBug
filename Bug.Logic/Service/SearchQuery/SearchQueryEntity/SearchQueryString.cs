using Bug.Common.DatabaseTools;
using Bug.Common.Enums;
using Bug.Common.StringTools;
using Bug.Logic.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryString : SearchQueryBase
  {
    #region Constructor
    public SearchQueryString(SearchParameter SearchParameter, Bug.Common.Enums.ResourceType ResourceContext, string RawValue)
      : base(SearchParameter, ResourceContext, RawValue)
    {
      this.SearchParamTypeId = Bug.Common.Enums.SearchParamType.String;
      this.ValueList = new List<SearchQueryStringValue>();
    }
    #endregion
    public List<SearchQueryStringValue> ValueList { get; set; }

    public override object CloneDeep()
    {
      var Clone = new SearchQueryString(this as SearchParameter, this.ResourceContext, this.RawValue);
      base.CloneDeep(Clone);
      Clone.ValueList = new List<SearchQueryStringValue>();
      Clone.ValueList.AddRange(this.ValueList);
      return Clone;
    }


    public override void ParseValue(string Values)
    {
      this.IsValid = true;
      this.ValueList.Clear();
      foreach (string Value in Values.Split(OrDelimiter))
      {
        //var SearchQueryStringValue = new SearchQueryStringValue();
        if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
        {
          bool? IsMissing = SearchQueryStringValue.ParseModifierEqualToMissing(Value);
          if (IsMissing.HasValue)
          {
            this.ValueList.Add(new SearchQueryStringValue(IsMissing.Value, null));
          }
          else
          {
            this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet the value was expected to be true or false yet found '{Value}'. ";
            this.IsValid = false;
            break;
          }
        }
        else
        {
          this.ValueList.Add(new SearchQueryStringValue(false, StringSupport.ToLowerTrimRemoveDiacriticsTruncate(Value, DatabaseMetaData.FieldLength.StringMaxLength)));
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
    //    //var SearchQueryStringValue = new SearchQueryStringValue();
    //    if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
    //    {
    //      bool? IsMissing = SearchQueryStringValue.ParseModifierEqualToMissing(Value);
    //      if (IsMissing.HasValue)
    //      {            
    //        this.ValueList.Add(new SearchQueryStringValue(IsMissing.Value, null));
    //      }
    //      else
    //      {
    //        this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet the value was expected to be true or false yet found '{Value}'. ";
    //        return false;
    //      }
    //    }
    //    else
    //    {          
    //      this.ValueList.Add(new SearchQueryStringValue(false, StringSupport.ToLowerTrimRemoveDiacriticsTruncate(Value, DatabaseMetaData.FieldLength.StringMaxLength)));
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
