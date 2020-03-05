using Bug.Common.Enums;
using System;

namespace Bug.Logic.UriSupport
{
  public interface IFhirUri
  {
    string CompartmentalisedResourseName { get; set; }
    bool ErrorInParseing { get; set; }
    FhirVersion FhirMajorVersion { get; set; }
    bool IsCompartment { get; set; }
    bool IsContained { get; set; }
    bool IsFormDataSearch { get; set; }
    bool IsHistoryReferance { get; set; }
    bool IsMetaData { get; set; }
    bool IsOperation { get; }
    bool IsRelativeToServer { get; set; }
    bool IsUrn { get; set; }
    string OperationName { get; set; }
    OperationScope? OperationType { get; set; }
    string OriginalString { get; set; }
    string ParseErrorMessage { get; set; }
    Uri PrimaryServiceRootRemote { get; set; }
    Uri PrimaryServiceRootServers { get; set; }
    string Query { get; set; }
    string ResourceId { get; set; }
    string ResourseName { get; set; }
    Uri UriPrimaryServiceRoot { get; }
    string Urn { get; set; }
    UrnType? UrnType { get; set; }
    string VersionId { get; set; }
  }
}