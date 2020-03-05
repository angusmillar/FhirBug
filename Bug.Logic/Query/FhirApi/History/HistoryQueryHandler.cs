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

namespace Bug.Logic.Query.FhirApi.History
{
  public class HistoryQueryHandler : IQueryHandler<HistoryQuery, FhirApiResult>
  {
    private readonly IValidateQueryService IValidateQueryService;
    private readonly IResourceStoreRepository IResourceStoreRepository;    
    private readonly IFhirResourceParseJsonService IFhirResourceParseJsonService;
    private readonly IFhirResourceBundleSupport IFhirResourceBundleSupport;
    private readonly IServerDateTimeSupport IServerDefaultDateTimeOffSet;
    private readonly IGZipper IGZipper;

    public HistoryQueryHandler(
      IValidateQueryService IValidateQueryService,
      IResourceStoreRepository IResourceStoreRepository,            
      IFhirResourceParseJsonService IFhirResourceParseJsonService,
      IFhirResourceBundleSupport IFhirResourceBundleSupport,
      IServerDateTimeSupport IServerDefaultDateTimeOffSet,
      IGZipper IGZipper)
    {
      this.IValidateQueryService = IValidateQueryService;
      this.IResourceStoreRepository = IResourceStoreRepository;            
      this.IFhirResourceParseJsonService = IFhirResourceParseJsonService;
      this.IFhirResourceBundleSupport = IFhirResourceBundleSupport;
      this.IServerDefaultDateTimeOffSet = IServerDefaultDateTimeOffSet;
      this.IGZipper = IGZipper;
    }

    public async Task<FhirApiResult> Handle(HistoryQuery query)
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


      IList<ResourceStore> ResourceStoreList = await IResourceStoreRepository.GetHistoryListAsync(query.FhirMajorVersion, query.ResourceName, query.ResourceId);

      var BundleModel = new BundleModel(Common.Enums.BundleType.History)
      {
        Total = ResourceStoreList.Count
      };
      BundleModel.Entry = new List<BundleModel.EntryComponent>();
      foreach (var ResourceStore in ResourceStoreList)
      {
        var entry = new BundleModel.EntryComponent();
        BundleModel.Entry.Add(entry);
        //entry.FullUrl = new System.Uri("dddddd");
        if (ResourceStore.ResourceBlob is object)
        {
          entry.Resource = IFhirResourceParseJsonService.ParseJson(ResourceStore.FhirVersion.FhirMajorVersion, IGZipper.Decompress(ResourceStore.ResourceBlob));
        }        
        entry.Request = new BundleModel.RequestComponent(ResourceStore.FkMethodId, new System.Uri("https://blabla/Patient/1"));
        entry.Response = new BundleModel.ResponseComponent($"{ResourceStore.HttpStatusCode.Code.ToString()} - {((int)ResourceStore.HttpStatusCode.Number).ToString()}")
        {          
          LastModified = IServerDefaultDateTimeOffSet.ZuluToServerTimeZone(ResourceStore.LastUpdated)
        };
      }

      return new FhirApiResult(System.Net.HttpStatusCode.OK, query.FhirMajorVersion)
      {
        FhirResource = IFhirResourceBundleSupport.GetFhirResource(query.FhirMajorVersion, BundleModel)
      };
        
    }
  }
}
