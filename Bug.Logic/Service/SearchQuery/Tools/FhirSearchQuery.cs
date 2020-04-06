using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Bug.Common.Enums;
using Bug.Common.StringTools;

namespace Bug.Logic.Service.SearchQuery.Tools
{
  public class FhirSearchQuery : IFhirSearchQuery
  {
    public const string TermInclude = "_include";
    public const string TermRevInclude = "_revinclude";
    public const string TermSort = "_sort";
    public const string TermCount = "_count";
    public const string TermContained = "_contained";
    public const string TermContainedType = "_containedType";
    public const string TermSummary = "_summary";
    public const string TermText = "_text";
    public const string TermContent = "_content";
    public const string TermQuery = "_query";
    public const string TermFilter = "_filter";
    public const string TermPage = "page";
    public const char TermChainDelimiter = '.';
    public const char TermSearchModifierDelimiter = ':';

    public int? Count { get; set; }
    public int? Page { get; set; }
    public IList<string> RevInclude { get; set; }
    public IList<string> Include { get; set; }
    public IList<SortParameter> Sort { get; set; }
    public IList<InvalidSearchQueryParameter> InvalidParameterList { get; set; }
    public ContainedSearch? Contained { get; set; }
    public ContainedType? ContainedType { get; set; }
    public SummaryType? SummaryType { get; set; }
    public Dictionary<string, StringValues> ParametersDictionary { get; set; }
    public string? Content { get; set; }
    public string? Text { get; set; }
    public string? Query { get; set; }
    public string? Filter { get; set; }
    private bool QueryItemProcessed { get; set; }

    public FhirSearchQuery()
    {
      this.InvalidParameterList = new List<InvalidSearchQueryParameter>();
      this.RevInclude = new List<string>();
      this.Include = new List<string>();
      this.Sort = new List<SortParameter>();
      this.ParametersDictionary = new Dictionary<string, StringValues>();
      this.QueryItemProcessed = false;
    }

    public bool Parse(Dictionary<string, StringValues> query)
    {
      this.InvalidParameterList = new List<InvalidSearchQueryParameter>();
      this.RevInclude = new List<string>();
      this.Include = new List<string>();
      this.Sort = new List<SortParameter>();
      this.ParametersDictionary = new Dictionary<string, StringValues>();
      this.QueryItemProcessed = false;

      foreach (var Item in query)
      {
        this.QueryItemProcessed = false;
        //Count                
        this.Count = TryParseIntegerParameter(Item, TermCount);
        
        //Page
        this.Page = TryParseIntegerParameter(Item, TermPage);

        //Include
        this.Include = ParseStringList(Item, TermInclude);

        //RevInclude
        this.RevInclude = ParseStringList(Item, TermRevInclude);

        //Sort
        ParseSortParameter(Item);

        //ContainedType       
        if (TryParseSingleEnumTerm(Item, TermContained, out ContainedSearch Contained))
        {
          this.Contained = Contained;
        }
        //ContainedType       
        if (TryParseSingleEnumTerm(Item, TermContainedType, out ContainedType ContainedType))
        {
          this.ContainedType = ContainedType;
        }

        //SummaryType       
        if (TryParseSingleEnumTerm(Item, TermContained, out SummaryType SummaryType))
        {
          this.SummaryType = SummaryType;
        }

        //Content
        this.Content = GetSimpleStringParameter(Item, TermContent);

        //Text
        this.Text = GetSimpleStringParameter(Item, TermText);

        //Query
        this.Query = GetSimpleStringParameter(Item, TermQuery);

        //Filter
        this.Filter = GetSimpleStringParameter(Item, TermFilter);

        //And any other query parameters to the Parameters list, these will be the general resource search parameters.
        if (!this.QueryItemProcessed)
        {
          this.ParametersDictionary.Add(Item.Key, Item.Value);
        }

      }
      return (this.InvalidParameterList.Count() == 0);
    }

