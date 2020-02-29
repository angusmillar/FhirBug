using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.CacheService
{
  public interface IMethodCache
  {
    Task<Method?> GetAsync(HttpVerb httpVerb);
    Task RemoveAsync(HttpVerb httpVerb);
    Task SetAsync(Method method);
  }
}