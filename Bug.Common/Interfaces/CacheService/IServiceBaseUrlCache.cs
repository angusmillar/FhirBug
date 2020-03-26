using Bug.Common.Interfaces.DomainModel;
using System.Threading.Tasks;

namespace Bug.Common.Interfaces.CacheService
{
  public interface IServiceBaseUrlCache
  {
    Task<IServiceBaseUrl?> GetAsync(string url);
    Task RemoveAsync(string url);
    Task SetAsync(IServiceBaseUrl serviceBaseUrl);
  }
}