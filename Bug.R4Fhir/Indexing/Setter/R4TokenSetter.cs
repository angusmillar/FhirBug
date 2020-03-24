using System;
using System.Collections.Generic;
using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using Bug.Common.StringTools;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;

namespace Bug.R4Fhir.Indexing.Setter
{
  public class R4TokenSetter : IR4TokenSetter
  {
    private ITypedElement? TypedElement;
    private Bug.Common.Enums.ResourceType ResourceType;
    private int SearchParameterId;
    private string? SearchParameterName;
    public R4TokenSetter() { }

    public IList<IndexToken> Set(ITypedElement typedElement, Bug.Common.Enums.ResourceType resourceType, int searchParameterId, string searchParameterName)
    {
      this.TypedElement = typedElement;
      this.ResourceType = resourceType;
      this.SearchParameterId = searchParameterId;
      this.SearchParameterName = searchParameterName;

      var ResourceIndexList = new List<IndexToken>();


      if (this.TypedElement is IFhirValueProvider FhirValueProvider && FhirValueProvider.FhirValue != null)
      {
        if (FhirValueProvider.FhirValue is Code Code)
        {
          SetCode(Code, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is CodeableConcept CodeableConcept)
        {
          SetCodeableConcept(CodeableConcept, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Coding Coding)
        {
          SetCoding(Coding, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue.TypeName == "code")
        {
          if (this.TypedElement.Value is string CodeValue)
            SetCodeTypeT(CodeValue, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is ContactPoint ContactPoint)
        {
          SetContactPoint(ContactPoint, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is FhirBoolean FhirBoolean)
        {
          SetFhirBoolean(FhirBoolean, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is FhirDateTime FhirDateTime)
        {
          SetFhirDateTime(FhirDateTime, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is FhirString FhirString)
        {
          SetFhirString(FhirString, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Id Id)
        {
          SetId(Id, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Identifier Identifier)
        {
          SetIdentifier(Identifier, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is PositiveInt PositiveInt)
        {
          SetPositiveInt(PositiveInt, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Quantity Quantity)
        {
          SetQuantity(Quantity, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Hl7.Fhir.Model.Range Range)
        {
          SetRange(Range, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Location.PositionComponent PositionComponent)
        {
          SePositionComponent(PositionComponent, ResourceIndexList);
        }
        else
        {
          throw new FormatException($"Unknown FhirType: {this.TypedElement.InstanceType} for the SearchParameter entity with the database key of: {this.SearchParameterId.ToString()} for a resource type of: {this.ResourceType.GetCode()} and search parameter name of: {this.SearchParameterName}");
        }
        return ResourceIndexList;
      }
      else if (this.TypedElement.Value is bool Bool)
      {
        var FhirBool = new FhirBoolean(Bool);
        SetFhirBoolean(FhirBool, ResourceIndexList);
      }
      else
      {
        throw new FormatException($"Unknown FhirType: {this.TypedElement.InstanceType} for the SearchParameter entity with the database key of: {this.SearchParameterId.ToString()} for a resource type of: {this.ResourceType.GetCode()} and search parameter name of: {this.SearchParameterName}");
      }

      return ResourceIndexList;
    }


    private void SetCodeTypeT(string CodeValue, IList<IndexToken> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(CodeValue))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, CodeValue));
      }
    }
    private void SePositionComponent(Location.PositionComponent PositionComponent, IList<IndexToken> ResourceIndexList)
    {
      if (PositionComponent.Latitude != null && PositionComponent.Latitude.HasValue && PositionComponent.Longitude != null && PositionComponent.Longitude.HasValue)
      {
        string Code = string.Join(":", PositionComponent.Latitude.Value, PositionComponent.Longitude.Value);
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, Code));
      }
      else if (PositionComponent.Latitude != null && PositionComponent.Latitude.HasValue)
      {
        string Code = string.Join(":", PositionComponent.Latitude.Value, string.Empty);
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, Code));
      }
      else if (PositionComponent.Longitude != null && PositionComponent.Longitude.HasValue)
      {
        string Code = string.Join(":", string.Empty, PositionComponent.Longitude.Value);
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, Code));
      }
    }
#pragma warning disable IDE0060 // Remove unused parameter
    private void SetRange(Hl7.Fhir.Model.Range range, IList<IndexToken> ResourceIndexList)
#pragma warning restore IDE0060 // Remove unused parameter
    {
      //There is no way to sensibly turn a Range into a Token type, so we just do nothing
      //and ignore setting the index. The reason this method is here is due to some search parameters
      //being a choice type where one of the choices is valid for token, like Boolean, yet others are 
      //not like Range as seen for the 'value' search parameter on the 'Group' resource .        
    }
    private void SetQuantity(Quantity Quantity, IList<IndexToken> ResourceIndexList)
    {
      if (Quantity.Value.HasValue && !string.IsNullOrWhiteSpace(Quantity.Unit))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(Quantity.Unit, Convert.ToString(Quantity.Value.Value)));
      }
      else if (Quantity.Value.HasValue)
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, Convert.ToString(Quantity.Value.Value)));
      }
      else if (!string.IsNullOrWhiteSpace(Quantity.Unit))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(Quantity.Unit, null));
      }
    }
    private void SetPositiveInt(PositiveInt PositiveInt, IList<IndexToken> ResourceIndexList)
    {
      if (PositiveInt.Value.HasValue)
      {
        string? AsString = Convert.ToString(PositiveInt.Value);
        if (AsString is object)
        {
          ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, AsString));
        }
      }
    }
    private void SetIdentifier(Identifier Identifier, IList<IndexToken> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(Identifier.Value) && !string.IsNullOrWhiteSpace(Identifier.System))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(Identifier.System, Identifier.Value));
      }
      else if (!string.IsNullOrWhiteSpace(Identifier.Value))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, Identifier.Value));
      }
      else if (!string.IsNullOrWhiteSpace(Identifier.System))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(Identifier.System, null));
      }
    }
    private void SetId(Id Id, IList<IndexToken> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(Id.Value))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, Id.Value));
      }
    }
    private void SetFhirString(FhirString FhirString, IList<IndexToken> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(FhirString.Value))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, FhirString.Value));
      }
    }
    private void SetFhirDateTime(FhirDateTime FhirDateTime, IList<IndexToken> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(FhirDateTime.Value))
      {
        if (Hl7.Fhir.Model.FhirDateTime.IsValidValue(FhirDateTime.Value))
        {
          ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, FhirDateTime.Value));
        }
      }
    }
    private void SetFhirBoolean(FhirBoolean FhirBoolean, IList<IndexToken> ResourceIndexList)
    {
      if (FhirBoolean.Value != null)
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, FhirBoolean.Value.ToString()));
      }
    }
    private void SetContactPoint(ContactPoint ContactPoint, IList<IndexToken> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(ContactPoint.Value) && (ContactPoint.System != null))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(ContactPoint.System.GetLiteral(), ContactPoint.Value));
      }
      else if (!string.IsNullOrWhiteSpace(ContactPoint.Value))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, ContactPoint.Value));
      }
      else if (ContactPoint.System != null)
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(ContactPoint.System.GetLiteral(), null));
      }
    }
    private void SetCoding(Coding Coding, IList<IndexToken> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(Coding.Code) && !string.IsNullOrWhiteSpace(Coding.System))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(Coding.System, Coding.Code));
      }
      else if (!string.IsNullOrWhiteSpace(Coding.Code))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, Coding.Code));
      }
      else if (!string.IsNullOrWhiteSpace(Coding.System))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(Coding.System, null));
      }
    }
    private void SetCodeableConcept(CodeableConcept CodeableConcept, IList<IndexToken> ResourceIndexList)
    {
      if (CodeableConcept.Coding.Count == 0)
      {
        if (!string.IsNullOrWhiteSpace(CodeableConcept.Text))
        {
          ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, CodeableConcept.Text));
        }
      }
      else
      {
        foreach (Coding Code in CodeableConcept.Coding)
        {
          SetCoding(Code, ResourceIndexList);
        }
      }
    }
    private void SetCode(Code Code, IList<IndexToken> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(Code.Value))
      {
        ResourceIndexList.Add(SetTokenIndexToLowerCaseTrim(null, Code.Value));
      }
    }


    private IndexToken SetTokenIndexToLowerCaseTrim(string? System, string? Code)
    {
      var ResourceIndex = new IndexToken(this.SearchParameterId);
      if (!string.IsNullOrWhiteSpace(System))
        ResourceIndex.System = StringSupport.ToLowerFast(System.Trim());

      if (!string.IsNullOrWhiteSpace(Code))
        ResourceIndex.Code = StringSupport.ToLowerFast(Code.Trim());

      return ResourceIndex;
    }
  }
}