    private string? GetSimpleStringParameter(KeyValuePair<string, StringValues> Item, string Term)
    {
      if (Item.Key == Term)
      {
        this.QueryItemProcessed = true;
        if (!IsParameterValueEmpty(Item))
        {
          CheckSingleParameterForMoreThanOne(Item);
          return Item.Value[Item.Value.Count - 1];
        }
      }
      return null;
    }

    private void CheckSingleParameterForMoreThanOne(KeyValuePair<string, StringValues> Item)
    {
      if (Item.Value.Count > 1)
      {
        for (int i = 0; i < Item.Value.Count - 1; i++)
        {
          this.InvalidParameterList.Add(new InvalidSearchQueryParameter(Item.Key, Item.Value[i], $"Found many parameters of the same type where only one can be provided, only the last instance will be used."));
        }
      }
    }

    private bool IsParameterValueEmpty(KeyValuePair<string, StringValues> Item)
    {
      if (Item.Value.Count == 0)
      {
        this.InvalidParameterList.Add(new InvalidSearchQueryParameter(Item.Key, string.Empty, $"The parameter did not contain a value."));
        return true;
      }
      return false;
    }


    private bool TryParseSingleEnumTerm<EnumType>(KeyValuePair<string, StringValues> Item, string Term, out EnumType EnumValue)
      where EnumType : Enum
    {
      if (Item.Key == Term)
      {
        this.QueryItemProcessed = true;
        if (TryParseSingleEnum<EnumType>(Item, out EnumType ParsedEnumValue))
        {
          EnumValue = ParsedEnumValue;
          return true;
        }
      }
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
      EnumValue = default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
      return false;
    }

    private bool TryParseSingleEnum<EnumType>(KeyValuePair<string, StringValues> Item, out EnumType EnumValue)
      where EnumType : Enum
    {
      if (!IsParameterValueEmpty(Item))
      {
        CheckSingleParameterForMoreThanOne(Item);
        string Value = Item.Value[Item.Value.Count - 1];
        string ValueLower = StringSupport.ToLowerFast(Value);
        var Dic = StringToEnumMap<EnumType>.GetDictionary();
        if (Dic.ContainsKey(ValueLower))
        {
          EnumValue = Dic[ValueLower];
          return true;
        }
        else
        {
          this.InvalidParameterList.Add(new InvalidSearchQueryParameter(Item.Key, Value, $"Unable to parse the provided value to an allowed value."));
        }
      }
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
      EnumValue = default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
      return false;
    }

    private void ParseSortParameter(KeyValuePair<string, StringValues> Item)
    {
      if (Item.Key == TermSort)
      {
        this.QueryItemProcessed = true;
        foreach (var SortValue in Item.Value)
        {
          if (SortValue.StartsWith('-'))
          {
            this.Sort.Add(new FhirSearchQuery.SortParameter(SortOrder.Descending, SortValue.TrimStart('-')));
          }
          else
          {
            this.Sort.Add(new FhirSearchQuery.SortParameter(SortOrder.Descending, SortValue));
          }
        }
      }
    }

    private IList<string> ParseStringList(KeyValuePair<string, StringValues> Item, string Term)
    {
      var Result = new List<string>();
      if (Item.Key == Term)
      {
        this.QueryItemProcessed = true;
        foreach (var IncludeValue in Item.Value)
        {
          Result.Add(IncludeValue);
        }
      }
      return Result;
    }

    private int? TryParseIntegerParameter(KeyValuePair<string, StringValues> Item, string Term)
    {
      int? Result = null;
      if (Item.Key == Term)
      {
        this.QueryItemProcessed = true;
        foreach (var Value in Item.Value)
        {
          if (int.TryParse(Value.Trim(), out int CountInt))
          {
            Result = CountInt;
          }
          else
          {
            this.InvalidParameterList.Add(new InvalidSearchQueryParameter(Item.Key, Value, $"Unable to parse the value to an integer."));
          }
        }
      }
      return Result;
    }

    public class SortParameter
    {
      public SortOrder SortOrder { get; set; }
      public string SearchParameter { get; set; }
      public SortParameter(SortOrder SortOrder, string SearchParameter)
      {
        this.SortOrder = SortOrder;
        this.SearchParameter = SearchParameter;
      }
    }

    
  }
}
