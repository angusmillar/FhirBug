using System;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using Bug.Common.Interfaces;
using Bug.Common.ApplicationConfig;
using Bug.Common.Dto.Indexing;
using Hl7.Fhir.Utility;
using Bug.Common.Enums;
using Bug.Common.FhirTools;

namespace Bug.Stu3Fhir.Indexing.Setter
{
  public class Stu3ReferenceSetter : IStu3ReferenceSetter
  {
    private readonly IServiceBaseUrl IPrimaryServiceRootCache;
    private readonly IFhirUriFactory IFhirUriFactory;
    private readonly IResourceTypeSupport IResourceTypeSupport;

    private ITypedElement? TypedElement;
    private Bug.Common.Enums.ResourceType ResourceType;
    private int SearchParameterId;
    private string? SearchParameterName;

    public Stu3ReferenceSetter(IFhirUriFactory IFhirUriFactory, IServiceBaseUrl IPrimaryServiceRootCache, IResourceTypeSupport IResourceTypeSupport)
    {
      this.IFhirUriFactory = IFhirUriFactory;
      this.IPrimaryServiceRootCache = IPrimaryServiceRootCache;
      this.IResourceTypeSupport = IResourceTypeSupport;
    }

    public IList<IndexReference> Set(ITypedElement typedElement, Bug.Common.Enums.ResourceType resourceType, int searchParameterId, string searchParameterName)
    {
      this.TypedElement = typedElement;
      this.ResourceType = resourceType;
      this.SearchParameterId = searchParameterId;
      this.SearchParameterName = searchParameterName;

      List<IndexReference> ResourceIndexList = new List<IndexReference>();


      if (this.TypedElement is IFhirValueProvider FhirValueProvider && FhirValueProvider.FhirValue != null)
      {
        if (FhirValueProvider.FhirValue is FhirUri FhirUri)
        {
          SetFhirUri(FhirUri, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is ResourceReference ResourceReference)
        {
          SetResourcereference(ResourceReference, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Resource Resource)
        {
          SetResource(Resource, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Attachment Attachment)
        {
          SetUri(Attachment, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Identifier Identifier)
        {
          SetIdentifier(Identifier, ResourceIndexList);
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

    private void SetIdentifier(Identifier Identifier, IList<IndexReference> ResourceIndexList)
    {
      if (Identifier != null && !string.IsNullOrWhiteSpace(Identifier.System) && !string.IsNullOrWhiteSpace(Identifier.Value))
      {
        string TempUrl = $"{Identifier.System}/{Identifier.Value}";
        SetReferance(TempUrl, ResourceIndexList);
      }
    }

    private static void SetResource(Resource resource, IList<IndexReference> ResourceIndexList)
    {
      if (resource.ResourceType.GetLiteral() == Bug.Common.Enums.ResourceType.Composition.GetCode() || resource.ResourceType.GetLiteral() == Bug.Common.Enums.ResourceType.MessageHeader.GetCode())
      {
        //ToDo: What do we do with this Resource as a ResourceReferance??
        //FHIR Spec says:
        //The first resource in the bundle, if the bundle type is "document" - this is a composition, and this parameter provides access to searches its contents
        //and
        //The first resource in the bundle, if the bundle type is "message" - this is a message header, and this parameter provides access to search its contents

        //So the intent is that search parameter 'composition' and 'message' are to work like chain parameters yet the chain reaches into the 
        //first resource of the bundle which should be a Composition  Resource or and MessageHeader resource.
        //Yet, unlike chaining where the reference points to the endpoint for that Resource type, here the reference points to the first entry of the bundle endpoint.
        // It almost feels like we should index at the bundle endpoint all the search parameters for both the Composition  and MessageHeader resources.
        //Or maybe we do special processing on Bundle commits were we pull out the first resource and store it at the appropriate 
        //endpoint yet hind it from access, then provide a reference index type at the bundle endpoint that chains to these 
        //hidden resources
      }
    }

    private void SetResourcereference(ResourceReference ResourceReference, IList<IndexReference> ResourceIndexList)
    {
      //Check the Uri is actual a Fhir resource reference 
      if (Hl7.Fhir.Rest.HttpUtil.IsRestResourceIdentity(ResourceReference.Reference))
      {
        if (!ResourceReference.IsContainedReference && ResourceReference.Url != null)
        {
          SetReferance(ResourceReference.Url.OriginalString, ResourceIndexList);
        }
      }
    }

    private void SetUri(Attachment Attachment, IList<IndexReference> ResourceIndexList)
    {
      if (Attachment != null && string.IsNullOrWhiteSpace(Attachment.Url))
      {
        SetReferance(Attachment.Url, ResourceIndexList);
      }
    }

    private void SetFhirUri(FhirUri FhirUri, IList<IndexReference> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(FhirUri.Value))
      {
        SetReferance(FhirUri.Value, ResourceIndexList);
      }
    }

    private void SetReferance(string UriString, IList<IndexReference> ResourceIndexList)
    {
      //Check the Uri is actual a Fhir resource reference         
      if (Hl7.Fhir.Rest.HttpUtil.IsRestResourceIdentity(UriString))
      {
        if (this.IFhirUriFactory.TryParse(UriString.Trim(), FhirVersion.Stu3, out IFhirUri? ReferanceUri, out string ErrorMessage))
        {
          if (Uri.IsWellFormedUriString(UriString, UriKind.Relative) || Uri.IsWellFormedUriString(UriString, UriKind.Absolute))
          {
            if (ReferanceUri is object)
            {
              var ResourceIndex = new IndexReference(this.SearchParameterId);
              SetResourceIndentityElements(ResourceIndex, ReferanceUri);
              ResourceIndexList.Add(ResourceIndex);
            }
          }
        }
      }
    }

    private void SetResourceIndentityElements(IndexReference ResourceIndex, IFhirUri FhirRequestUri)
    {
      ResourceIndex.FkResourceTypeId = IResourceTypeSupport.GetTypeFromName(FhirRequestUri.ResourseName);
      ResourceIndex.VersionId = FhirRequestUri.VersionId;
      ResourceIndex.ResourceId = FhirRequestUri.ResourceId;
      ResourceIndex.ServiceBaseUrl.IsPrimary = FhirRequestUri.IsRelativeToServer;
    }
  }
}
