using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.DomainModel
{
  #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
  public class SearchParameter : BaseIntKey
  {    
    public string Name { get; set; }   
    public string Description { get; set; }
    public SearchParamType SearchParamType { get; set; }
    public Bug.Common.Enums.SearchParamType FkSearchParamTypeId { get; set; }
    public string Url { get; set; }
    public string FhirPath { get; set; }
    public ICollection<SearchParameterResourceName> ResourceNameList { get; set; }
    public ICollection<SearchParameterTargetResourceName> TargetResourceNameList { get; set; }
    public Common.Enums.FhirVersion FkFhirVersionId { get; set; }
    public FhirVersion FhirVersion { get; set; }
  }
}
