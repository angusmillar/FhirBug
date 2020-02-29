using Bug.Common.Enums;
using Bug.Logic.DomainModel;
using System.Threading.Tasks;

namespace Bug.Logic.Service.TableService
{
  public interface IFhirVersionTableService
  {
    Task<FhirVersion> GetSetFhirVersion(FhirMajorVersion fhirMajorVersion);
  }
}