using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using Bug.Common.StringTools;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;

namespace Bug.R4Fhir.Indexing.Setter
{
  public class R4UriSetter : IR4UriSetter
  {
    private ITypedElement? TypedElement;
    private Bug.Common.Enums.ResourceType ResourceType;
    private int SearchParameterId;
    private string? SearchParameterName;
    public R4UriSetter() { }

    public IList<IndexUri> Set(ITypedElement typedElement, Bug.Common.Enums.ResourceType resourceType, int searchParameterId, string searchParameterName)
    {
      this.TypedElement = typedElement;
      this.ResourceType = resourceType;
      this.SearchParameterId = searchParameterId;
      this.SearchParameterName = searchParameterName;

      var ResourceIndexList = new List<IndexUri>();

      if (this.TypedElement is IFhirValueProvider FhirValueProvider && FhirValueProvider.FhirValue != null)
      {
        if (FhirValueProvider.FhirValue is FhirUri FhirUri)
        {
          SetUri(FhirUri, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is FhirUrl FhirUrl)
        {
          SetUrl(FhirUrl, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Oid Oid)
        {
          SetOid(Oid, ResourceIndexList);
        }
        else
        {
          throw new FormatException($"Unknown FhirType: {this.TypedElement.InstanceType} for the SearchParameter entity with the database key of: {this.SearchParameterId.ToString()} for a resource type of: {this.ResourceType.GetCode()} and search parameter name of: {this.SearchParameterName}");
        }
        return ResourceIndexList;
      }
      else
      {
        throw new FormatException($"Unknown FhirType: {this.TypedElement.InstanceType} for the SearchParameter entity with the database key of: {this.SearchParameterId.ToString()} for a resource type of: {this.ResourceType.GetCode()} and search parameter name of: {this.SearchParameterName}");
      }
    }

    private void SetOid(Oid Oid, IList<IndexUri> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(Oid.Value))
      {

        ResourceIndexList.Add(new IndexUri(this.SearchParameterId, Oid.Value));
      }
    }
    private void SetUri(FhirUri FhirUri, IList<IndexUri> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(FhirUri.Value))
      {
        ResourceIndexList.Add(new IndexUri(this.SearchParameterId, StringSupport.ToLowerFast(FhirUri.Value.StripHttp())));
      }
    }

    private void SetUrl(FhirUrl FhirUrl, IList<IndexUri> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(FhirUrl.Value))
      {
        ResourceIndexList.Add(new IndexUri(this.SearchParameterId, StringSupport.ToLowerFast(FhirUrl.Value.StripHttp())));
      }
    }
  }
}
