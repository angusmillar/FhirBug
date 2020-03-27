using System;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using Bug.Common.Interfaces;
using Bug.Common.Dto.Indexing;
using Hl7.Fhir.Utility;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.Interfaces.CacheService;
using Bug.Common.Interfaces.DomainModel;
using Bug.Common.Interfaces.Repository;
using Bug.Common.StringTools;

namespace Bug.Stu3Fhir.Indexing.Setter
{
  public class Stu3ReferenceSetter : IStu3ReferenceSetter
  {
    private readonly Bug.Common.ApplicationConfig.IServiceBaseUrl IPrimaryServiceRootCache;
    private readonly IFhirUriFactory IFhirUriFactory;
    private readonly IResourceTypeSupport IResourceTypeSupport;
    private readonly IServiceBaseUrlCache IServiceBaseUrlCache;
    private readonly IServiceBaseUrlRepository IServiceBaseUrlRepository;

    private ITypedElement? TypedElement;
    private Bug.Common.Enums.ResourceType ResourceType;
    private int SearchParameterId;
    private string? SearchParameterName;

    public Stu3ReferenceSetter(IFhirUriFactory IFhirUriFactory,
      Bug.Common.ApplicationConfig.IServiceBaseUrl IPrimaryServiceRootCache,
      IResourceTypeSupport IResourceTypeSupport,
      IServiceBaseUrlCache IServiceBaseUrlCache,
      IServiceBaseUrlRepository IServiceBaseUrlRepository)
    {
      this.IFhirUriFactory = IFhirUriFactory;
      this.IPrimaryServiceRootCache = IPrimaryServiceRootCache;
      this.IResourceTypeSupport = IResourceTypeSupport;
      this.IServiceBaseUrlCache = IServiceBaseUrlCache;
      this.IServiceBaseUrlRepository = IServiceBaseUrlRepository;
    }

    public async System.Threading.Tasks.Task<IList<IndexReference>> SetAsync(ITypedElement typedElement, Bug.Common.Enums.ResourceType resourceType, int searchParameterId, string searchParameterName)
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
          await SetFhirUri(FhirUri, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is ResourceReference ResourceReference)
        {
          await SetResourcereference(ResourceReference, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Resource Resource)
        {
          SetResource(Resource, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Attachment Attachment)
        {
          await SetUri(Attachment, ResourceIndexList);
        }
        else if (FhirValueProvider.FhirValue is Identifier Identifier)
        {
          await SetIdentifier(Identifier, ResourceIndexList);
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

    private async System.Threading.Tasks.Task SetIdentifier(Identifier Identifier, IList<IndexReference> ResourceIndexList)
    {
      if (Identifier != null && !string.IsNullOrWhiteSpace(Identifier.System) && !string.IsNullOrWhiteSpace(Identifier.Value))
      {
        string TempUrl = $"{Identifier.System}/{Identifier.Value}";
        await SetReferance(TempUrl, ResourceIndexList);
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

    private async System.Threading.Tasks.Task SetResourcereference(ResourceReference ResourceReference, IList<IndexReference> ResourceIndexList)
    {
      //Check the Uri is actual a Fhir resource reference 
      if (Hl7.Fhir.Rest.HttpUtil.IsRestResourceIdentity(ResourceReference.Reference))
      {
        if (!ResourceReference.IsContainedReference && ResourceReference.Url != null)
        {
          await SetReferance(ResourceReference.Url.OriginalString, ResourceIndexList);
        }
      }
    }

    private async System.Threading.Tasks.Task SetUri(Attachment Attachment, IList<IndexReference> ResourceIndexList)
    {
      if (Attachment != null && string.IsNullOrWhiteSpace(Attachment.Url))
      {
        await SetReferance(Attachment.Url, ResourceIndexList);
      }
    }

    private async System.Threading.Tasks.Task SetFhirUri(FhirUri FhirUri, IList<IndexReference> ResourceIndexList)
    {
      if (!string.IsNullOrWhiteSpace(FhirUri.Value))
      {
        await SetReferance(FhirUri.Value, ResourceIndexList);
      }
    }

    private async System.Threading.Tasks.Task SetReferance(string UriString, IList<IndexReference> ResourceIndexList)
    {
      //Check the Uri is actual a Fhir resource reference         
      if (Hl7.Fhir.Rest.HttpUtil.IsRestResourceIdentity(UriString))
      {
        if (this.IFhirUriFactory.TryParse(UriString.Trim(), FhirVersion.Stu3, out IFhirUri? ReferanceUri, out string ErrorMessage))
        {
          if (ReferanceUri is object)
          {
            var ResourceIndex = new IndexReference(this.SearchParameterId);
            await SetResourceIndentityElements(ResourceIndex, ReferanceUri);
            ResourceIndexList.Add(ResourceIndex);
          }
        }
        else
        {
          string message = $"One of the resources references found in the submitted resource is invalid. The reference was : {UriString}. The error was: {ErrorMessage}";
          throw new Bug.Common.Exceptions.FhirErrorException(System.Net.HttpStatusCode.BadRequest, new string[] { message });
        }
      }
    }

    private async System.Threading.Tasks.Task SetResourceIndentityElements(IndexReference ResourceIndex, IFhirUri FhirRequestUri)
    {
      ResourceIndex.ResourceTypeId = IResourceTypeSupport.GetTypeFromName(FhirRequestUri.ResourseName);
      if (!string.IsNullOrWhiteSpace(FhirRequestUri.ResourceId))
        ResourceIndex.ResourceId = FhirRequestUri.ResourceId;
      if (!string.IsNullOrWhiteSpace(FhirRequestUri.VersionId))
        ResourceIndex.VersionId = FhirRequestUri.VersionId;
      if (!string.IsNullOrWhiteSpace(FhirRequestUri.CanonicalVersionId))
        ResourceIndex.CanonicalVersionId = FhirRequestUri.CanonicalVersionId;


      IServiceBaseUrl? ServiceBaseUrl;
      ServiceBaseUrl = await IServiceBaseUrlCache.GetAsync(StringSupport.StripHttp(FhirRequestUri.UriPrimaryServiceRoot!.OriginalString));
      if (ServiceBaseUrl is null)
      {
        ServiceBaseUrl = await IServiceBaseUrlRepository.GetBy(StringSupport.StripHttp(FhirRequestUri.UriPrimaryServiceRoot!.OriginalString));
        if (ServiceBaseUrl is null)
        {
          ServiceBaseUrl = IServiceBaseUrlRepository.Add(StringSupport.StripHttp(FhirRequestUri.UriPrimaryServiceRoot.OriginalString), FhirRequestUri.IsRelativeToServer);
          await IServiceBaseUrlRepository.SaveChangesAsync();
        }
      }
      ResourceIndex.ServiceBaseUrlId = ServiceBaseUrl.Id;
    }


  }
}
