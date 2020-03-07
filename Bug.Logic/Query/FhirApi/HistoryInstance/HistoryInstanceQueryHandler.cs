using Bug.Common.Compression;
using Bug.Common.Exceptions;
using Bug.Common.FhirTools;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using Bug.Logic.Service;
using Bug.Logic.Service.TableService;
using Bug.Logic.Service.ValidatorService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bug.Common.FhirTools.Bundle;
using Bug.Common.DateTimeTools;
using Bug.Common.Enums;
using Bug.Common.ApplicationConfig;
using System;
using Bug.Logic.Service.FhirResourceService;

namespace Bug.Logic.Query.FhirApi.HistoryInstance
{
  public class HistoryInstanceQueryHandler : IQueryHandler<HistoryInstanceQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;    
    private readonly IFhirResourceParseJsonService IFhirResourceParseJsonService;
    private readonly IFhirResourceBundleSupport IFhirResourceBundleSupport;
    private readonly IServerDateTimeSupport IServerDateTimeSupport;
    private readonly IGZipper IGZipper;
    private readonly IHistoryBundleService IHistoryBundleService;

    public HistoryInstanceQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,            
      IFhirResourceParseJsonService IFhirResourceParseJsonService,
      IFhirResourceBundleSupport IFhirResourceBundleSupport,
      IServerDateTimeSupport IServerDateTimeSupport,      
      IGZipper IGZipper,
      IHistoryBundleService IHistoryBundleService)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;            
      this.IFhirResourceParseJsonService = IFhirResourceParseJsonService;
      this.IFhirResourceBundleSupport = IFhirResourceBundleSupport;
      this.IServerDateTimeSupport = IServerDateTimeSupport;      
      this.IGZipper = IGZipper;
      this.IHistoryBundleService = IHistoryBundleService;
    }

    public async Task<FhirApiResult> Handle(HistoryInstanceQuery query)
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


      IList<ResourceStore> ResourceStoreList = await IResourceStoreRepository.GetInstanceHistoryListAsync(query.FhirVersion, query.ResourceName, query.ResourceId);

      //Construct the History Bundle
      var BundleModel = IHistoryBundleService.GetHistoryBundleModel(ResourceStoreList);
      
      return new FhirApiResult(System.Net.HttpStatusCode.OK, query.FhirVersion)
      {
        FhirResource = IFhirResourceBundleSupport.GetFhirResource(query.FhirVersion, BundleModel)
      };
        
    }
  }
}
