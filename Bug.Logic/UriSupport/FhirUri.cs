using Bug.Common.ApplicationConfig;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.StringTools;
using Bug.Logic.Interfaces.CompositionRoot;
using System;
using System.Linq;

#nullable disable
namespace Bug.Logic.UriSupport
{
  public enum UrnType { uuid, oid };

  public class FhirUri : IFhirUri
  {
    public string ParseErrorMessage { get; set; }
    public bool ErrorInParseing { get; set; }
    public string ResourseName { get; set; }
    public string CompartmentalisedResourseName { get; set; }
    public string ResourceId { get; set; }
    public string VersionId { get; set; }
    public string OperationName { get; set; }
    public string Query { get; set; }
    public string OriginalString { get; set; }
    public bool IsUrn { get; set; }
    public string Urn { get; set; }
    public UrnType? UrnType { get; set; }
    public bool IsFormDataSearch { get; set; }
    public bool IsRelativeToServer { get; set; }
    public bool IsOperation
    {
      get
      {
        return (this.OperationType.HasValue);
      }
    }
    public OperationScope? OperationType { get; set; }
    public bool IsContained { get; set; }
    public bool IsMetaData { get; set; }
    public bool IsHistoryReferance { get; set; }
    public bool IsCompartment { get; set; }
    public System.Uri UriPrimaryServiceRoot
    {
      get
      {
        if (this.IsRelativeToServer)
          return this.PrimaryServiceRootServers;
        else
          return this.PrimaryServiceRootRemote;
      }
    }
    public Uri PrimaryServiceRootRemote { get; set; }
    public Uri PrimaryServiceRootServers { get; set; }
    public FhirVersion FhirMajorVersion { get; set; }
  }
}

