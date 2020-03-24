using System;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using Bug.Common.Dto.Indexing;
using Bug.Common.Enums;
using Bug.R4Fhir.Enums;

namespace Bug.R4Fhir.Indexing.Setter
{
  public class R4NumberSetter : IR4NumberSetter
  {
    private ITypedElement? TypedElement;
    private Bug.Common.Enums.ResourceType ResourceType;
    private int SearchParameterId;
    private string? SearchParameterName;
    private QuantityComparatorMap QuantityComparatorMap;

    public R4NumberSetter()
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
        if (FhirValueProvider.FhirValue is Integer Integer)
        {
          SetInteger(Integer, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is PositiveInt PositiveInt)
        {
          SetPositiveInt(PositiveInt, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Duration Duration)
        {
          SetDuration(Duration, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is FhirDecimal FhirDecimal)
        {
          SetFhirDecimal(FhirDecimal, ResourceIndexList);
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

    private void SetFhirDecimal(FhirDecimal FhirDecimal, IList<IndexQuantity> ResourceIndexList)
    {
      if (FhirDecimal.Value.HasValue)
      {
        var ResourceIndex = new IndexQuantity(this.SearchParameterId)
        {
          Quantity = FhirDecimal.Value,
          Comparator = null
        };
        ResourceIndexList.Add(ResourceIndex);
      }
    }

    private void SetRange(Hl7.Fhir.Model.Range Range, IList<IndexQuantity> ResourceIndexList)
    {
      if (Range.Low != null)
      {
        if (Range.Low.Value.HasValue)
        {
          var ResourceIndex = new IndexQuantity(this.SearchParameterId)
          {
            Quantity = Range.Low.Value,
            Comparator = this.QuantityComparatorMap.GetReverse(Quantity.QuantityComparator.GreaterOrEqual)
          };
          ResourceIndexList.Add(ResourceIndex);
        }
      }
      if (Range.High != null)
      {
        if (Range.High.Value.HasValue)
        {
          var ResourceIndex = new IndexQuantity(this.SearchParameterId)
          {
            Quantity = Range.High.Value,
            Comparator = this.QuantityComparatorMap.GetReverse(Quantity.QuantityComparator.LessOrEqual)
          };
          ResourceIndexList.Add(ResourceIndex);
        }
      }
    }

    private void SetDuration(Duration Duration, IList<IndexQuantity> ResourceIndexList)
    {
      if (Duration.Value.HasValue)
      {
        var ResourceIndex = new IndexQuantity(this.SearchParameterId)
        {
          Quantity = (decimal)Duration.Value
        };
        if (Duration.Comparator.HasValue)
          ResourceIndex.Comparator = this.QuantityComparatorMap.GetReverse(Duration.Comparator.Value);
        ResourceIndexList.Add(ResourceIndex);
      }
    }

    private void SetPositiveInt(PositiveInt PositiveInt, IList<IndexQuantity> ResourceIndexList)
    {
      if (PositiveInt.Value.HasValue)
      {
        if (PositiveInt.Value < 0)
          throw new FormatException(string.Format("PositiveInt must be a positive value, value was : {0}", PositiveInt.Value.ToString()));

        var ResourceIndex = new IndexQuantity(this.SearchParameterId)
        {
          Quantity = Convert.ToInt32(PositiveInt.Value),
          Comparator = null
        };
        ResourceIndexList.Add(ResourceIndex);
      }
    }

    private void SetInteger(Integer Integer, IList<IndexQuantity> ResourceIndexList)
    {
      if (Integer.Value.HasValue)
      {
        var ResourceIndex = new IndexQuantity(this.SearchParameterId);
        ResourceIndexList.Add(ResourceIndex);
        ResourceIndex.Quantity = Convert.ToInt32(Integer.Value);
        ResourceIndex.Comparator = null;
        ResourceIndexList.Add(ResourceIndex);
      }
    }
  }
}
