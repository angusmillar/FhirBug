using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Bug.Common.Enums;
using Bug.Common.StringTools;
using static Bug.Logic.Service.SearchQuery.SearchQueryEntity.SearchQueryInclude;

namespace Bug.Logic.Service.SearchQuery.Tools
{
  public class FhirSearchQuery : IFhirSearchQuery
  {
    public const string TermInclude = "_include";
    public const string TermRevInclude = "_revinclude";
    public const string TermIncludeIterate = "iterate";
    public const string TermIncludeRecurse = "recurse";
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
    public const string TermHas = "_has";

    public int? Count { get; set; }
    public int? Page { get; set; }
    public IList<IncludeParameter> RevInclude { get; set; }
    public IList<IncludeParameter> Include { get; set; }
    public IList<SortParameter> Sort { get; set; }
    public IList<HasParameter> Has { get; set; }
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
      this.RevInclude = new List<IncludeParameter>();
      this.Include = new List<IncludeParameter>();
      this.Sort = new List<SortParameter>();
      this.Has = new List<HasParameter>();
      this.ParametersDictionary = new Dictionary<string, StringValues>();
      this.QueryItemProcessed = false;
    }

    public bool Parse(Dictionary<string, StringValues> query)
    {
      this.InvalidParameterList = new List<InvalidSearchQueryParameter>();
      this.RevInclude = new List<IncludeParameter>();
      this.Include = new List<IncludeParameter>();
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

        //Include & RevInclude
        ParseIncludes(Item);

        //Sort
        ParseSortParameter(Item);

        //Has
        ParseHasParameter(Item);

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

    private void ParseHasParameter(KeyValuePair<string, StringValues> Item)
    {
      //GET [base]/Patient?_has:Observation:patient:code=1234-5
      //or
      //GET [base]/Patient?_has:Observation:patient:_has:AuditEvent:entity:user=MyUserId
      if (Item.Key.StartsWith($"{TermHas}{TermSearchModifierDelimiter}"))
      {
        this.QueryItemProcessed = true;
        var HasSplit = Item.Key.Split(TermHas);
        FhirSearchQuery.HasParameter? RootHasParameter = null;
        FhirSearchQuery.HasParameter? PreviousHasParameter = null;        
        for (int i = 1; i < HasSplit.Length; i++)
        {
          var ModifierSplit = HasSplit[i].Split(TermSearchModifierDelimiter);
          if (ModifierSplit.Length == 4 && RootHasParameter is null)
          {
            if (ModifierSplit[3] == string.Empty)
            {
              RootHasParameter = new HasParameter(ModifierSplit[1], ModifierSplit[2]);
              RootHasParameter.RawHasParameter = $"{Item.Key}={ Item.Value}";
              PreviousHasParameter = RootHasParameter;
            }
            else
            {
              RootHasParameter = new HasParameter(ModifierSplit[1], ModifierSplit[2]);
              RootHasParameter.SearchQuery = new KeyValuePair<string, StringValues>(ModifierSplit[3], Item.Value);
              RootHasParameter.RawHasParameter = $"{Item.Key}={ Item.Value}";
              PreviousHasParameter = RootHasParameter;
            }            
          }  
          else if (ModifierSplit.Length == 4 && RootHasParameter is object)
          {
            if (ModifierSplit[3] == string.Empty)
            {
              PreviousHasParameter!.ChildHasParameter = new HasParameter(ModifierSplit[1], ModifierSplit[2]);
              PreviousHasParameter = PreviousHasParameter!.ChildHasParameter;
            }
            else
            {
              PreviousHasParameter!.ChildHasParameter = new HasParameter(ModifierSplit[1], ModifierSplit[2]);
              PreviousHasParameter!.ChildHasParameter.SearchQuery = new KeyValuePair<string, StringValues>(ModifierSplit[3], Item.Value);
              PreviousHasParameter = PreviousHasParameter!.ChildHasParameter;
            }
          }
          else
          {
            this.InvalidParameterList.Add(new InvalidSearchQueryParameter(Item.Key, Item.Value, $"The {TermHas} query must contain a resource name followed by a reference search parameter name followed by another {TermHas} parameter or a search parameter and value where each is separated by a colon {TermSearchModifierDelimiter}. For instance: _has:Observation:patient:code=1234-5 or _has:Observation:patient:_has:AuditEvent:entity:user=MyUserId. The {TermHas} qery found was : {Item.Key}={Item.Value} "));
          }
        }

        if (RootHasParameter is object)
        {
          this.Has.Add(RootHasParameter);
        }
      }
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

    private void ParseIncludes(KeyValuePair<string, StringValues> Item)
    {

      if (Item.Key.Equals($"{TermInclude}{TermSearchModifierDelimiter}{TermIncludeRecurse}", StringComparison.Ordinal))
      {
        this.QueryItemProcessed = true;
        this.Include.Add(new IncludeParameter(false, true, IncludeType.Include, Item.Value));
      }
      else if (Item.Key.Equals($"{TermInclude}{TermSearchModifierDelimiter}{TermIncludeIterate}", StringComparison.Ordinal))
      {
        this.QueryItemProcessed = true;
        this.Include.Add(new IncludeParameter(true, false, IncludeType.Include, Item.Value));
      }
      else if (Item.Key.Equals($"{TermInclude}", StringComparison.Ordinal))
      {
        this.QueryItemProcessed = true;
        this.Include.Add(new IncludeParameter(false, false, IncludeType.Include, Item.Value));
      }
      else if (Item.Key.Equals($"{TermRevInclude}{TermSearchModifierDelimiter}{TermIncludeRecurse}", StringComparison.Ordinal))
      {
        this.QueryItemProcessed = true;
        this.Include.Add(new IncludeParameter(false, true, IncludeType.Revinclude, Item.Value));
      }
      else if (Item.Key.Equals($"{TermRevInclude}{TermSearchModifierDelimiter}{TermIncludeIterate}", StringComparison.Ordinal))
      {
        this.QueryItemProcessed = true;
        this.Include.Add(new IncludeParameter(true, false, IncludeType.Revinclude, Item.Value));
      }
      else if (Item.Key.Equals($"{TermRevInclude}", StringComparison.Ordinal))
      {
        this.QueryItemProcessed = true;
        this.Include.Add(new IncludeParameter(false, false, IncludeType.Revinclude, Item.Value));
      }
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

    public class IncludeParameter
    {
      public IncludeType Type { get; set; }
      public bool Recurse { get; set; }
      public bool Iterate { get; set; }
      public string Value { get; set; }
      public IncludeParameter(bool Iterate, bool Recurse, IncludeType Type, string Value)
      {
        this.Type = Type;
        this.Iterate = Iterate;
        this.Recurse = Recurse;
        this.Value = Value;
      }
    }

    public class HasParameter
    {
      public string RawHasParameter { get; set; }
      public HasParameter? ChildHasParameter { get; set; }
      public string TargetResourceForSearchQuery { get; set; }
      public string BackReferenceSearchParameterName { get; set; }
      public KeyValuePair<string, StringValues>? SearchQuery { get; set; }

      public HasParameter(string TargetResource, string ReferenceSearchParameterName)
      {
        this.TargetResourceForSearchQuery = TargetResource;
        this.BackReferenceSearchParameterName = ReferenceSearchParameterName;
        this.RawHasParameter = string.Empty;
      }

    }

  }
}
