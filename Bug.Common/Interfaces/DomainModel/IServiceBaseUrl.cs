namespace Bug.Common.Interfaces.DomainModel
{
  public interface IServiceBaseUrl
  {
    int Id { get; set; }
    bool IsPrimary { get; set; }
    string Url { get; set; }
    public Common.Enums.FhirVersion FhirVersionId { get; set; }
  }
}