using System.Threading.Tasks;

namespace Bug.Logic.CacheService
{
  public interface IHttpStatusCodeCache
  {
    Task<DomainModel.HttpStatusCode?> GetAsync(System.Net.HttpStatusCode httpStatusCode);
    Task RemoveAsync(System.Net.HttpStatusCode httpStatusCode);
    Task SetAsync(DomainModel.HttpStatusCode httpStatusCode);
  }
}