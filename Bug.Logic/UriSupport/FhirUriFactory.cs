using Bug.Common.ApplicationConfig;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.StringTools;
using Bug.Logic.Interfaces.CompositionRoot;
using System;
using System.Linq;


namespace Bug.Logic.UriSupport
{
  public class FhirUriFactory : IFhirUriFactory
  {
    private readonly IServiceBaseUrl IServiceBaseUrl;
    private readonly IValidateResourceNameFactory IResourceNameSupportFactory;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="PrimaryServiceRoot">Should take the form http://SomeServer.net/bla/bla/bla/fhir </param>
    public FhirUriFactory(IServiceBaseUrl IServiceBaseUrl, IValidateResourceNameFactory IResourceNameSupportFactory)
    {
      this.IResourceNameSupportFactory = IResourceNameSupportFactory;
      this.IServiceBaseUrl = IServiceBaseUrl;
    }

    private const string _MetadataName = "metadata";
    private const string _HistoryName = "_history";
    private const string _SearchFormDataName = "_search";
    private const string _UrnName = "urn";
    private const string _OidName = "oid";
    private const string _uuidName = "uuid";
    private const string _HttpName = "http";
    private const string _HttpsName = "https";

    public bool TryParse(string requestUri, FhirVersion fhirVersion, out IFhirUri? fhirUri, out string errorMessage)
    {
      FhirUri fhirUriParse = new FhirUri(fhirVersion)
      {
        PrimaryServiceRootServers = this.IServiceBaseUrl.Url(fhirVersion),        
      };

      if (ProcessRequestUri(System.Net.WebUtility.UrlDecode(requestUri), fhirUriParse))
      {
        fhirUri = fhirUriParse;
        errorMessage = string.Empty;
        return true;
      }
      else
      {
        errorMessage = fhirUriParse.ParseErrorMessage;
        fhirUri = null;
        return false;
      }
    }
    private bool ProcessRequestUri(string RequestUri, FhirUri fhirUri)
    {
      fhirUri.OriginalString = RequestUri;
      
      string ChainResult = ResolveQueryUriPart(RequestUri, fhirUri);

      if (fhirUri.ErrorInParseing)
      {
        return false;
      }
      ChainResult = ResolvePrimaryServiceRoot(ChainResult, fhirUri);

      if (fhirUri.ErrorInParseing)
      {
        return false;
      }
      ChainResult = ResolveRelativeUriPart(ChainResult, fhirUri);

      if (fhirUri.ErrorInParseing)
      {
        return false;
      }
      ChainResult = ResolveResourceIdPart(ChainResult, fhirUri);

      if (ChainResult != string.Empty)
      {
        fhirUri.ParseErrorMessage = $"The URI has extra unknown content near the end of : '{ChainResult}'. The full URI was: '{RequestUri}'";
        fhirUri.ErrorInParseing = true;
        return false;
      }
      else
      {
        return true;
      }
    }
    private string ResolveQueryUriPart(string value, FhirUri fhirUri)
    {
      if (value.Contains('?'))
      {
        var Split = value.Split('?');
        fhirUri.Query = Split[1];
        return Split[0];
      }
      else
      {
        return value;
      }
    }
    private string ResolvePrimaryServiceRoot(string RequestUri, FhirUri fhirUri)
    {
      string RequestRelativePath;
      if (RequestUri.StripHttp().ToLower().StartsWith(fhirUri.PrimaryServiceRootServers!.OriginalString.StripHttp()))
      {
        //If the request URL starts with our known servers root then cut it off and return relative part , job done.
        fhirUri.IsRelativeToServer = true;
        RequestUri = RequestUri.StripHttp();
        string PrimaryServiceRootServers = fhirUri.PrimaryServiceRootServers!.OriginalString.StripHttp();
        RequestRelativePath = RequestUri.Substring(PrimaryServiceRootServers.Count(), RequestUri.Count() - PrimaryServiceRootServers.Count());
        if (RequestRelativePath.StartsWith("/"))
          RequestRelativePath = RequestRelativePath.TrimStart('/');
        return RequestRelativePath;
      }
      else if (RequestUri.ToLower().StartsWith(_HttpName) || RequestUri.ToLower().StartsWith(_HttpsName))
      {
        //If not and the URL starts with 'http' or 'https' then loop through each segment of the URL looking for 
        //a segment that matches to the FHIR Resource name. Once found we can determine the remote root and return the 
        // relative part.
        fhirUri.IsRelativeToServer = false;
        string RemotePrimaryServiceRoot = string.Empty;
        var PathSplit = RequestUri.Split('#')[0].Split('/');
        foreach (string Segment in PathSplit)
        {
          if (IsResourceTypeString(Segment, fhirUri))
          {
            //Resource segment found 
            break;
          }
          else if (Segment.StartsWith("$"))
          {
            //Operation segment found
            break;
          }
          else if (Segment.ToLower() == _MetadataName)
          {
            //metadate segment found
            break;
          }
          RemotePrimaryServiceRoot = String.Join("/", RemotePrimaryServiceRoot, Segment);
        }
        RemotePrimaryServiceRoot = RemotePrimaryServiceRoot.TrimStart('/');
        //strip off and set the primary root
        fhirUri.PrimaryServiceRootRemote = new Uri(RemotePrimaryServiceRoot);
        RequestRelativePath = RequestUri.Substring(RemotePrimaryServiceRoot.Count(), RequestUri.Count() - RemotePrimaryServiceRoot.Count());
        if (RequestRelativePath.StartsWith("/"))
          RequestRelativePath = RequestRelativePath.TrimStart('/');
        return RequestRelativePath;
      }
      else if (RequestUri.ToLower().StartsWith($"{_UrnName}:{_uuidName}:") || RequestUri.ToLower().StartsWith($"{_UrnName}:{_OidName}:"))
      {
        fhirUri.IsRelativeToServer = false;
        fhirUri.IsUrn = true;
        if (RequestUri.ToLower().StartsWith($"{_UrnName}:{_uuidName}:"))
        {
          fhirUri.UrnType = UriSupport.UrnType.uuid;
          fhirUri.Urn = RequestUri;
          if (!UuidSupport.IsValidValue(fhirUri.Urn))
          {
            fhirUri.ParseErrorMessage = $"The {_UrnName}:{_uuidName} value given is not valid: {fhirUri.Urn}";
            fhirUri.ErrorInParseing = true;
            return string.Empty;
          }
        }
        if (RequestUri.ToLower().StartsWith($"{_UrnName}:{_OidName}:"))
        {
          fhirUri.UrnType = UriSupport.UrnType.oid;
          fhirUri.Urn = RequestUri;
          if (!OidSupport.IsValidValue(fhirUri.Urn))
          {
            fhirUri.ParseErrorMessage = $"The {_UrnName}:{_OidName} value given is not valid: {fhirUri.Urn}";
            fhirUri.ErrorInParseing = true;
            return string.Empty;
          }
        }
        RequestRelativePath = RequestUri.Substring(fhirUri.Urn.Count(), RequestUri.Count() - fhirUri.Urn.Count());
        return RequestRelativePath;
      }
      else
      {
        //The path has not Primary root and is a relative URI
        fhirUri.IsRelativeToServer = true;
        RequestRelativePath = RequestUri;
        return RequestRelativePath;
      }
    }
    private string ResolveRelativeUriPart(string RequestRelativePath, FhirUri fhirUri)
    {
      if (RequestRelativePath == string.Empty)
        return string.Empty;

      string Remainder; ;
      var SplitParts = RequestRelativePath.Split('?')[0].Split('/');
      foreach (string Segment in SplitParts)
      {
        if (Segment.StartsWith("$"))
        {
          //It is a base operation          
          fhirUri.OperationType = OperationScope.Base;
          fhirUri.OperationName = Segment.TrimStart('$');
          return RequestRelativePath.Substring(fhirUri.OperationName.Count() + 1, RequestRelativePath.Count() - (fhirUri.OperationName.Count() + 1));
        }
        else if (Segment.StartsWith("#"))
        {
          //It is a contained referance with out a resource name e.g (#123456) and not (Patient/#123456)
          fhirUri.IsContained = true;
          fhirUri.IsRelativeToServer = false;
          fhirUri.ResourceId = Segment.TrimStart('#');
          return RequestRelativePath.Substring(fhirUri.ResourceId.Count() + 1, RequestRelativePath.Count() - (fhirUri.ResourceId.Count() + 1));
        }
        else if (Segment.ToLower() == _MetadataName)
        {
          //This is a metadata request
          fhirUri.IsMetaData = true;
          Remainder = RequestRelativePath.Substring(_MetadataName.Count(), RequestRelativePath.Count() - _MetadataName.Count());
          return RemoveStartsWithSlash(Remainder);
        }
        else if (SplitParts.Count() > 1 || fhirUri.OriginalString.Contains('/'))
        {
          //This is a Resource referance where Patient/123456          
          fhirUri.ResourseName = Segment;
          if (!IsResourceTypeString(fhirUri.ResourseName, fhirUri))
          {
            fhirUri.ParseErrorMessage = GenerateIncorrectResourceNameMessage(fhirUri.ResourseName, fhirUri);
            fhirUri.ErrorInParseing = true;
            Remainder = RequestRelativePath.Substring(fhirUri.ResourseName.Count(), RequestRelativePath.Count() - fhirUri.ResourseName.Count());
            return RemoveStartsWithSlash(Remainder);
          }
          Remainder = RequestRelativePath.Substring(fhirUri.ResourseName.Count(), RequestRelativePath.Count() - fhirUri.ResourseName.Count());
          return RemoveStartsWithSlash(Remainder);
        }
        else if (SplitParts.Count() == 1)
        {
          return Segment;
        }
      }
      fhirUri.ParseErrorMessage = $"The URI has no resource or metadata or $Operation or #Contained segment. Found invalid segment: {RequestRelativePath} in URL {fhirUri.OriginalString}";
      fhirUri.ErrorInParseing = true;
      return string.Empty;
    }
    private string ResolveResourceIdPart(string value, FhirUri fhirUri)
    {
      string Remainder = value;
      if (value == string.Empty)
      {
        return value;
      }
      else
      {
        var Split = value.Split('/');
        foreach (string Segment in Split)
        {
          if (string.IsNullOrWhiteSpace(fhirUri.ResourceId))
          {
            //Resource Id
            if (Segment.StartsWith("#"))
            {
              //Contained Resource #Id
              fhirUri.IsContained = true;
              fhirUri.IsRelativeToServer = false;
              fhirUri.ResourceId = Segment.TrimStart('#');
              Remainder = RemoveStartsWithSlash(Remainder.Substring(fhirUri.ResourceId.Count() + 1, Remainder.Count() - (fhirUri.ResourceId.Count() + 1)));
            }
            else if (Segment.ToLower() == _SearchFormDataName)
            {
              //Search Form Data 
              fhirUri.IsFormDataSearch = true;
              Remainder = RemoveStartsWithSlash(Remainder.Substring(_SearchFormDataName.Count(), Remainder.Count() - _SearchFormDataName.Count()));
              //Must not be anything after _search, the search parameters are in the body.
              break;
            }
            else if (!fhirUri.IsOperation && Segment.StartsWith("$"))
            {
              //A Resource $operation e.g (base/Patient/$operation)              
              fhirUri.OperationType = OperationScope.Resource;
              fhirUri.OperationName = Segment.TrimStart('$');
              Remainder = RemoveStartsWithSlash(Remainder.Substring(fhirUri.OperationName.Count() + 1, Remainder.Count() - (fhirUri.OperationName.Count() + 1)));
              return Remainder;
            }
            else
            {
              //Normal Resource Id
              fhirUri.ResourceId = Segment;
              Remainder = RemoveStartsWithSlash(Remainder.Substring(fhirUri.ResourceId.Count(), Remainder.Count() - fhirUri.ResourceId.Count()));
            }
          }
          else
          {
            if (!fhirUri.IsOperation && !string.IsNullOrWhiteSpace(fhirUri.ResourceId) && Segment.StartsWith("$"))
            {
              //A Resource Instance $operation e.g (base/Patient/10/$operation)              
              fhirUri.OperationType = OperationScope.Instance;
              fhirUri.OperationName = Segment.TrimStart('$');
              Remainder = RemoveStartsWithSlash(Remainder.Substring(fhirUri.OperationName.Count() + 1, Remainder.Count() - (fhirUri.OperationName.Count() + 1)));
              return Remainder;
            }
            else if (Segment.ToLower() == _HistoryName)
            {
              //History segment e.g (_history)
              //Is this case iterate over loop again to see is we have a Resource VersionId
              fhirUri.IsHistoryReferance = true;
              Remainder = RemoveStartsWithSlash(Remainder.Substring(_HistoryName.Count(), Remainder.Count() - _HistoryName.Count()));
            }
            else if (fhirUri.IsHistoryReferance)
            {
              //History version id
              fhirUri.VersionId = Segment;
              Remainder = RemoveStartsWithSlash(Remainder.Substring(fhirUri.VersionId.Count(), Remainder.Count() - fhirUri.VersionId.Count()));
              return Remainder;
            }
            else if (IsResourceTypeString(Segment, fhirUri))
            {
              //Is this a Compartment reference e.g ([base]/Patient/[id]/Condition?code:in=http://hspc.org/ValueSet/acute-concerns)
              //where 'Patient' is the compartment and 'Condition' is the resource.
              fhirUri.CompartmentalisedResourseName = Segment;
              //this.CompartmentalisedResourseType = IResourceNameResolutionSupport.GetResourceType(this.ResourseName);
              fhirUri.IsCompartment = true;
              Remainder = RemoveStartsWithSlash(Remainder.Substring(fhirUri.CompartmentalisedResourseName.Count(), Remainder.Count() - fhirUri.CompartmentalisedResourseName.Count()));
              return Remainder;
            }
          }
        }
        return Remainder;
      }
    }
    private static string RemoveStartsWithSlash(string value)
    {
      if (value.StartsWith("/"))
        value = value.TrimStart('/');
      return value;
    }
    private bool IsResourceTypeString(string value, FhirUri fhirUri)
    {
      //This is a valid Resource Type string   
      if (fhirUri.FhirVersion == FhirVersion.Stu3)
      {
        return IResourceNameSupportFactory.GetStu3().IsKnownResource(value);
      }
      else if (fhirUri.FhirVersion == FhirVersion.R4)
      {
        return IResourceNameSupportFactory.GetR4().IsKnownResource(value);
      }
      else
      {
        throw new Bug.Common.Exceptions.FhirVersionFatalException(fhirUri.FhirVersion);
      }
    }
    private string GenerateIncorrectResourceNameMessage(string ResourceName, FhirUri fhirUri)
    {      
      if (ResourceName.ToLower() == "_history")
      {
        return $"This server has not implemented the whole system Interaction of history. Instance level history is implemented, for example '[base]/Patient/1/_history'";
      }
      else if (ResourceName.ToLower() == "conformance")
      {
        return $"The resource name given '{ResourceName}' is not a resource supported by the .net FHIR API Version: {fhirUri.FhirVersion.GetCode()}. Perhaps you wish to find the server's conformance statement resource named 'CapabilityStatement' which can be obtained from the endpoint '[base]/metadata' ";
      }
      else
      {
        if (char.IsLower(ResourceName.ToCharArray()[0]))
        {
          if (IsResourceTypeString(StringSupport.UppercaseFirst(ResourceName), fhirUri))
          {
            return $"The resource name or Compartment name given '{ResourceName}' must begin with a capital letter, e.g ({StringSupport.UppercaseFirst(ResourceName)})";
          }
          else
          {
            return $"The resource name or Compartment name given '{ResourceName}' is not a Resource supported by the .net FHIR API Version: {fhirUri.FhirVersion.GetCode()}.";
          }
        }
        else
        {
          return $"The resource name or compartment is not supported for this FHIR version. The resource name found in the URI was {fhirUri.ResourseName} and the version of FHIR used was {fhirUri.FhirVersion.GetCode()}. Remember that FHIR resource names are case sensitive."; ;
        }
      }
    }

  }
}

