using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Create;
using Bug.Logic.Query.FhirApi.Read;
using Bug.Logic.Query.FhirApi.Update;
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
    public bool IsValid(FhirBaseApiQuery fhirApiQuery, out FhirResource? OperationOutCome)
    {
      //if (fhirApiQuery.RequestUri is null)
      //  throw new ArgumentNullException(paramName: nameof(fhirApiQuery.RequestUri));

      //OperationOutCome = null;

      //Parse the FHUR URI for errors
      IFhirUri? FhirUri;
      if (!IFhirUriValidator.IsValid(fhirApiQuery.RequestUri.OriginalString, fhirApiQuery.FhirMajorVersion, out FhirUri, out OperationOutCome))
      {
        return false;
      }

      if (fhirApiQuery is FhirApiResourceQuery fhirApiResourceQuery)
      {
        if (string.IsNullOrWhiteSpace(fhirApiResourceQuery.ResourceName))
        {
          string message = $"An empty resource name found in the request URL for a {nameof(fhirApiResourceQuery)} request. " +
            $"All {nameof(fhirApiResourceQuery)} requests must have a resource name. The full URL was: {FhirUri.OriginalString}";
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
        }
      }

      if (fhirApiQuery is FhirApiResourceInstanceQuery fhirApiResourceInstanceQuery)
      {
        if (string.IsNullOrWhiteSpace(FhirUri!.ResourceId))
        {
          string message = $"An empty resource id found in the request URL for a {nameof(fhirApiResourceInstanceQuery)} request. " +
            $"All {nameof(fhirApiResourceInstanceQuery)} requests must have a resource id. The full URL was: {FhirUri.OriginalString}";
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
        }
      }

      if (fhirApiQuery is CreateQuery createQuery)
      {
        //Check the Method is set correctly
        if (createQuery.Method != HttpVerb.POST)
        {
          string message = $"The {nameof(createQuery)} did not have a Method of: {HttpVerb.POST.GetCode()}. The Method found was: {createQuery.Method.GetCode()}. All CreateQueries must be of Method: {HttpVerb.POST.GetCode()}";
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
        }

        //Check we have a FHIR Resource
        if (createQuery.FhirResource is null)
        {
          string message = $"A {HttpVerb.POST.GetCode()} request must have a FHIR resource provided in the body of the request.";
          OperationOutCome = IOperationOutcomeSupport.GetError(fhirApiQuery.FhirMajorVersion, new string[] { message });
          return false;
        }

        //Check the FHIR Resource's name equals the FHIR URL endpoint
        string ResourceName = IFhirResourceNameSupport.GetName(createQuery.FhirResource);
        if (!ResourceName.Equals(FhirUri.ResourseName, StringComparison.CurrentCulture))
        {
          string message = $"The resource provided in the body of the request does not match the resource type stated in the URL. The resource in the body was of type '{ResourceName}' and the URL stated the type '{FhirUri.ResourseName}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetError(createQuery.FhirResource.FhirMajorVersion, new string[] { message });
          return false;
        }

        //Check the FHIR Resource's name equals the query proptery ResourceName (should never fail but worth checking)
        if (!ResourceName.Equals(createQuery.ResourceName, StringComparison.CurrentCulture))
        {
          string message = $"The resource provided in the body of the request does not match the resource type found on the {nameof(createQuery)}. The resource in the body was of type: {ResourceName} and the {nameof(createQuery)} stated the type: {createQuery.ResourceName}. The full URL was: {FhirUri.OriginalString}";
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
        }


      }

      if (fhirApiQuery is UpdateQuery updateQuery)
      {
        //Check the Method is set correctly
        if (updateQuery.Method != HttpVerb.PUT)
        {
          string message = $"The {nameof(updateQuery)} did not have a Method of: {HttpVerb.PUT.GetCode()}. The Method found was: {updateQuery.Method.GetCode()}. All CreateQueries must be of Method: {HttpVerb.PUT.GetCode()}";
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
        }

        //Check we have a FHIR Resource
        if (updateQuery.FhirResource is null)
        {
          string message = $"A {HttpVerb.PUT.GetCode()} request must have a FHIR resource provided in the body of the request.";
          OperationOutCome = IOperationOutcomeSupport.GetError(fhirApiQuery.FhirMajorVersion, new string[] { message });
          return false;
        }

        //Check the FHIR Resource's name equals the FHIR URL endpoint
        string ResourceName = IFhirResourceNameSupport.GetName(updateQuery.FhirResource);
        if (!ResourceName.Equals(FhirUri.ResourseName, StringComparison.CurrentCulture))
        {
          string message = $"The resource provided in the body of the request does not match the resource type stated in the URL. The resource in the body was of type '{ResourceName}' and the URL stated the type '{FhirUri.ResourseName}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetError(updateQuery.FhirResource.FhirMajorVersion, new string[] { message });
          return false;
        }

        //Check the FHIR Resource's name equals the query proptery ResourceName (should never fail but worth checking)
        if (!ResourceName.Equals(updateQuery.ResourceName, StringComparison.CurrentCulture))
        {
          string message = $"The resource provided in the body of the request does not match the resource type found on the {nameof(updateQuery)}. The resource in the body was of type: {ResourceName} and the {nameof(updateQuery)} stated the type: {updateQuery.ResourceName}. The full URL was: {FhirUri.OriginalString}";
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
        }

        //Check the URL has a resource id equals the resource's id              
        string ResourceResourceId = IFhirResourceIdSupport.GetResourceId(updateQuery.FhirResource);
        if (!ResourceResourceId.Equals(FhirUri!.ResourceId, StringComparison.CurrentCulture))
        {
          string message = $"The resource id found in the body of the request does not match the resource id stated in the request URL. " +
            $"The resource id in the body was: '{ResourceResourceId}' and in the URL it was: '{FhirUri.ResourceId}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetError(updateQuery.FhirResource.FhirMajorVersion, new string[] { message });
          return false;
        }

        //Check the UpdateQuery ResourceId property matches the URL and Resource (this should never fail)
        if (updateQuery.ResourceId.Equals(FhirUri!.ResourceId, StringComparison.CurrentCulture) &&
          updateQuery.ResourceId.Equals(ResourceResourceId, StringComparison.CurrentCulture))
        {
          string message = $"The resource id found in the body of the request does not match the resource id stated in the request URL. " +
            $"The resource id in the body was: '{ResourceResourceId}' and in the URL it was: '{FhirUri.ResourceId}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetError(updateQuery.FhirResource.FhirMajorVersion, new string[] { message });
          return false;
        }
      }

      if (fhirApiQuery is ReadQuery readQuery)
      {

      }

      return true;
    }

  }
}
