using Bug.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Bug.Logic.DomainModel
{ 
  public class ResourceStore : BaseIntKey
  {
    public string ResourceId { get; set; }
    public string? ContainedId { get; set; }
    public int VersionId { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime LastUpdated { get; set; }
    public byte[]? ResourceBlob { get; set; }
    public Common.Enums.ResourceType ResourceTypeId { get; set; }
    public ResourceType ResourceType { get; set; }
    public Common.Enums.FhirVersion FhirVersionId { get; set; }
    public FhirVersion FhirVersion { get; set; }
    public HttpVerb MethodId { get; set; }
    public Method Method { get; set; }
    public int HttpStatusCodeId { get; set; }
    public HttpStatusCode HttpStatusCode { get; set; }
    public ICollection<IndexDateTime> DateTimeIndexList { get; set; }
    public ICollection<IndexQuantity> QuantityIndexList { get; set; }
    public ICollection<IndexReference> ReferenceIndexList { get; set; }
    public ICollection<IndexString> StringIndexList { get; set; }
    public ICollection<IndexToken> TokenIndexList { get; set; }
    public ICollection<IndexUri> UriIndexList { get; set; }

  }
}
