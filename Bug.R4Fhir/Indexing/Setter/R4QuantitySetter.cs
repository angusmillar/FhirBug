using System;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using Hl7.Fhir.Utility;
using Bug.Common.Dto.Indexing;
using Bug.R4Fhir.Enums;
using Bug.Common.Enums;

namespace Bug.R4Fhir.Indexing.Setter
{
  public class R4QuantitySetter : IR4QuantitySetter
  {
    private ITypedElement? TypedElement;
    private Bug.Common.Enums.ResourceType ResourceType;
    private int SearchParameterId;
    private string? SearchParameterName;
    private QuantityComparatorMap QuantityComparatorMap;

    public R4QuantitySetter()
    {
      this.QuantityComparatorMap = new QuantityComparatorMap();
    }

    public IList<IndexQuantity> Set(ITypedElement typedElement, Bug.Common.Enums.ResourceType resourceType, int searchParameterId, string searchParameterName)
    {
      this.TypedElement = typedElement;
      this.ResourceType = resourceType;
      this.SearchParameterId = searchParameterId;
      this.SearchParameterName = searchParameterName;

      var ResourceIndexList = new List<IndexQuantity>();

      if (this.TypedElement is IFhirValueProvider FhirValueProvider && FhirValueProvider.FhirValue != null)
      {
        if (FhirValueProvider.FhirValue is Money Money)
        {
          SetMoney(Money, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is SimpleQuantity SimpleQuantity)
        {
          SetSimpleQuantity(SimpleQuantity, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Quantity Quantity)
        {
          SetQuantity(Quantity, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Location.PositionComponent PositionComponent)
        {
          SetPositionComponent(PositionComponent, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Hl7.Fhir.Model.Range Range)
        {
          SetRange(Range, ResourceIndexList);
        }
        else
        {
          throw new FormatException($"Unknown FhirType: {this.TypedElement.InstanceType} for the SearchParameter entity with the database key of: {this.SearchParameterId.ToString()} for a resource type of: {this.ResourceType.GetCode()} and search parameter name of: {this.SearchParameterName}");
        }
        return ResourceIndexList;
      }
      else
      {
        throw new FormatException($"Unknown Navigator FhirType: {this.TypedElement.InstanceType} for the SearchParameter entity with the database key of: {this.SearchParameterId.ToString()} for a resource type of: {this.ResourceType.GetCode()} and search parameter name of: {this.SearchParameterName}");
      }
    }

    private void SetRange(Hl7.Fhir.Model.Range Range, IList<IndexQuantity> ResourceIndexList)
    {
      //If either value is missing then their is no range as the Range data type uses SimpleQuantity 
      //which has no Comparator property. Therefore there is no such thing as >10 or <100, their must be to values
      // for examples 10 - 100. 
      if (Range.High.Value.HasValue && Range.Low.Value.HasValue)
      {
        var ResourceIndex = new IndexQuantity(this.SearchParameterId);
        //Set the low end of range
        MainQuantitySetter(Range.Low, ResourceIndex);

        //Set the High end of range Manually        
        ResourceIndex.QuantityHigh = Range.High.Value;
        ResourceIndex.CodeHigh = string.IsNullOrWhiteSpace(Range.High.Code) ? null : Range.High.Code;
        ResourceIndex.SystemHigh = string.IsNullOrWhiteSpace(Range.High.System) ? null : Range.High.System;
        if (Range.High.Comparator.HasValue)
          ResourceIndex.ComparatorHigh = this.QuantityComparatorMap.GetReverse(Range.High.Comparator.Value);
        ResourceIndex.UnitHigh = string.IsNullOrWhiteSpace(Range.High.Unit) ? null : Range.High.Unit;

        ResourceIndexList.Add(ResourceIndex);
      }
    }
    private void SetPositionComponent(Location.PositionComponent PositionComponent, IList<IndexQuantity> ResourceIndexList)
    {
      //todo:
      //The only Quantity for Location.PositionComponent is in the Location resource and it's use is a little odd.
      //You never actual store a 'near-distance' search parameter as an index but rather it is used in conjunction with the 
      //'near' search parameter. 
      //for instance the search would be like this:
      //GET [base]/Location?near=-83.694810:42.256500&near-distance=11.20||km...
      //Where we need to work out the distance say in km between 'near' [latitude]:[longitude] we have stored in the db index and the [latitude]:[longitude] given in the search url's 'near'.
      //If that distance is less then or equal to the  'near-distance' given in the search Url (11.20km here) then return the resource.   
      //Update: Talked to Brian Pos and I can see I do need to store this as it's own index. SQL has a geography datatype which needs to be used
      //See ref: https://docs.microsoft.com/en-us/sql/t-sql/spatial-geography/spatial-types-geography
      // I also have some of Brians code as an example in NOTES, search for 'Brian's FHIR position longitude latitude code' in FHIR notebook
      //I think this will have to be a special case, maybe not a noraml Pyro FHIR token index but another, or maybe add it to the Token index yet it will be null 99% of time. 
      //More thinking required. At present the server never indexes this so the search never finds it. Not greate!
    }
    private void SetQuantity(Quantity Quantity, IList<IndexQuantity> ResourceIndexList)
    {
      var ResourceIndex = new IndexQuantity(this.SearchParameterId);
      MainQuantitySetter(Quantity, ResourceIndex);
      ResourceIndexList.Add(ResourceIndex);
    }
    private void SetSimpleQuantity(SimpleQuantity SimpleQuantity, IList<IndexQuantity> ResourceIndexList)
    {
      var ResourceIndex = new IndexQuantity(this.SearchParameterId);
      MainQuantitySetter(SimpleQuantity, ResourceIndex);
      ResourceIndexList.Add(ResourceIndex);
    }

    private void SetMoney(Money Money, IList<IndexQuantity> ResourceIndexList)
    {
      var ResourceIndex = new IndexQuantity(this.SearchParameterId)
      {
        Quantity = Money.Value
      };
      if (Money.Currency.HasValue)
      {
        ResourceIndex.Code = Money.Currency.Value.GetLiteral();
        ResourceIndex.System = "urn:iso:std:iso:4217";
      }
      ResourceIndexList.Add(ResourceIndex);
    }

    private void MainQuantitySetter(Quantity Quantity, IndexQuantity ResourceIndex)
    {
      ResourceIndex.Quantity = Quantity.Value;
      ResourceIndex.Code = string.IsNullOrWhiteSpace(Quantity.Code) ? null : Quantity.Code;
      ResourceIndex.System = string.IsNullOrWhiteSpace(Quantity.System) ? null : Quantity.System;
      if (Quantity.Comparator.HasValue)
        ResourceIndex.Comparator = this.QuantityComparatorMap.GetReverse(Quantity.Comparator.Value);
      ResourceIndex.Unit = string.IsNullOrWhiteSpace(Quantity.Unit) ? null : Quantity.Unit;
    }

  }
}
