using Bug.Common.DecimalTools;
using Bug.Common.Enums;
using Bug.Common.StringTools;
using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Bug.Common.DecimalTools.DecimalSupport;

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


    public override void ParseValue(string Values)
    {
      this.IsValid = true;
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
            this.IsValid = false;
            break;
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
            this.IsValid = false;
            break;
          }
          string NumberAsString = SearchQueryDateTimeValue.RemovePrefix(Split[0], Prefix).Trim();
          if (Split.Count() == 1)
          {            
            if (Decimal.TryParse(NumberAsString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal TempDecimal))
            {
              DecimalInfo DecimalInfo = DecimalSupport.GetDecimalInfo(TempDecimal);
              var DtoSearchParameterNumber = new SearchQueryQuantityValue(false,
                Prefix,
                null,
                null,
                DecimalInfo.Precision,
                DecimalInfo.Scale,
                TempDecimal);
              this.ValueList.Add(DtoSearchParameterNumber);
            }
            else
            {
              this.InvalidMessage = $"Expected a Quantity value yet was unable to parse the provided value '{NumberAsString}' as a Decimal. ";
              this.IsValid = false;
              break;              
            }
          }
          else if (Split.Count() == 2)
          {            
            if (Decimal.TryParse(NumberAsString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal TempDecimal))
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
              DecimalInfo DecimalInfo = DecimalSupport.GetDecimalInfo(TempDecimal);
              var DtoSearchParameterNumber = new SearchQueryQuantityValue(false,
                Prefix,
                System,
                null,
                DecimalInfo.Precision,
                DecimalInfo.Scale,
                TempDecimal);

              this.ValueList.Add(DtoSearchParameterNumber);
            }
            else
            {
              this.InvalidMessage = $"Expected a Quantity value yet was unable to parse the provided value '{NumberAsString}' as a Decimal. ";
              this.IsValid = false;
              break;              
            }
          }
          else if (Split.Count() == 3)
          {            
            if (Decimal.TryParse(NumberAsString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal TempDecimal))
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
              DecimalInfo DecimalInfo = DecimalSupport.GetDecimalInfo(TempDecimal);
              var DtoSearchParameterNumber = new SearchQueryQuantityValue(false,
                Prefix,
                System,
                Code,
                DecimalInfo.Precision,
                DecimalInfo.Scale,
                TempDecimal);

              this.ValueList.Add(DtoSearchParameterNumber);
            }
            else
            {
              this.InvalidMessage = $"Expected a Quantity value yet was unable to parse the provided value '{NumberAsString}' as a Decimal. ";
              this.IsValid = false;
              break;              
            }
          }
          else
          {
            this.InvalidMessage = $"Expected a Quantity value type yet found to many {VerticalBarDelimiter} Delimiters. ";
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

//    public override bool ParseValue(string Values)
//    {
//      this.ValueList.Clear();
//      foreach (var Value in Values.Split(OrDelimiter))
//      {
////        var DtoSearchParameterNumber = new SearchQueryQuantityValue();
//        if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
//        {
//          bool? IsMissing = SearchQueryQuantityValue.ParseModifierEqualToMissing(Value);
//          if (IsMissing.HasValue)
//          {
//            this.ValueList.Add(new SearchQueryQuantityValue(IsMissing.Value, null, null, null, null, null, null));
//          }
//          else
//          {
//            this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet is value was expected to be true or false yet found '{Value}'. ";
//            return false;
//          }
//        }
//        else
//        {
//          //Examples:
//          //Syntax: [parameter]=[prefix][number]|[system]|[code] matches a quantity with the given unit    
//          //Observation?value=5.4|http://unitsofmeasure.org|mg
//          //Observation?value=5.4||mg
//          //Observation?value=le5.4|http://unitsofmeasure.org|mg
//          //Observation?value=ap5.4|http://unitsofmeasure.org|mg

//          //Observation?value=ap5.4
//          //Observation?value=ap5.4|
//          //Observation?value=ap5.4|http://unitsofmeasure.org
//          //Observation?value=ap5.4|http://unitsofmeasure.org|
          
//          string[] Split = Value.Trim().Split(VerticalBarDelimiter);
//          SearchComparator? Prefix = SearchQueryDateTimeValue.GetPrefix(Split[0]);
//          if (!SearchQueryQuantityValue.ValidatePreFix(this.SearchParamTypeId, Prefix) && Prefix.HasValue)
//          {
//            this.InvalidMessage = $"The search parameter had an unsupported prefix of '{Prefix.Value.GetCode()}'. ";
//            return false;
//          }
//          string NumberAsString = SearchQueryDateTimeValue.RemovePrefix(Split[0], Prefix).Trim();
//          if (Split.Count() == 1)
//          {
//            decimal TempDecimal;
//            if (Decimal.TryParse(NumberAsString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out TempDecimal))
//            {
//              DecimalInfo DecimalInfo = DecimalSupport.GetDecimalInfo(TempDecimal);
//              var DtoSearchParameterNumber = new SearchQueryQuantityValue(false,
//                Prefix,
//                null, 
//                null,
//                DecimalInfo.Precision,
//                DecimalInfo.Scale,
//                TempDecimal);
//              this.ValueList.Add(DtoSearchParameterNumber);
//            }
//            else
//            {
//              this.InvalidMessage = $"Expected a Quantity value yet was unable to parse the provided value '{NumberAsString}' as a Decimal. ";
//              return false;
//            }
//          }
//          else if (Split.Count() == 2)
//          {
//            decimal TempDecimal;
//            if (Decimal.TryParse(NumberAsString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out TempDecimal))
//            {
//              string? System;
//              if (!string.IsNullOrWhiteSpace(Split[1].Trim()))
//              {
//                System = Split[1].Trim();
//              }
//              else
//              {
//                System = null;
//              }
//              DecimalInfo DecimalInfo = DecimalSupport.GetDecimalInfo(TempDecimal);
//              var DtoSearchParameterNumber = new SearchQueryQuantityValue(false,
//                Prefix,
//                System,
//                null,
//                DecimalInfo.Precision,
//                DecimalInfo.Scale,
//                TempDecimal);

//              this.ValueList.Add(DtoSearchParameterNumber);
//            }
//            else
//            {
//              this.InvalidMessage = $"Expected a Quantity value yet was unable to parse the provided value '{NumberAsString}' as a Decimal. ";
//              return false;
//            }
//          }
//          else if (Split.Count() == 3)
//          {            
//            decimal TempDecimal;
//            if (Decimal.TryParse(NumberAsString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out TempDecimal))
//            {
//              string? System = null;
//              if (!string.IsNullOrWhiteSpace(Split[1].Trim()))
//              {
//                System = Split[1].Trim();
//              }

//              string? Code = null;
//              if (!string.IsNullOrWhiteSpace(Split[2].Trim()))
//              {
//                Code = Split[2].Trim();
//              }
//              DecimalInfo DecimalInfo = DecimalSupport.GetDecimalInfo(TempDecimal);
//              var DtoSearchParameterNumber = new SearchQueryQuantityValue(false,
//                Prefix,
//                System,
//                Code,
//                DecimalInfo.Precision,
//                DecimalInfo.Scale,
//                TempDecimal);

//              this.ValueList.Add(DtoSearchParameterNumber);
//            }
//            else
//            {
//              this.InvalidMessage = $"Expected a Quantity value yet was unable to parse the provided value '{NumberAsString}' as a Decimal. ";
//              return false;
//            }
//          }
//          else
//          {
//            this.InvalidMessage = $"Expected a Quantity value type yet found to many {VerticalBarDelimiter} Delimiters. ";
//            return false;
//          }
//        }
//      }
//      if (this.ValueList.Count == 0)
//        return false;
//      else
//        return true;
//    }


 
  }
}
