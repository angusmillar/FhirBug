using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bug.Data.Repository.Base;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using System.Threading.Tasks;
using Bug.Common.Enums;
using Bug.Logic.Service.SearchQuery.SearchQueryEntity;
using LinqKit;
using Bug.Data.Predicates;

namespace Bug.Data.Repository
{
  public class ResourceStoreRepository : Repository<ResourceStore>, IResourceStoreRepository
  {
    private readonly IPredicateFactory IPredicateFactory;
    public ResourceStoreRepository(AppDbContext context, IPredicateFactory IPredicateFactory)
      : base(context) 
    {
      this.IPredicateFactory = IPredicateFactory;
    }

    public async Task<IList<ResourceStore>> GetSearch(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceType, IList<ISearchQueryBase> searchQueryList)
    {
      var Predicate = IPredicateFactory.Get(fhirVersion, resourceType, searchQueryList);
      string Debug = Predicate.Expand().ToString();
      return await DbSet.AsExpandable().Where(Predicate).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public async Task<ResourceStore?> GetCurrentAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId)
    {
      return await DbSet.SingleOrDefaultAsync(x =>
        x.FhirVersionId == fhirMajorVersion &
        x.ResourceTypeId == resourceType &
        x.ResourceId == resourceId &
        x.IsCurrent == true);
    }

    public async Task<bool> ReferentialIntegrityCheckAsync(Common.Enums.FhirVersion fhirVersion, Common.Enums.ResourceType resourceTypeId, string resourceId, int? versionId = null)
    {
      if (versionId.HasValue)
      {
        return await DbSet.AnyAsync(x =>
          x.FhirVersionId == fhirVersion &
          x.ResourceTypeId == resourceTypeId &
          x.ResourceId == resourceId &
          x.VersionId == versionId.Value &
          x.IsDeleted == false);
      }
      else
      {
        return await DbSet.AnyAsync(x =>
          x.FhirVersionId == fhirVersion &
          x.ResourceTypeId == resourceTypeId &
          x.ResourceId == resourceId &
          x.IsCurrent == true &
          x.IsDeleted == false);
      }
    }

    public async Task<IList<ResourceStore>> GetResourceHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType)
    {
      return await DbSet.Select(x => new ResourceStore()
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        IsCurrent = x.IsCurrent,
        IsDeleted = x.IsDeleted,
        LastUpdated = x.LastUpdated,
        VersionId = x.VersionId,
        ResourceBlob = x.ResourceBlob,
        FhirVersionId = x.FhirVersionId,
        ResourceTypeId = x.ResourceTypeId,
        ResourceType = x.ResourceType,
        MethodId = x.MethodId,
        HttpStatusCodeId = x.HttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FhirVersionId == fhirMajorVersion &
        y.ResourceTypeId == resourceType).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public async Task<IList<ResourceStore>> GetBaseHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion)
    {
      return await DbSet.Select(x => new ResourceStore()
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        IsCurrent = x.IsCurrent,
        IsDeleted = x.IsDeleted,
        LastUpdated = x.LastUpdated,
        VersionId = x.VersionId,
        ResourceBlob = x.ResourceBlob,
        FhirVersionId = x.FhirVersionId,
        ResourceTypeId = x.ResourceTypeId,
        ResourceType = x.ResourceType,
        MethodId = x.MethodId,
        HttpStatusCodeId = x.HttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FhirVersionId == fhirMajorVersion).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public async Task<ResourceStore?> GetVersionAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId, int versionId)
    {
      return await DbSet.SingleOrDefaultAsync(x =>
        x.FhirVersionId == fhirMajorVersion &
        x.ResourceTypeId == resourceType &
        x.ResourceId == resourceId &
        x.VersionId == versionId);
    }

    public async Task<IList<ResourceStore>> GetInstanceHistoryListAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId)
    {
      return await DbSet.Select(x => new ResourceStore()
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        IsCurrent = x.IsCurrent,
        IsDeleted = x.IsDeleted,
        LastUpdated = x.LastUpdated,
        VersionId = x.VersionId,
        ResourceBlob = x.ResourceBlob,
        FhirVersionId = x.FhirVersionId,
        ResourceTypeId = x.ResourceTypeId,
        ResourceType = x.ResourceType,
        MethodId = x.MethodId,
        HttpStatusCodeId = x.HttpStatusCodeId,
        HttpStatusCode = x.HttpStatusCode
      }).Where(y =>
        y.FhirVersionId == fhirMajorVersion &
        y.ResourceTypeId == resourceType &
        y.ResourceId == resourceId).OrderBy(z => z.LastUpdated).ToListAsync();
    }

    public async Task UpdateCurrentAsync(ResourceStore resourceStore)
    {
      DbSet.Attach(resourceStore);
      _context.Entry(resourceStore).Property(v => v.IsCurrent).IsModified = true;
      _context.Entry(resourceStore).Property(v => v.Updated).IsModified = true;
      RemoveAllIndexes(resourceStore.Id);

      //Remove all Contained Resources and their indexes for this main Resource    
      foreach (var ContainedResource in await DbSet.Select(x => new ResourceStore()
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        ContainedId = x.ContainedId
      }).Where(x => x.ResourceId == resourceStore.ResourceId && x.ContainedId != null).ToArrayAsync())
      {
        RemoveAllIndexes(ContainedResource.Id);
        _context.Set<ResourceStore>().Remove(ContainedResource);
      }
    }

    public async Task<ResourceStore?> GetCurrentMetaAsync(Common.Enums.FhirVersion fhirMajorVersion, Common.Enums.ResourceType resourceType, string resourceId)
    {
      return await DbSet.Select(x => new ResourceStore()
      {
        Id = x.Id,
        ResourceId = x.ResourceId,
        IsCurrent = x.IsCurrent,
        IsDeleted = x.IsDeleted,
        LastUpdated = x.LastUpdated,
        VersionId = x.VersionId,
        FhirVersionId = x.FhirVersionId,
        ResourceTypeId = x.ResourceTypeId,
        //ResourceType = x.ResourceType
      }).SingleOrDefaultAsync(y =>
        y.FhirVersionId == fhirMajorVersion &
        y.ResourceTypeId == resourceType &
        y.ResourceId == resourceId &
        y.IsCurrent == true);
    }

    private void RemoveAllIndexes(int resourceStoreId)
    {
      RemoveIndex<IndexDateTime>(resourceStoreId);
      RemoveIndex<IndexQuantity>(resourceStoreId);
      RemoveIndex<IndexReference>(resourceStoreId);
      RemoveIndex<IndexString>(resourceStoreId);
      RemoveIndex<IndexToken>(resourceStoreId);
      RemoveIndex<IndexUri>(resourceStoreId);
    }

    private void RemoveIndex<IndexType>(int resourceStoreId) where IndexType : IndexBase, new()
    {
      _context.Set<IndexType>().Where(x => x.ResourceStoreId == resourceStoreId)
        .Select(y => new IndexType() { Id = y.Id }).ToList()
        .ForEach(b => _context.Set<IndexType>().Remove(b));
    }


  }
}

