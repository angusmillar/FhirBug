using Bug.Common.Interfaces.DomainModel;
using System.Threading.Tasks;

namespace Bug.Common.Interfaces.CacheService
{
  public interface IServiceBaseUrlCache
  {
    Task<IServiceBaseUrl?> GetAsync(string url);
    Task<IServiceBaseUrl> GetPrimaryAsync();
    Task RemoveAsync(string url);
    Task RemovePrimaryAsync(string url);
    Task SetPrimaryAsync(IServiceBaseUrl serviceBaseUrl);
    Task SetAsync(IServiceBaseUrl serviceBaseUrl);
  }
}