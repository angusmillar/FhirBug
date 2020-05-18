using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.Interfaces.DomainModel;
using Bug.Logic.DomainModel;
using Bug.Logic.Service.Fhir;

namespace Bug.Logic.Service.SearchQuery.SearchQueryEntity
{
  public interface ISearchQueryBase : ISearchParameter
  {
    ISearchQueryBase? ChainedSearchParameter { get; set; }
    bool HasLogicalOrProperties { get; set; }
    string InvalidMessage { get; set; }
    bool IsValid { get; set; }
    SearchModifierCode? Modifier { get; set; }
    IServiceBaseUrl? PrimaryServiceBaseUrl { get; set; }
    string RawValue { get; set; }
    Bug.Common.Enums.ResourceType? TypeModifierResource { get; set; }
    object CloneDeep();
    object CloneDeep(SearchQueryBase Clone);
    void ParseModifier(string parameterName, IResourceTypeSupport IResourceTypeSupport, IKnownResource IKnownResource);
    void ParseValue(string Value);   
    Bug.Common.Enums.ResourceType ResourceContext { get; set; }
  }
}