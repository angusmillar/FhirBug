using Bug.Logic.Service.SearchQuery;

namespace Bug.Logic.Service.Serach
{
  public interface ISearchService
  {
    SearchServiceOutcome Process(ISerachQueryServiceOutcome ISerachQueryServiceOutcome);
  }
}