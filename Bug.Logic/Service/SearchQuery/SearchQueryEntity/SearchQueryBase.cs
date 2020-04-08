using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.FhirTools.SearchQuery;
using Bug.Common.Interfaces.DomainModel;
using Bug.Common.StringTools;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.Fhir;
using Bug.Logic.Service.SearchQuery.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public abstract class SearchQueryBase : SearchParameter, ISearchQueryBase
  {
    protected const char OrDelimiter = ',';
    protected const char CompositeDelimiter = '$';
    protected const char ParameterValueDelimiter = '=';
    public string RawValue { get; set; }
    public Bug.Common.Enums.SearchModifierCode? Modifier { get; set; }
    public string TypeModifierResource { get; set; }
    public ISearchQueryBase? ChainedSearchParameter { get; set; }
    public bool HasLogicalOrProperties { get; set; }
    public bool IsValid { get; set; }
    public string InvalidMessage { get; set; }
    public IServiceBaseUrl? PrimaryServiceBaseUrl { get; set; }
    public Bug.Common.Enums.ResourceType ResourceContext { get; set; }

    public SearchQueryBase(SearchParameter SearchParameter, Bug.Common.Enums.ResourceType ResourceContext, string RawValue)
    {
      this.ResourceContext = ResourceContext;
      this.Id = SearchParameter.Id;
      this.Name = SearchParameter.Name;
      this.Description = SearchParameter.Description;
      this.SearchParamTypeId = SearchParameter.SearchParamTypeId;
      this.Url = SearchParameter.Url;
      this.FhirPath = SearchParameter.FhirPath;
      this.ResourceTypeList = SearchParameter.ResourceTypeList;
      this.TargetResourceTypeList = SearchParameter.TargetResourceTypeList;
      this.ComponentList = SearchParameter.ComponentList;
      this.FhirVersionId = SearchParameter.FhirVersionId;      
      this.RawValue = RawValue;

      this.TypeModifierResource = string.Empty;
      this.InvalidMessage = string.Empty;
      this.HasLogicalOrProperties = false;
      this.IsValid = true;
    }
    public virtual void ParseModifier(string parameterName, IResourceTypeSupport IResourceTypeSupport, IKnownResource IKnownResource)
    {
      if (parameterName.Contains(FhirSearchQuery.TermSearchModifierDelimiter))
      {
        string parameterNameModifierPart = parameterName.Split(FhirSearchQuery.TermSearchModifierDelimiter)[1];
        var SearchModifierTypeDic = StringToEnumMap<Common.Enums.SearchModifierCode>.GetDictionary();
        string ValueCaseCorrectly = StringSupport.ToLowerFast(parameterNameModifierPart);
        if (SearchModifierTypeDic.ContainsKey(ValueCaseCorrectly))
        {
          this.Modifier = SearchModifierTypeDic[ValueCaseCorrectly];          
        }
        else
        {
          string TypedResourceName = parameterNameModifierPart;
          if (parameterNameModifierPart.Contains("."))
          {
            char[] delimiters = { '.' };
            TypedResourceName = parameterNameModifierPart.Split(delimiters)[0].Trim();
          }

          if (IKnownResource.IsKnownResource(this.FhirVersionId, TypedResourceName))
          {
            Common.Enums.ResourceType? ResourceType = IResourceTypeSupport.GetTypeFromName(TypedResourceName);
            if (ResourceType != null)
            {
              this.TypeModifierResource = TypedResourceName;
              this.Modifier = SearchModifierCode.Type;              
            }
            else
            {
              throw new ApplicationException($"Found a known resource to the FHIR API yet this resource was not found in the Enum list for {typeof(Common.Enums.ResourceType).Name}");
            }
          }
          else
          {
            this.InvalidMessage = $"Unable to parse the given search parameter's Modifier: {parameterName}, ";
            this.IsValid = false;
          }

        }
      }
      else
      {
        this.Modifier = null;
        this.TypeModifierResource = string.Empty;        
      }

      if (this.Modifier.HasValue)
      {
        SearchModifierCode[] oSupportedModifierArray = SearchQuerySupport.GetModifiersForSearchType(this.SearchParamTypeId);
        if (!oSupportedModifierArray.Any(x => x == this.Modifier.Value))
        {
          this.InvalidMessage += $"The parameter's modifier: '{this.Modifier.GetCode()}' is not supported by this server for this search parameter type '{this.SearchParamTypeId.GetCode()}', the whole parameter was : '{this.RawValue}', ";
          this.IsValid = false;
        }

      }
    }
    public abstract void ParseValue(string Value);    
    public abstract object CloneDeep();
    public virtual object CloneDeep(SearchQueryBase Clone)
    {
      //Base SearchParameter class
      Clone.Id = this.Id;
      Clone.Name = this.Name;
      Clone.Description = this.Description;
      Clone.SearchParamType = this.SearchParamType;
      Clone.SearchParamTypeId = this.SearchParamTypeId;
      Clone.Url = this.Url;
      Clone.FhirPath = this.FhirPath;
      if (this.ResourceTypeList is object)
      {
        Clone.ResourceTypeList = new List<SearchParameterResourceType>();
        this.ResourceTypeList.ToList().ForEach(x => Clone.ResourceTypeList.Add(
          new SearchParameterResourceType()
          {
            Id = x.Id,
            ResourceTypeId = x.ResourceTypeId,
            SearchParameterId = x.SearchParameterId,
            Created = x.Created,
            Updated = x.Updated
          }));        
      }
      if (this.TargetResourceTypeList != null)
      {
        Clone.TargetResourceTypeList = new List<SearchParameterTargetResourceType>();
        this.TargetResourceTypeList.ToList().ForEach(x => Clone.TargetResourceTypeList.Add(
          new SearchParameterTargetResourceType()
          {
            Id = x.Id,
            ResourceTypeId = x.ResourceTypeId,
            SearchParameterId = x.SearchParameterId,
            Created = x.Created,
            Updated = x.Updated
          }));        
      }
      if (this.ComponentList != null)
      {
        Clone.ComponentList = new List<SearchParameterComponent>();
        this.ComponentList.ToList().ForEach(x => Clone.ComponentList.Add(
          new SearchParameterComponent()
          {
            Id = x.Id,
            Definition = x.Definition,
            Expression = x.Expression,
          }));        
      }
      Clone.FhirVersionId = this.FhirVersionId;
      Clone.FhirVersion = this.FhirVersion;

      //This class
      Clone.RawValue = this.RawValue;
      Clone.Modifier = this.Modifier;
      Clone.TypeModifierResource = this.TypeModifierResource;
      Clone.ChainedSearchParameter = this.ChainedSearchParameter;
      Clone.HasLogicalOrProperties = this.HasLogicalOrProperties;
      Clone.IsValid = this.IsValid;
      Clone.InvalidMessage = this.InvalidMessage;
      Clone.PrimaryServiceBaseUrl = this.PrimaryServiceBaseUrl;

      return Clone;
    }


  }
}
