using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.UriSupport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bug.Logic.Service
{
  public class ValidateQueryService : IValidateQueryService
  {
    private readonly IOperationOutcomeSupport IOperationOutcomeSupport;
    private readonly IFhirResourceNameSupport IFhirResourceNameSupport;
    private readonly IFhirResourceIdSupport IFhirResourceIdSupport;
    private readonly IFhirUriValidator IFhirUriValidator;

    public ValidateQueryService(IOperationOutcomeSupport IOperationOutcomeSupport, 
      IFhirResourceNameSupport IFhirResourceNameSupport,
      IFhirResourceIdSupport IFhirResourceIdSupport,
      IFhirUriValidator IFhirUriValidator)
    {
      this.IOperationOutcomeSupport = IOperationOutcomeSupport;
      this.IFhirResourceNameSupport = IFhirResourceNameSupport;
      this.IFhirResourceIdSupport = IFhirResourceIdSupport;
      this.IFhirUriValidator = IFhirUriValidator;
    }
    public bool IsValid(FhirApiResourceQuery fhirApiResourceQuery, out FhirResource? OperationOutCome)
    {
      if (fhirApiResourceQuery.FhirResource is null)
        throw new ArgumentNullException(paramName: nameof(fhirApiResourceQuery.FhirResource));

      if (fhirApiResourceQuery.RequestUri is null)
        throw new ArgumentNullException(paramName: nameof(fhirApiResourceQuery.RequestUri));

      OperationOutCome = null;
      
      //Parse the FHUR URI for errors
      IFhirUri? FhirUri;
      if (!IFhirUriValidator.IsValid(fhirApiResourceQuery.RequestUri.OriginalString, fhirApiResourceQuery.FhirMajorVersion, out FhirUri, out OperationOutCome))
      {
        return false;
      }

      if (fhirApiResourceQuery.HttpVerb == HttpVerb.PUT || fhirApiResourceQuery.HttpVerb == HttpVerb.POST)
      {
        //Check we have a FHIR Resource
        if (fhirApiResourceQuery.FhirResource is null)
        {
          string message = $"A {HttpVerb.PUT.GetCode()} or {HttpVerb.POST.GetCode()} request must have a FHIR resource provided in the body of the request.";
          OperationOutCome = IOperationOutcomeSupport.GetFatal(fhirApiResourceQuery.FhirMajorVersion, new string[] { message });
          return false;
        }

        //Check the FHIR Resource's name equals the FHIR URL endpoint
        string ResourceName = IFhirResourceNameSupport.GetName(fhirApiResourceQuery.FhirResource);
        if (!ResourceName.Equals(FhirUri.ResourseName, StringComparison.CurrentCulture))
        {
          string message = $"The resource provided in the body of the request does not match the resource type stated in the URL. The resource in the body was of type '{ResourceName}' and the URL stated the type '{FhirUri.ResourseName}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetFatal(fhirApiResourceQuery.FhirResource.FhirMajorVersion, new string[] { message });
          return false;
        }

        //Check the FHIR Resource's name equals the query proptery ResourceName (should never fail but worth checking)
        if (!ResourceName.Equals(fhirApiResourceQuery.ResourceName, StringComparison.CurrentCulture))
        {
          string message = $"The resource provided in the body of the request does not match the resource type stated in the URL. The resource in the body was of type '{ResourceName}' and the URL stated the type '{FhirUri.ResourseName}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetFatal(fhirApiResourceQuery.FhirResource.FhirMajorVersion, new string[] { message });
          return false;
        }
      }
      
      if (fhirApiResourceQuery.HttpVerb == HttpVerb.PUT)
      {
        //Check the URL has a resource id
        if (string.IsNullOrWhiteSpace(FhirUri!.ResourseName))
        {
          string message = $"The was not resource id found in the request URL for this {HttpVerb.PUT.GetCode()} request. " +
            $"All {HttpVerb.PUT.GetCode()} requests must have a resource id. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetFatal(fhirApiResourceQuery.FhirResource.FhirMajorVersion, new string[] { message });
          return false;
        }

        //Check the URL has a resource id equals the resource's id
        string ResourceId = IFhirResourceIdSupport.GetResourceId(fhirApiResourceQuery.FhirResource);
        if (!ResourceId.Equals(FhirUri!.ResourceId, StringComparison.CurrentCulture))
        {
          string message = $"The resource id found in the body of the request does not match the resource id stated in the request URL. " +
            $"The resource id in the body was: '{ResourceId}' and in the URL it was: '{FhirUri.ResourceId}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetFatal(fhirApiResourceQuery.FhirResource.FhirMajorVersion, new string[] { message });
          return false;
        }
      }
      
      return true;
    }
  }
}
