using System.Collections.Generic;

namespace Bug.Logic.DomainModel
{
  public interface ISearchParameter
  {
    ICollection<SearchParameterComponent> ComponentList { get; set; }
    string Description { get; set; }
    string FhirPath { get; set; }
    FhirVersion FhirVersion { get; set; }
    Common.Enums.FhirVersion FhirVersionId { get; set; }
    string Name { get; set; }
    ICollection<SearchParameterResourceType> ResourceTypeList { get; set; }
    SearchParamType SearchParamType { get; set; }
    Common.Enums.SearchParamType SearchParamTypeId { get; set; }
    ICollection<SearchParameterTargetResourceType> TargetResourceTypeList { get; set; }
    string Url { get; set; }
  }
}