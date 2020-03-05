using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.Interfaces.Repository
{
  public interface IFhirVersionRepository : IRepository<DomainModel.FhirVersion>
  {
    Task<DomainModel.FhirVersion?> GetByVersionAsycn(Common.Enums.FhirVersion fhirMajorVersion);
  }
}