using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IResourceNameRepository : IRepository<ResourceName>
  {
    Task<ResourceName?> GetByResourceName(string ResourceName);
  }
}