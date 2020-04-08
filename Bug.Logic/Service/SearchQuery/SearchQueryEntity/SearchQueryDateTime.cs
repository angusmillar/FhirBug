using Bug.Common.Enums;
using System.Collections.Generic;
using Bug.Common.DateTimeTools;
using Bug.Logic.DomainModel;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryDateTime : SearchQueryBase
  {
    #region Constructor
    public SearchQueryDateTime(SearchParameter SearchParameter, Bug.Common.Enums.ResourceType ResourceContext, string RawValue)
      : base(SearchParameter, ResourceContext, RawValue)
    {
      this.SearchParamTypeId = Bug.Common.Enums.SearchParamType.Date;
      this.ValueList = new List<SearchQueryDateTimeValue>();
    }
    #endregion

    public List<SearchQueryDateTimeValue> ValueList { get; set; }

    public override object CloneDeep()
    {
      var Clone = new SearchQueryDateTime(this as SearchParameter, this.ResourceContext, this.RawValue);
      base.CloneDeep(Clone);
      Clone.ValueList = new List<SearchQueryDateTimeValue>();
      Clone.ValueList.AddRange(this.ValueList);
      return Clone;
    }
    
    public override void ParseValue(string Values)
    {
      this.IsValid = true;
      this.ValueList = new List<SearchQueryDateTimeValue>();
      foreach (string Value in Values.Split(OrDelimiter))
      {
        //var DtoSearchParameterDateTimeValue = new SearchQueryDateTimeValue();
        if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
        {
          bool? IsMissing = SearchQueryDateTimeValue.ParseModifierEqualToMissing(Value);
          if (IsMissing.HasValue)
          {
            ValueList.Add(new SearchQueryDateTimeValue(IsMissing.Value, null, null, null));
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

          SearchComparator? Prefix = SearchQueryDateTimeValue.GetPrefix(Value);
          if (!SearchQueryQuantityValue.ValidatePreFix(this.SearchParamTypeId, Prefix) && Prefix.HasValue)
          {
            this.InvalidMessage = $"The search parameter had an unsupported prefix of '{Prefix.Value.GetCode()}'. ";
            this.IsValid = false;
            break;            
          }

          string DateTimeStirng = SearchQueryDateTimeValue.RemovePrefix(Value, Prefix);
          FhirDateTimeSupport FhirDateTimeSupport = new FhirDateTimeSupport(DateTimeStirng.Trim());
          if (FhirDateTimeSupport.IsValid)
          {
            if (FhirDateTimeSupport.Value.HasValue)
            {
              var SearchQueryDateTimeValue = new SearchQueryDateTimeValue(false, Prefix, FhirDateTimeSupport.Precision, FhirDateTimeSupport.Value.Value);
              ValueList.Add(SearchQueryDateTimeValue);
            }
            else
            {
              throw new System.ApplicationException($"Internal Server Error: The {nameof(FhirDateTimeSupport)} property of {nameof(FhirDateTimeSupport.IsValid)} was True yet the Value was null. This should not happen." );
            }
          }
          else
          {
            this.InvalidMessage = $"Unable to parse the provided value of {DateTimeStirng.Trim()} as a FHIR DateTime.";
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
    //  this.ValueList = new List<SearchQueryDateTimeValue>();
    //  foreach (string Value in Values.Split(OrDelimiter))
    //  {
    //    //var DtoSearchParameterDateTimeValue = new SearchQueryDateTimeValue();
    //    if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
    //    {
    //      bool? IsMissing = SearchQueryDateTimeValue.ParseModifierEqualToMissing(Value);
    //      if (IsMissing.HasValue)
    //      {            
    //        ValueList.Add(new SearchQueryDateTimeValue(IsMissing.Value, null, null, null));
    //      }
    //      else
    //      {
    //        this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet is value was expected to be true or false yet found '{Value}'. ";
    //        return false;
    //      }
    //    }
    //    else
    //    {
          
    //      SearchComparator? Prefix = SearchQueryDateTimeValue.GetPrefix(Value);
    //      if (!SearchQueryQuantityValue.ValidatePreFix(this.SearchParamTypeId, Prefix) && Prefix.HasValue)
    //      {
    //        this.InvalidMessage = $"The search parameter had an unsupported prefix of '{Prefix.Value.GetCode()}'. ";
    //        return false;
    //      }

    //      string DateTimeStirng = SearchQueryDateTimeValue.RemovePrefix(Value, Prefix);
    //      FhirDateTimeSupport FhirDateTimeSupport = new FhirDateTimeSupport(DateTimeStirng.Trim());
    //      if (FhirDateTimeSupport.IsValid)
    //      {
    //        if (FhirDateTimeSupport.Value.HasValue)
    //        {
    //          var SearchQueryDateTimeValue = new SearchQueryDateTimeValue(false, Prefix, FhirDateTimeSupport.Precision, FhirDateTimeSupport.Value.Value);
    //          ValueList.Add(SearchQueryDateTimeValue);
    //        }
    //        else
    //        {

    //          return false;
    //        }
    //      }
    //      else
    //      {
    //        this.InvalidMessage = $"Unable to parse the provided value of {DateTimeStirng.Trim()} as a FHIR DateTime.";
    //        return false;
    //      }
    //    }
    //  }
    //  if (ValueList.Count > 1)
    //  {
    //    this.HasLogicalOrProperties = true;
    //  }
    //  if (this.ValueList.Count == 0)
    //  {
    //    return false;
    //  }
    //  else
    //  {
    //    return true;
    //  }
    //}
    
  }
}
