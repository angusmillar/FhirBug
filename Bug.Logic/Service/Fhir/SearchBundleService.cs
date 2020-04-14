using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.Enums;
using Bug.Common.FhirTools.Bundle;
using Bug.Common.Interfaces.CacheService;
using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Service.Fhir
{
  public class SearchBundleService : ISearchBundleService
  {
    private readonly IGZipper IGZipper;
    private readonly IFhirResourceParseJsonService IFhirResourceParseJsonService;
    private readonly IServerDateTimeSupport IServerDateTimeSupport;
    private readonly IServiceBaseUrlCache IServiceBaseUrlCache;
    public SearchBundleService(IGZipper IGZipper,
      IFhirResourceParseJsonService IFhirResourceParseJsonService,
      IServerDateTimeSupport IServerDateTimeSupport,
      IServiceBaseUrlCache IServiceBaseUrlCache)
    {
      this.IGZipper = IGZipper;
      this.IFhirResourceParseJsonService = IFhirResourceParseJsonService;
      this.IServerDateTimeSupport = IServerDateTimeSupport;
      this.IServiceBaseUrlCache = IServiceBaseUrlCache;
    }

    public async Task<BundleModel> GetSearchBundleModel(IList<ResourceStore> ResourceStoreList, Bug.Common.Enums.FhirVersion fhirVersion)
    {
      var IServiceBaseUrl = await IServiceBaseUrlCache.GetPrimaryAsync(fhirVersion);
      //Construct the History Bundle
      var BundleModel = new BundleModel(BundleType.Searchset)
      {
        Total = ResourceStoreList.Count
      };
      BundleModel.Entry = new List<BundleModel.EntryComponent>();
      foreach (var ResourceStore in ResourceStoreList)
      {
        var entry = new BundleModel.EntryComponent
        {
          FullUrl = new Uri($"{IServiceBaseUrl.Url}/{ResourceStore.ResourceId}")
        };
        BundleModel.Entry.Add(entry);
        if (ResourceStore.ResourceBlob is object)
        {
          entry.Resource = IFhirResourceParseJsonService.ParseJson(ResourceStore.FhirVersionId, IGZipper.Decompress(ResourceStore.ResourceBlob));
        }

      }
      return BundleModel;
    }
  }
}
