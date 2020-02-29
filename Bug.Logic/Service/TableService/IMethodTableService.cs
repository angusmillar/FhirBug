using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.Service.TableService
{
  public interface IMethodTableService
  {
    Task<Method> GetSetMethod(HttpVerb httpVerb);
  }
}