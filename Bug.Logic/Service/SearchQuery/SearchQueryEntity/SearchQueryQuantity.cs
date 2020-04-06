using Bug.Common.Enums;
using Bug.Common.StringTools;
using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{ 
  public class SearchQueryQuantity : SearchQueryBase
  {
    #region Private Properties
    private const char VerticalBarDelimiter = '|';
    #endregion

    #region Constructor
    public SearchQueryQuantity(SearchParameter SearchParameter, Bug.Common.Enums.ResourceType ResourceContext, string RawValue)
      : base(SearchParameter, ResourceContext, RawValue)
    {
      this.SearchParamTypeId = Bug.Common.Enums.SearchParamType.Quantity;   
      this.ValueList = new List<SearchQueryQuantityValue>();
    }
    #endregion

    public List<SearchQueryQuantityValue> ValueList { get; set; }

    public override object CloneDeep()
    {
      var Clone = new SearchQueryQuantity(this as SearchParameter, this.ResourceContext, this.RawValue);
      base.CloneDeep(Clone);
      Clone.ValueList = new List<SearchQueryQuantityValue>();
      Clone.ValueList.AddRange(this.ValueList);
      return Clone;
    }

    public override bool TryParseValue(string Values)
    {
      this.ValueList.Clear();
      foreach (var Value in Values.Split(OrDelimiter))
      {
//        var DtoSearchParameterNumber = new SearchQueryQuantityValue();
        if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
        {
          bool? IsMissing = SearchQueryQuantityValue.ParseModifierEqualToMissing(Value);
          if (IsMissing.HasValue)
          {
            this.ValueList.Add(new SearchQueryQuantityValue(IsMissing.Value, null, null, null, null, null, null));
          }
          else
          {
            this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet is value was expected to be true or false yet found '{Value}'. ";
            return false;
          }
        }
        else
        {
          //Examples:
          //Syntax: [parameter]=[prefix][number]|[system]|[code] matches a quantity with the given unit    
          //Observation?value=5.4|http://unitsofmeasure.org|mg
          //Observation?value=5.4||mg
          //Observation?value=le5.4|http://unitsofmeasure.org|mg
          //Observation?value=ap5.4|http://unitsofmeasure.org|mg

          //Observation?value=ap5.4
          //Observation?value=ap5.4|
          //Observation?value=ap5.4|http://unitsofmeasure.org
          //Observation?value=ap5.4|http://unitsofmeasure.org|
          
          string[] Split = Value.Trim().Split(VerticalBarDelimiter);
          SearchComparator? Prefix = SearchQueryDateTimeValue.GetPrefix(Split[0]);
          if (!SearchQueryQuantityValue.ValidatePreFix(this.SearchParamTypeId, Prefix) && Prefix.HasValue)
          {
            this.InvalidMessage = $"The search parameter had an unsupported prefix of '{Prefix.Value.GetCode()}'. ";
            return false;
          }
          string NumberAsString = SearchQueryDateTimeValue.RemovePrefix(Split[0], Prefix);
          if (Split.Count() == 1)
          {
            decimal TempDouble;
            if (Decimal.TryParse(NumberAsString, out TempDouble))
            {
              var DtoSearchParameterNumber = new SearchQueryQuantityValue(false,
                Prefix,
                null, 
                null, 
                StringSupport.GetPrecisionFromDecimal(NumberAsString), 
                StringSupport.GetScaleFromDecimal(NumberAsString), 
                TempDouble);
              this.ValueList.Add(DtoSearchParameterNumber);
            }
            else
            {
              this.InvalidMessage = $"Expected a Quantity value yet was unable to parse the provided value '{NumberAsString}' as a Decimal. ";
              return false;
            }
          }
          else if (Split.Count() == 2)
          {
            decimal TempDouble;
            if (Decimal.TryParse(NumberAsString, out TempDouble))
            {
              string? System;
              if (!string.IsNullOrWhiteSpace(Split[1].Trim()))
              {
                System = Split[1].Trim();
              }
              else
              {
                System = null;
              }
              
              var DtoSearchParameterNumber = new SearchQueryQuantityValue(false,
                Prefix,
                System,
                null,
                StringSupport.GetPrecisionFromDecimal(NumberAsString),
                StringSupport.GetScaleFromDecimal(NumberAsString),
                TempDouble);

              this.ValueList.Add(DtoSearchParameterNumber);
            }
            else
            {
              this.InvalidMessage = $"Expected a Quantity value yet was unable to parse the provided value '{NumberAsString}' as a Decimal. ";
              return false;
            }
          }
          else if (Split.Count() == 3)
          {             
            decimal TempDouble;
            if (Decimal.TryParse(NumberAsString, out TempDouble))
            {
              string? System = null;
              if (!string.IsNullOrWhiteSpace(Split[1].Trim()))
              {
                System = Split[1].Trim();
              }

              string? Code = null;
              if (!string.IsNullOrWhiteSpace(Split[2].Trim()))
              {
                Code = Split[2].Trim();
              }
              
              var DtoSearchParameterNumber = new SearchQueryQuantityValue(false,
                Prefix,
                System,
                Code,
                StringSupport.GetPrecisionFromDecimal(NumberAsString),
                StringSupport.GetScaleFromDecimal(NumberAsString),
                TempDouble);

              this.ValueList.Add(DtoSearchParameterNumber);
            }
            else
            {
              this.InvalidMessage = $"Expected a Quantity value yet was unable to parse the provided value '{NumberAsString}' as a Decimal. ";
              return false;
            }
          }
          else
          {
            this.InvalidMessage = $"Expected a Quantity value type yet found to many {VerticalBarDelimiter} Delimiters. ";
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
