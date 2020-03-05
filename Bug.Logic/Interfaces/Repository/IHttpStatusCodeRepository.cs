using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IHttpStatusCodeRepository
  {
    Task<Logic.DomainModel.HttpStatusCode?> GetByCode(System.Net.HttpStatusCode httpStatusCode);
  }
}