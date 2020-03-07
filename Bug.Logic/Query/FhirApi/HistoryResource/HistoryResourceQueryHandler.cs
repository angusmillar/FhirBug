using Bug.Common.Compression;
using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.TableService;
using Bug.Logic.Service.ValidatorService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bug.Common.FhirTools.Bundle;
using Bug.Common.FhirTools;
using Bug.Common.DateTimeTools;
using Bug.Logic.Service.FhirResourceService;

namespace Bug.Logic.Query.FhirApi.HistoryResource
{
  public class HistoryResourceQueryHandler : IQueryHandler<HistoryResourceQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;    
    private readonly IFhirResourceBundleSupport IFhirResourceBundleSupport;
    private readonly IHistoryBundleService IHistoryBundleService;

    public HistoryResourceQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,
      IFhirResourceBundleSupport IFhirResourceBundleSupport,
      IHistoryBundleService IHistoryBundleService)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;
      this.IFhirResourceBundleSupport = IFhirResourceBundleSupport;
      this.IHistoryBundleService = IHistoryBundleService;
    }

    public async Task<FhirApiResult> Handle(HistoryResourceQuery query)
    {

      if (!IValidateQueryService.IsValid(query, out FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiResult(System.Net.HttpStatusCode.BadRequest, IsNotValidOperationOutCome!.FhirMajorVersion)
        {
          ResourceId = null,
          FhirResource = IsNotValidOperationOutCome,
          VersionId = null
        };
      }

      IList<ResourceStore> ResourceStoreList = await IResourceStoreRepository.GetResourceHistoryListAsync(query.FhirVersion, query.ResourceName);

      //Construct the History Bundle
      var BundleModel = IHistoryBundleService.GetHistoryBundleModel(ResourceStoreList);      

      return new FhirApiResult(System.Net.HttpStatusCode.OK, query.FhirVersion)
      {
        FhirResource = IFhirResourceBundleSupport.GetFhirResource(query.FhirVersion, BundleModel)
      };

    }
  }
}
