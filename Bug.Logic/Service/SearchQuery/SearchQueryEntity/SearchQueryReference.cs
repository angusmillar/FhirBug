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

    public override void ParseValue(string Values)
    {
      this.IsValid = true;
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
            this.IsValid = false;
            break;
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
              this.IsValid = false;
              break;
            }
          }
          else if (IFhirUriFactory.TryParse(Value.Trim(), this.FhirVersionId, out IFhirUri? FhirUri, out string ErrorMessage))
          {
            if (string.IsNullOrWhiteSpace(FhirUri!.ResourseName))
            {
              //After parsing the search parameter of type reference if there is no Resource Type for the reference
              //e.g no Patient is the example 'Patient/123456'
              string RefResources = string.Empty;
              this.TargetResourceTypeList.ToList().ForEach(v => RefResources += ", " + v.ResourceTypeId.GetCode());
              string ResourceName = this.ResourceContext.GetCode();
              string Message = string.Empty;
              Message = $"The search parameter: {this.Name} is ambiguous. ";
              Message += $"Additional information: ";
              Message += $"The search parameter: {this.Name} can reference the following resource types ({RefResources.TrimStart(',').Trim()}). ";
              Message += $"To correct this you must prefix the search parameter with a Type modifier, for example: {this.Name}={this.TargetResourceTypeList.ToArray()[0].ResourceTypeId.GetCode()}/{FhirUri.ResourceId}";
              Message += $"or: {this.Name}:{this.TargetResourceTypeList.ToArray()[0].ResourceTypeId.GetCode()}={FhirUri.ResourceId} ";
              Message += $"If the: {this.TargetResourceTypeList.ToArray()[0].ResourceTypeId.GetCode()} resource was the intended reference for the search parameter: {this.Name}.";
              this.IsValid = false;
              break;
            }

            //Most likely an absolute or relative so just parse it 
            SearchQueryReferenceValue = new SearchQueryReferenceValue(false, FhirUri);
          }
          else
          {
            this.InvalidMessage = $"Unable to parse the reference search parameter of: '{Value}'. {ErrorMessage}";
            this.IsValid = false;
            break;
          }

          if (SearchQueryReferenceValue is object && SearchQueryReferenceValue.FhirUri is object)
          {
            //Check the Resource type we resolved to is allowed for the search parameter            
            if (!string.IsNullOrWhiteSpace(SearchQueryReferenceValue.FhirUri.ResourseName) && !this.TargetResourceTypeList.Any(x => x.ResourceTypeId.GetCode() == SearchQueryReferenceValue.FhirUri.ResourseName))
            {
              this.InvalidMessage = $"The resource name used in the reference search parameter is not allowed for this search parameter type against this resource type.";
              this.IsValid = false;
              break;
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

      if (ValueList.Count > 1)
      {
        this.HasLogicalOrProperties = true;
      }

      if (this.ValueList.Count == 0)
      {
        if (!this.IsChained)
        {
          this.InvalidMessage = $"Unable to parse any values into a {this.GetType().Name} from the string: {Values}.";
          this.IsValid = false;
        }        
      }

    }

    //public override bool ParseValue(string Values)
    //{
    //  this.ValueList = new List<SearchQueryReferenceValue>();
    //  foreach (string Value in Values.Split(OrDelimiter))
    //  {
    //    if (this.Modifier.HasValue && this.Modifier == SearchModifierCode.Missing)
    //    {
    //      bool? IsMissing = SearchQueryReferenceValue.ParseModifierEqualToMissing(Value);
    //      if (IsMissing.HasValue)
    //      {
    //        this.ValueList.Add(new SearchQueryReferenceValue(IsMissing.Value, null));
    //      }
    //      else
    //      {
    //        this.InvalidMessage = $"Found the {SearchModifierCode.Missing.GetCode()} Modifier yet is value was expected to be true or false yet found '{Value}'. ";
    //        return false;
    //      }
    //    }
    //    else if (!this.IsChained) // If IsChained then there is no value to check
    //    {
    //      SearchQueryReferenceValue? SearchQueryReferenceValue = null;
    //      if (!Value.Contains('/') && !string.IsNullOrWhiteSpace(Value.Trim()) && this.TargetResourceTypeList.Count() == 1)
    //      {
    //        //If only one allowed Resource type then use this so the reference is just a resource Id '10' and we add on the appropriate ResourceName i.e 'Patient/10'
    //        string ParseValue = $"{this.TargetResourceTypeList.ToArray()[0].ResourceTypeId.GetCode()}/{Value.Trim()}";
    //        if (IFhirUriFactory.TryParse(ParseValue, this.FhirVersionId, out IFhirUri? FhirUri, out string ErrorMessage))
    //        {
    //          SearchQueryReferenceValue = new SearchQueryReferenceValue(false, FhirUri);
    //        }
    //        else
    //        {
    //          this.InvalidMessage = $"The resource reference was {Value} and the only allowed resource type was appended to give {ParseValue} however parsing of this as a FHIR reference failed with the error: {ErrorMessage}";
    //          return false;
    //        }
    //      }
    //      else if (IFhirUriFactory.TryParse(Value.Trim(), this.FhirVersionId, out IFhirUri? FhirUri, out string ErrorMessage))
    //      {
    //        //Most likely an absolute or relative so just parse it 
    //        SearchQueryReferenceValue = new SearchQueryReferenceValue(false, FhirUri);
    //      }
    //      else
    //      {
    //        this.InvalidMessage = $"Unable to parse the reference search parameter of: '{Value}'. {ErrorMessage}";
    //        return false;
    //      }

    //      if (SearchQueryReferenceValue is object && SearchQueryReferenceValue.FhirUri is object)
    //      {
    //        //Check the Resource type we resolved to is allowed for the search parameter            
    //        if (!string.IsNullOrWhiteSpace(SearchQueryReferenceValue.FhirUri.ResourseName) && !this.TargetResourceTypeList.Any(x => x.ResourceTypeId.GetCode() == SearchQueryReferenceValue.FhirUri.ResourseName))            
    //        {
    //          this.InvalidMessage = $"The resource name used in the reference search parameter is not allowed for this search parameter type against this resource type.";
    //          return false;
    //        }
    //        else
    //        {
    //          this.ValueList.Add(SearchQueryReferenceValue);
    //        }
    //      }
    //      else
    //      {
    //        throw new ArgumentNullException($"Internal Server Error: Either {nameof(SearchQueryReferenceValue)} or {nameof(SearchQueryReferenceValue.FhirUri)} was found to be null");
    //      }
    //    }
    //  }

    //  if (this.ValueList.Count() > 1)
    //    this.HasLogicalOrProperties = true;

    //  if (this.IsChained || this.ValueList.Count > 0)
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
