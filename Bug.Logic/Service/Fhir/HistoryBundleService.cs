﻿using Bug.Common.Compression;
using Bug.Common.DateTimeTools;
using Bug.Common.Enums;
using Bug.Common.FhirTools.Bundle;
using Bug.Logic.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service.Fhir
{
  public class HistoryBundleService : IHistoryBundleService
  {
    private readonly IGZipper IGZipper;
    private readonly IFhirResourceParseJsonService IFhirResourceParseJsonService;
    private readonly IServerDateTimeSupport IServerDateTimeSupport;
    public HistoryBundleService(IGZipper IGZipper,
      IFhirResourceParseJsonService IFhirResourceParseJsonService,
      IServerDateTimeSupport IServerDateTimeSupport)
    {
      this.IGZipper = IGZipper;
      this.IFhirResourceParseJsonService = IFhirResourceParseJsonService;
      this.IServerDateTimeSupport = IServerDateTimeSupport;
    }

    public BundleModel GetHistoryBundleModel(IList<ResourceStore> ResourceStoreList)
    {
      //Construct the History Bundle
      var BundleModel = new BundleModel(BundleType.History)
      {
        Total = ResourceStoreList.Count
      };
      BundleModel.Entry = new List<BundleModel.EntryComponent>();
      foreach (var ResourceStore in ResourceStoreList)
      {
        var entry = new BundleModel.EntryComponent();
        BundleModel.Entry.Add(entry);
        if (ResourceStore.ResourceBlob is object)
        {
          entry.Resource = IFhirResourceParseJsonService.ParseJson(ResourceStore.FhirVersionId, IGZipper.Decompress(ResourceStore.ResourceBlob));
        }

        string RequestUrl = ResourceStore.ResourceTypeId.GetCode();
        if (ResourceStore.MethodId == HttpVerb.PUT || ResourceStore.MethodId == HttpVerb.DELETE)
        {
          RequestUrl = $"{RequestUrl}/{ResourceStore.ResourceId}";
        }
        entry.Request = new BundleModel.RequestComponent(ResourceStore.MethodId, RequestUrl);
        entry.Response = new BundleModel.ResponseComponent($"{ResourceStore.HttpStatusCode.Code.ToString()} - {((int)ResourceStore.HttpStatusCode.Number).ToString()}")
        {
          LastModified = IServerDateTimeSupport.ZuluToServerTimeZone(ResourceStore.LastUpdated)
        };

      }
      return BundleModel;
    }
  }
}
