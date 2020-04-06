using Bug.Common.Compression;
using Bug.Common.Enums;
using Bug.Common.Exceptions;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.ValidatorService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bug.Common.FhirTools.Bundle;
using Bug.Common.FhirTools;
using Bug.Common.DateTimeTools;
using Bug.Logic.Service.Fhir;

namespace Bug.Logic.Query.FhirApi.HistoryBase
{
  public class HistoryBaseQueryHandler : IQueryHandler<HistoryBaseQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;    
    private readonly IHistoryBundleService IHistoryBundleService;
    private readonly IFhirResourceBundleSupport IFhirResourceBundleSupport;    

    public HistoryBaseQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,      
      IHistoryBundleService IHistoryBundleService,
      IFhirResourceBundleSupport IFhirResourceBundleSupport)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;      
      this.IHistoryBundleService = IHistoryBundleService;
      this.IFhirResourceBundleSupport = IFhirResourceBundleSupport;      
    }

    public async Task<FhirApiResult> Handle(HistoryBaseQuery query)
    {

      if (!IValidateQueryService.IsValid(query, out Common.FhirTools.FhirResource? IsNotValidOperationOutCome))
      {
        return new FhirApiResult(System.Net.HttpStatusCode.BadRequest, IsNotValidOperationOutCome!.FhirVersion, query.CorrelationId)
        {
          ResourceId = null,
          FhirResource = IsNotValidOperationOutCome,
          VersionId = null
        };
      }

      IList<ResourceStore> ResourceStoreList = await IResourceStoreRepository.GetBaseHistoryListAsync(query.FhirVersion);
      
      var BundleModel = IHistoryBundleService.GetHistoryBundleModel(ResourceStoreList);
      
      return new FhirApiResult(System.Net.HttpStatusCode.OK, query.FhirVersion, query.CorrelationId)
      {
        FhirResource = IFhirResourceBundleSupport.GetFhirResource(query.FhirVersion, BundleModel)
      };

    }
  }
}
