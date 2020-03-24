using System;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using Bug.Common.StringTools;
using Bug.Common.DatabaseTools;

namespace Bug.R4Fhir.Indexing.Setter
{
  public class R4StringSetter : IR4StringSetter
  {
    private ITypedElement? TypedElement;
    private Bug.Common.Enums.ResourceType ResourceType;
    private int SearchParameterId;
    private string? SearchParameterName;

    private const string ItemDelimeter = " ";
    public R4StringSetter() { }

    public IList<IndexString> Set(ITypedElement typedElement, Bug.Common.Enums.ResourceType resourceType, int searchParameterId, string searchParameterName)
    {
      this.TypedElement = typedElement;
      this.ResourceType = resourceType;
      this.SearchParameterId = searchParameterId;
      this.SearchParameterName = searchParameterName;

      var ResourceIndexList = new List<IndexString>();

      if (this.TypedElement is IFhirValueProvider FhirValueProvider && FhirValueProvider.FhirValue != null)
      {
        if (FhirValueProvider.FhirValue is FhirString FhirString)
        {
          SetFhirString(FhirString, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Address address)
        {
          SetAddress(address, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is HumanName HumanName)
        {
          SetHumanName(HumanName, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Markdown Markdown)
        {
          SetMarkdown(Markdown, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Annotation Annotation)
        {
          SetAnnotation(Annotation, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Base64Binary Base64Binary)
        {
          //No good purpose to index base64 content as a search index          
        }
        else
        {
          throw new FormatException($"Unknown FhirType: {this.TypedElement.InstanceType} for the SearchParameter entity with the database key of: {this.SearchParameterId.ToString()} for a resource type of: {this.ResourceType.GetCode()} and search parameter name of: {this.SearchParameterName}");
        }
      }
      else if (this.TypedElement.Value is bool Bool)
      {
        ResourceIndexList.Add(new IndexString(this.SearchParameterId, LowerTrimRemoveDiacriticsAndTruncate(Bool.ToString())));
      }
      else
      {
        throw new FormatException($"Unknown Navigator FhirType: {this.TypedElement.InstanceType} for the SearchParameter entity with the database key of: {this.SearchParameterId.ToString()} for a resource type of: {this.ResourceType.GetCode()} and search parameter name of: {this.SearchParameterName}");
      }

      return ResourceIndexList;
    }

    private void SetFhirString(FhirString FhirString, IList<IndexString> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(FhirString.Value))
      {
        ResourceIndexList.Add(new IndexString(this.SearchParameterId, LowerTrimRemoveDiacriticsAndTruncate(FhirString.Value)));
      }
    }
    private void SetAnnotation(Annotation Annotation, IList<IndexString> ResourceIndexList)
    {
      if (Annotation.Text is object)
      {
        if (!string.IsNullOrWhiteSpace(Annotation.Text.Value))
        {
          ResourceIndexList.Add(new IndexString(this.SearchParameterId, LowerTrimRemoveDiacriticsAndTruncate(Annotation.Text.Value)));
        }
      }
    }
    private void SetMarkdown(Markdown Markdown, IList<IndexString> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(Markdown.Value))
      {
        ResourceIndexList.Add(new IndexString(this.SearchParameterId, LowerTrimRemoveDiacriticsAndTruncate(Markdown.Value)));
      }
    }
    private void SetHumanName(HumanName HumanName, IList<IndexString> ResourceIndexList)
    {
      string FullName = string.Empty;
      foreach (var Given in HumanName.Given)
      {
        FullName += Given + ItemDelimeter;
      }

      if (!string.IsNullOrWhiteSpace(HumanName.Family))
      {
        FullName += HumanName.Family + ItemDelimeter;
      }

      if (FullName != string.Empty)
      {
        ResourceIndexList.Add(new IndexString(this.SearchParameterId, LowerTrimRemoveDiacriticsAndTruncate(FullName)));
      }
    }
    private void SetAddress(Address Address, IList<IndexString> ResourceIndexList)
    {
      string FullAdddress = string.Empty;
      foreach (var Line in Address.Line)
      {
        FullAdddress += Line + ItemDelimeter;
      }
      if (!string.IsNullOrWhiteSpace(Address.City))
      {
        FullAdddress += Address.City + ItemDelimeter;
      }
      if (!string.IsNullOrWhiteSpace(Address.PostalCode))
      {
        FullAdddress += Address.PostalCode + ItemDelimeter;
      }
      if (!string.IsNullOrWhiteSpace(Address.State))
      {
        FullAdddress += Address.State + ItemDelimeter;
      }
      if (!string.IsNullOrWhiteSpace(Address.Country))
      {
        FullAdddress += Address.Country + ItemDelimeter;
      }
      if (FullAdddress != string.Empty)
      {
        ResourceIndexList.Add(new IndexString(this.SearchParameterId, LowerTrimRemoveDiacriticsAndTruncate(FullAdddress)));
      }
    }

    private string LowerTrimRemoveDiacriticsAndTruncate(string item)
    {
      return StringSupport.ToLowerTrimRemoveDiacriticsTruncate(item, DatabaseMetaData.FieldLength.StringMaxLength);
    }
  }
}
