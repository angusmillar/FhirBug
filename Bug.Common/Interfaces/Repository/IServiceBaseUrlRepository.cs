using Bug.Common.Interfaces.DomainModel;
using System.Threading.Tasks;

namespace Bug.Common.Interfaces.Repository
{
  public interface IServiceBaseUrlRepository
  {
    Task<IServiceBaseUrl?> GetBy(string url);
    IServiceBaseUrl Add(string url, bool IsPrimary);
    Task SaveChangesAsync();
  }
}