using Bug.Common.ApplicationConfig;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.StringTools;
using Bug.Logic.Interfaces.CompositionRoot;
using System;
using System.Linq;

namespace Bug.Logic.UriSupport
{
  public enum UrnType { uuid, oid };

  public class FhirUri : IFhirUri
  {
    public FhirUri(FhirVersion fhirVersion)
    {
      this.FhirVersion = fhirVersion;
      this.ParseErrorMessage = string.Empty;
      this.ErrorInParseing = false;
      this.ResourseName = string.Empty;
      this.CompartmentalisedResourseName = string.Empty;
      this.ResourseName = string.Empty;
      this.ResourceId = string.Empty;
      this.VersionId = string.Empty;
      this.OperationName = string.Empty;
      this.Query = string.Empty;
      this.OriginalString = string.Empty;
      this.IsUrn = false;
      this.Urn = string.Empty;
      this.IsFormDataSearch = false;
      this.IsRelativeToServer = false;
      this.IsContained = false;
      this.IsCompartment = false;
      this.IsMetaData = false;
      this.IsHistoryReferance = false;
    }

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
        {
          if (this.PrimaryServiceRootServers is object)
          {
            return this.PrimaryServiceRootServers;
          }
          else
          {
            throw new ArgumentNullException(nameof(this.PrimaryServiceRootServers));
          }          
        }          
        else
        {
          if (this.PrimaryServiceRootRemote is object)
          {
            return this.PrimaryServiceRootRemote;
          }
          else
          {
            throw new ArgumentNullException(nameof(this.PrimaryServiceRootServers));
          }          
        }          
      }
    }
    public Uri? PrimaryServiceRootRemote { get; set; }
    public Uri? PrimaryServiceRootServers { get; set; }
    public FhirVersion FhirVersion { get; set; }
  }
}

