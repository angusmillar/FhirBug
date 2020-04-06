using Bug.Common.ApplicationConfig;
using Bug.Common.Enums;
using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel.Projection;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service.Fhir;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bug.Logic.Service.ReferentialIntegrity
{
  public class ReferentialIntegrityService : IReferentialIntegrityService
  {
    private readonly IResourceStoreRepository IResourceStoreRepository;
    private readonly IIndexReferenceRepository IIndexReferenceRepository;
    private readonly IServiceBaseUrlCache IServiceBaseUrlCache;
    private readonly IOperationOutcomeSupport IOperationOutcomeSupport;
    private readonly IEnforceResourceReferentialIntegrity IEnforceResourceReferentialIntegrity;
    public ReferentialIntegrityService(IResourceStoreRepository IResourceStoreRepository,
      IIndexReferenceRepository IIndexReferenceRepository,
      IServiceBaseUrlCache IServiceBaseUrlCache,
      IOperationOutcomeSupport IOperationOutcomeSupport,
      IEnforceResourceReferentialIntegrity IEnforceResourceReferentialIntegrity)
    {
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IIndexReferenceRepository = IIndexReferenceRepository;
      this.IServiceBaseUrlCache = IServiceBaseUrlCache;
      this.IOperationOutcomeSupport = IOperationOutcomeSupport;
      this.IEnforceResourceReferentialIntegrity = IEnforceResourceReferentialIntegrity;
    }

    public async Task<ReferentialIntegrityOutcome> CheckOnCommit(Common.Enums.FhirVersion fhirVersion, List<Bug.Common.Dto.Indexing.IndexReference> IndexReferenceList)
    {
      var Outcome = new ReferentialIntegrityOutcome();

      List<string> ErrorMessageList = new List<string>();
      var PrimaryServiceBaseUrl = await IServiceBaseUrlCache.GetPrimaryAsync(fhirVersion);
      foreach (var Index in IndexReferenceList.Where(x => x.ServiceBaseUrlId == PrimaryServiceBaseUrl.Id))
      {
        if (!Index.ServiceBaseUrlId.HasValue)
          throw new ArgumentNullException(nameof(Index.ServiceBaseUrlId));
        if (!Index.ResourceTypeId.HasValue)
          throw new ArgumentNullException(nameof(Index.ResourceTypeId));
        if (string.IsNullOrWhiteSpace(Index.ResourceId))
          throw new ArgumentNullException(nameof(Index.ResourceId));

        string Ref = $"{Index.ResourceTypeId.GetCode()}/{Index.ResourceId}";
        if (!string.IsNullOrWhiteSpace(Index.VersionId))
        {
          Ref += $"/_history/{Index.VersionId}";
        }
        else if(!string.IsNullOrWhiteSpace(Index.CanonicalVersionId))
        {
          Ref += $"|{Index.CanonicalVersionId}";
        }

          bool HasIntegerVersionError = false;
        int? IntegerVersionId = null;
        
        if (!string.IsNullOrWhiteSpace(Index.VersionId))
        {
          if (!int.TryParse(Index.VersionId, out int IntegerVersionIdTest))
          {
            HasIntegerVersionError = true;
            ErrorMessageList.Add($"The submitted resource contained a resource reference relative to this server which has a resource version part that is not an integer. All resources in this server must have an integer version therefore this resource reference must be invalid. The reference found was: {Ref} .");
          }
          else
          {
            IntegerVersionId = IntegerVersionIdTest;
          }
        }

        if (IEnforceResourceReferentialIntegrity.EnforceRelativeResourceReferentialIntegrity && !HasIntegerVersionError)
        {
          if (!await IResourceStoreRepository.ReferentialIntegrityCheckAsync(fhirVersion, Index.ResourceTypeId.Value, Index.ResourceId, IntegerVersionId))
          {
            ErrorMessageList.Add($"Enforce relative resource referential integrity is turned on for this server. The submitted resource contained a resource reference of: {Ref} which does not exist in the server.");
          }
        }
      }
      if (ErrorMessageList.Count > 0)
      //Remove duplicates with Distinct() because the same reference can be reported twice 
      //or more as it can have many search parameters pointing to the same reference
      {
        Outcome.IsError = true;
        Outcome.FhirResource = IOperationOutcomeSupport.GetError(fhirVersion, ErrorMessageList.Distinct().ToArray());
      }

      return Outcome;
    }

    public async Task<ReferentialIntegrityOutcome> CheckOnDelete(Common.Enums.FhirVersion fhirVersion, ResourceType resourceType, string resourceId)
    {
      var Outcome = new ReferentialIntegrityOutcome();
      if (!IEnforceResourceReferentialIntegrity.EnforceRelativeResourceReferentialIntegrity)
      {
        Outcome.IsError = false;
        return Outcome;
      }
      
      var PrimaryServiceBaseUrl = await IServiceBaseUrlCache.GetPrimaryAsync(fhirVersion);
      if (!await IIndexReferenceRepository.AnyAsync(fhirVersion, PrimaryServiceBaseUrl.Id, resourceType, resourceId))
      {
        Outcome.IsError = false;
        return Outcome;
      }
      else
      {
        List<string> ErrorMessageList = new List<string>
        {
          $"Enforce relative resource referential integrity is turned on for this server. " +
          $"The following resources have resource references to the resource you are trying to delete. " +
          $"These references must be resolved before the resource can be deleted. Only the first 100 detected references are listed below. "
        };
        List <ReferentialIntegrityQuery> ReferentialIntegrityQueryList = await IIndexReferenceRepository.GetResourcesReferenced(fhirVersion, PrimaryServiceBaseUrl.Id, resourceType, resourceId);
        foreach(var ReferentialIntegrityQuery in ReferentialIntegrityQueryList)
        {
          ErrorMessageList.Add($"The resource: {ReferentialIntegrityQuery.TargetResourceTypeId.GetCode()}/{ReferentialIntegrityQuery.TargetResourceId} references the resource attempting to be deleted.");
        }
        Outcome.IsError = true;
        Outcome.FhirResource = IOperationOutcomeSupport.GetError(fhirVersion, ErrorMessageList.ToArray());
        return Outcome;
      }
     
    }
  }
}
