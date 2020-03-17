using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel
{
  public class SearchParameter : BaseIntKey
  {    
    public string Name { get; set; }   
    public string Description { get; set; }
    public SearchParamType SearchParamType { get; set; }
    public Bug.Common.Enums.SearchParamType FkSearchParamTypeId { get; set; }
    public string Url { get; set; }
    public string FhirPath { get; set; }
    public ICollection<SearchParameterResourceType> ResourceTypeList { get; set; }
    public ICollection<SearchParameterTargetResourceType> TargetResourceTypeList { get; set; }
    public ICollection<SearchParameterComponent> ComponentList { get; set; }
    public Common.Enums.FhirVersion FkFhirVersionId { get; set; }
    public FhirVersion FhirVersion { get; set; }
  }
}
