using Hl7.Fhir.ElementModel;

namespace Bug.R4Fhir.Indexing.Resolver
{
  public interface IFhirPathResolve
  {
    ITypedElement Resolver(string url);
  }
}