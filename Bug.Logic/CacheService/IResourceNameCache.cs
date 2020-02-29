using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.CacheService
{
  public interface IResourceNameCache
  {
    Task<ResourceName?> GetAsync(string ResourceName);
    Task SetAsync(ResourceName resourceName);
  }
}