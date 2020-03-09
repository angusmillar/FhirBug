using Bug.Common.Enums;
using System;

namespace Bug.Logic.UriSupport
{
  public interface IFhirUri
  {
    string CompartmentalisedResourseName { get; }
    bool ErrorInParseing { get; }
    FhirVersion FhirVersion { get; }
    bool IsCompartment { get; }
    bool IsContained { get; }
    bool IsFormDataSearch { get; }
    bool IsHistoryReferance { get; }
    bool IsMetaData { get; }
    bool IsOperation { get; }
    bool IsRelativeToServer { get; }
    bool IsUrn { get; }
    string OperationName { get; }
    OperationScope? OperationType { get; }
    string OriginalString { get; }
    string ParseErrorMessage { get; }
    Uri? PrimaryServiceRootRemote { get; }
    Uri? PrimaryServiceRootServers { get; }
    string Query { get; }
    string ResourceId { get; }
    string ResourseName { get; }
    Uri? UriPrimaryServiceRoot { get; }
    string Urn { get; }
    UrnType? UrnType { get; }
    string VersionId { get; }
  }
}