using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.Service.TableService
{
  public interface IResourceNameTableService
  {
    Task<ResourceName> GetSetResourceName(string resourceName);
  }
}