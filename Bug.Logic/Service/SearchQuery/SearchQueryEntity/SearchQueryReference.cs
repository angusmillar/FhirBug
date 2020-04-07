using Bug.Common.Enums;
using Bug.Common.Interfaces;
using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public class SearchQueryReference : SearchQueryBase
  {
    private readonly IFhirUriFactory IFhirUriFactory;
    public SearchQueryReference(SearchParameter SearchParameter, Bug.Common.Enums.ResourceType ResourceContext, IFhirUriFactory IFhirUriFactory, string RawValue, bool IsChained)
      : base(SearchParameter, ResourceContext, RawValue)
    {
      this.IFhirUriFactory = IFhirUriFactory;
      this.SearchParamTypeId = Bug.Common.Enums.SearchParamType.Reference;
      this.IsChained = IsChained;
      this.ValueList = new List<SearchQueryReferenceValue>();      
    }

    public List<SearchQueryReferenceValue> ValueList { get; set; }
    public bool IsChained { get; set; }    

    public override object CloneDeep()
    {
      var Clone = new SearchQueryReference(this as SearchParameter, this.ResourceContext, IFhirUriFactory, this.RawValue, this.IsChained);
      base.CloneDeep(Clone);
      Clone.ValueList = new List<SearchQueryReferenceValue>();
      Clone.ValueList.AddRange(this.ValueList);      
      return Clone;
    }

    public override bool TryParseValue(string Values)
    {
      this.ValueList = new List<SearchQueryReferenceValue>();
      foreach (string Value in Values.Split(OrDelimiter))
      {
        if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
        {
          bool? IsMissing = SearchQueryReferenceValue.ParseModifierEqualToMissing(Value);
          if (IsMissing.HasValue)
          {
            this.ValueList.Add(new SearchQueryReferenceValue(IsMissing.Value, null));
          }
          else
          {
            this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet is value was expected to be true or false yet found '{Value}'. ";
            return false;
          }
        }
        else if (!this.IsChained) // If IsChained then there is no value to check
        {
          SearchQueryReferenceValue? SearchQueryReferenceValue = null;
          if (!Value.Contains('/') && !string.IsNullOrWhiteSpace(Value.Trim()) && this.TargetResourceTypeList.Count() == 1)
          {
            //If only one allowed Resource type then use this so the reference is just a resource Id '10' and we add on the appropriate ResourceName i.e 'Patient/10'
            string ParseValue = $"{this.TargetResourceTypeList.ToArray()[0].ResourceTypeId.GetCode()}/{Value.Trim()}";
            if (IFhirUriFactory.TryParse(ParseValue, this.FhirVersionId, out IFhirUri? FhirUri, out string ErrorMessage))
            {
              SearchQueryReferenceValue = new SearchQueryReferenceValue(false, FhirUri);
            }
            else
            {
              this.InvalidMessage = $"The resource reference was {Value} and the only allowed resource type was appended to give {ParseValue} however parsing of this as a FHIR reference failed with the error: {ErrorMessage}";
              return false;
            }
          }
          else if (IFhirUriFactory.TryParse(Value.Trim(), this.FhirVersionId, out IFhirUri? FhirUri, out string ErrorMessage))
          {
            //Most likely an absolute or relative so just parse it 
            SearchQueryReferenceValue = new SearchQueryReferenceValue(false, FhirUri);
          }
          else
          {
            this.InvalidMessage = $"Unable to parse the reference search parameter of: '{Value}'. {ErrorMessage}";
            return false;
          }

          if (SearchQueryReferenceValue is object && SearchQueryReferenceValue.FhirUri is object)
          {
            //Check the Resource type we resolved to is allowed for the search parameter            
            if (!string.IsNullOrWhiteSpace(SearchQueryReferenceValue.FhirUri.ResourseName) && !this.TargetResourceTypeList.Any(x => x.ResourceTypeId.GetCode() == SearchQueryReferenceValue.FhirUri.ResourseName))            
            {
              this.InvalidMessage = $"The resource name used in the reference search parameter is not allowed for this search parameter type against this resource type.";
              return false;
            }
            else
            {
              this.ValueList.Add(SearchQueryReferenceValue);
            }
          }
          else
          {
            throw new ArgumentNullException($"Internal Server Error: Either {nameof(SearchQueryReferenceValue)} or {nameof(SearchQueryReferenceValue.FhirUri)} was found to be null");
          }
        }
      }

      if (this.ValueList.Count() > 1)
        this.HasLogicalOrProperties = true;

      if (this.IsChained || this.ValueList.Count > 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

  }
}
