using Bug.Logic.CacheService;
using Bug.Logic.DomainModel;
using Bug.Logic.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Logic.Service.TableService
{
  public class ResourceNameTableService : IResourceNameTableService
  {
    private readonly IResourceNameRepository IResourceNameRepository;
    private readonly IResourceNameCache IResourceNameCache;
    public ResourceNameTableService(IResourceNameRepository IResourceNameRepository, IResourceNameCache IResourceNameCache)
    {
      this.IResourceNameRepository = IResourceNameRepository;
      this.IResourceNameCache = IResourceNameCache;
    }

    public async Task<ResourceName> GetSetResourceName(string resourceName)
    {
      if (resourceName is null)
        throw new ArgumentNullException(paramName: nameof(resourceName));

      ResourceName? ResName = await IResourceNameCache.GetAsync(resourceName);
      if (ResName is null)
      {
        ResName = new ResourceName() { Name = resourceName };
        IResourceNameRepository.Add(ResName);
        await IResourceNameRepository.SaveChangesAsync();
        await IResourceNameCache.SetAsync(ResName);
      }
      return ResName;
    }
  }
}
