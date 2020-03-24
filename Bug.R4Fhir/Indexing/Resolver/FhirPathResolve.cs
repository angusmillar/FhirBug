using Bug.Common.Interfaces;


namespace Bug.R4Fhir.Indexing.Resolver
{
  public class FhirPathResolve : IFhirPathResolve
  {
    private readonly IFhirUriFactory IFhirUriFactory;

    public FhirPathResolve(IFhirUriFactory IFhirUriFactory)
    {
      this.IFhirUriFactory = IFhirUriFactory;
    }

    public Hl7.Fhir.ElementModel.ITypedElement Resolver(string url)
    {
      CustomResolveElementNavigator ResolveElementNavigator = new CustomResolveElementNavigator();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
      if (IFhirUriFactory.TryParse(url, Common.Enums.FhirVersion.R4, out IFhirUri? fhirUri, out string ErrorMessage))
#pragma warning restore IDE0059 // Unnecessary assignment of a value
      {
        if (fhirUri is object && !string.IsNullOrWhiteSpace(fhirUri.ResourseName))
        {
          ResolveElementNavigator.Name = fhirUri.ResourseName;
          //This type property is the key property to set for resolve() as it needs to match the comparison
          //for example 'AuditEvent.agent.who.where(resolve() is Patient)' Patient is Patient
          //PyroElementNavigator.Type = PyroRequestUri.FhirRequestUri.ResourseName;
          ResolveElementNavigator.InstanceType = fhirUri.ResourseName;
          ResolveElementNavigator.Value = fhirUri.ResourseName;
          ResolveElementNavigator.Location = url;
          return ResolveElementNavigator;
        }
      }

#pragma warning disable CS8603 // Possible null reference return.
      return null;
#pragma warning restore CS8603 // Possible null reference return.
    }
  }
}
