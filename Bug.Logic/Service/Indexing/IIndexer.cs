using Bug.Common.Enums;
using Bug.Common.FhirTools;
using System.Threading.Tasks;

namespace Bug.Logic.Service.Indexing
{
  public interface IIndexer
  {
    Task<IndexerOutcome> Process(Common.FhirTools.FhirResource fhirResource, ResourceType resourceType);
  }
}