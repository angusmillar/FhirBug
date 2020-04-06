using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryComposite : SearchQueryBase
  {
    #region Constructor
    public SearchQueryComposite(SearchParameter SearchParameter, Bug.Common.Enums.ResourceType ResourceContext, string RawValue)
      : base(SearchParameter, ResourceContext, RawValue)
    {
      this.SearchParamTypeId = Bug.Common.Enums.SearchParamType.Composite;
      this.ValueList = new List<SearchQueryCompositeValue>();
    }
    #endregion

    public List<SearchQueryCompositeValue> ValueList { get; set; }

    public override object CloneDeep()
    {
      var Clone = new SearchQueryComposite(this as SearchParameter, this.ResourceContext, this.RawValue);
      base.CloneDeep(Clone);
      Clone.ValueList = new List<SearchQueryCompositeValue>();
      Clone.ValueList.AddRange(this.ValueList);
      return Clone;
    }

    public void ParseCompositeValue(List<ISearchQueryBase> SearchParameterBaseList, string Values)
    {
      this.ValueList = new List<SearchQueryCompositeValue>();
      foreach (string Value in Values.Split(OrDelimiter))
      {
        //var DtoSearchParameterCompositeValue = new SearchQueryCompositeValue();
        if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
        {
          bool? IsMissing = SearchQueryCompositeValue.ParseModifierEqualToMissing(Value);
          if (IsMissing.HasValue)
          {
            this.ValueList.Add(new SearchQueryCompositeValue(IsMissing.Value));
          }
          else
          {
            this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet is value was expected to be true or false yet found '{Value}'. ";
            this.IsValid = false;           
          }
        }
        else
        {
          var CompositeSplit = Value.Split(CompositeDelimiter);
          if (CompositeSplit.Count() != SearchParameterBaseList.Count())
          {
            StringBuilder sb = new StringBuilder();
            sb.Append($"The SearchParameter {this.Name} is a Composite type search parameter which means it expects more than a single search value where each value must be separated by a '{CompositeDelimiter}' delimiter character. " +
                      $"However, this instance provided had more or less values than is required for the search parameter used. " +
                      $"{this.Name} expects {SearchParameterBaseList.Count()} values yet {CompositeSplit.Count()} found. ");
            sb.Append("The values expected for this parameter are as follows: ");
            int counter = 1;
            foreach (var item in SearchParameterBaseList)
            {
              string ResourceNameList = string.Empty;
              foreach(SearchParameterResourceType SearchParameterResourceType in item.ResourceTypeList)
              {
                ResourceNameList += $"{SearchParameterResourceType.ResourceTypeId.GetCode()}, ";
              }

              sb.Append($"Value: {counter.ToString()} is to be a {item.SearchParamTypeId.GetCode()} search parameter type as per the single search parameter '{item.Name}' for the resource types {ResourceNameList}. ");
              counter++;
            }
            this.InvalidMessage = sb.ToString();
            this.IsValid = false;
          }

          var SearchQueryCompositeValue = new SearchQueryCompositeValue(false);
          for (int i = 0; i < CompositeSplit.Length; i++)
          {
            SearchParameterBaseList[i].RawValue = SearchParameterBaseList[i].Name + ParameterValueDelimiter + CompositeSplit[i];
            if (!SearchParameterBaseList[i].TryParseValue(CompositeSplit[i]))
            {
              string ResourceNameList = string.Empty;
              foreach (SearchParameterResourceType SearchParameterResourceType in SearchParameterBaseList[i].ResourceTypeList)
              {
                ResourceNameList += $"{SearchParameterResourceType.ResourceTypeId.GetCode()}, ";
              }

              int ItemCount = i + 1;
              string Error = $"Value: {ItemCount.ToString()} is to be a {SearchParameterBaseList[i].SearchParamTypeId.GetCode()} search parameter type as per the single search parameter '{SearchParameterBaseList[i].Name}' for the resource type {ResourceNameList}. " +
                $"However, an error was found in parsing its value. Further information: {SearchParameterBaseList[i].InvalidMessage}";
              this.InvalidMessage = Error;
              this.IsValid = false;
              break;
            }
            else
            {
              SearchQueryCompositeValue.SearchQueryBaseList.Add(SearchParameterBaseList[i]);
            }
          }
          this.ValueList.Add(SearchQueryCompositeValue);
        }
      }
      if (this.ValueList.Count() > 1)
        this.HasLogicalOrProperties = true;
      if (this.ValueList.Count == 0)
      {
        this.InvalidMessage = $"No values were found for the search parameter name {this.Name}";
        this.IsValid = false;        
      }      
    }

    public override bool TryParseValue(string Values)
    {
      throw new ApplicationException("Internal Server Error: Composite Search Parameters values must be parsed with the specialized method 'TryParseCompositeValue'");
    }
    
  }
}
