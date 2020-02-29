using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IMethodRepository : IRepository<Method>
  {
    Task<Method?> GetBy(HttpVerb httpVerb);
  }
}