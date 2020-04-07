using Bug.Common.DecimalTools;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.StringTools;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.SearchQuery.Tools;
using System;
using System.Collections.Generic;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryNumber : SearchQueryBase
  {
    #region Constructor
    public SearchQueryNumber(SearchParameter SearchParameter, Bug.Common.Enums.ResourceType ResourceContext, string RawValue)
      : base(SearchParameter, ResourceContext, RawValue)
    {      
      this.SearchParamTypeId = Bug.Common.Enums.SearchParamType.Number;
      this.ValueList = new List<SearchQueryNumberValue>(); 
    }
    #endregion

    public List<SearchQueryNumberValue> ValueList { get; set; }

    public override object CloneDeep()
    {
      var Clone = new SearchQueryNumber(this as SearchParameter, this.ResourceContext, this.RawValue);
      base.CloneDeep(Clone);
      Clone.ValueList = new List<SearchQueryNumberValue>();
      Clone.ValueList.AddRange(this.ValueList);
      return Clone;
    }

    public override bool TryParseValue(string Values)
    {
      this.ValueList = new List<SearchQueryNumberValue>();
      foreach (var Value in Values.Split(OrDelimiter))
      {
        //var DtoSearchParameterNumber = new SearchQueryNumberValue();
        if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
        {
          bool? IsMissing = SearchQueryNumberValue.ParseModifierEqualToMissing(Value);
          if (IsMissing.HasValue)
          {
            this.ValueList.Add(new SearchQueryNumberValue(IsMissing.Value, null, null, null, null));
          }
          else
          {
            this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet is value was expected to be true or false yet found '{Value}'. ";
            return false;
          }
        }
        else
        {
          decimal TempDouble;          
          SearchComparator? Prefix = SearchQueryDateTimeValue.GetPrefix(Value);
          if (!SearchQueryQuantityValue.ValidatePreFix(this.SearchParamTypeId, Prefix) && Prefix.HasValue)
          {
            this.InvalidMessage = $"The search parameter had an unsupported prefix of '{Prefix.Value.GetCode()}'. ";
            return false;
          }

          string NumberAsString = SearchQueryDateTimeValue.RemovePrefix(Value, Prefix);
          if (Decimal.TryParse(NumberAsString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal TempDecimal))
          {
            var DecimalInfo = DecimalSupport.GetDecimalInfo(TempDecimal);
            var SearchQueryNumberValue = new SearchQueryNumberValue(false,
              Prefix,
              DecimalInfo.Precision,
              DecimalInfo.Scale,
              TempDecimal);
            this.ValueList.Add(SearchQueryNumberValue);
          }
          else
          {
            return false;
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
