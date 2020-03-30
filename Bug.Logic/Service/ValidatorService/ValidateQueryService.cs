using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Common.Interfaces;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Create;
using Bug.Logic.Query.FhirApi.Update;
using Bug.Logic.UriSupport;
using System;

namespace Bug.Logic.Service.ValidatorService
{
  public class ValidateQueryService : IValidateQueryService
  {
    private readonly IOperationOutcomeSupport IOperationOutcomeSupport;
    private readonly IFhirResourceNameSupport IFhirResourceNameSupport;
    private readonly IFhirResourceIdSupport IFhirResourceIdSupport;
    private readonly IFhirUriFactory IFhirUriFactory;

    public ValidateQueryService(IOperationOutcomeSupport IOperationOutcomeSupport,
      IFhirResourceNameSupport IFhirResourceNameSupport,
      IFhirResourceIdSupport IFhirResourceIdSupport,
      IFhirUriFactory IFhirUriFactory)
    {
      this.IOperationOutcomeSupport = IOperationOutcomeSupport;
      this.IFhirResourceNameSupport = IFhirResourceNameSupport;
      this.IFhirResourceIdSupport = IFhirResourceIdSupport;
      this.IFhirUriFactory = IFhirUriFactory;
    }
    public bool IsValid(FhirBaseApiQuery fhirApiQuery, out FhirResource? OperationOutCome)
    {      
      OperationOutCome = null;

      if (!IFhirUriFactory.TryParse(fhirApiQuery.RequestUri.OriginalString, fhirApiQuery.FhirVersion, out IFhirUri? FhirUri, out string ErrorMessage))
      {
        OperationOutCome = IOperationOutcomeSupport.GetError(fhirApiQuery.FhirVersion, new string[] { ErrorMessage });
        return false;
      }

      if (FhirUri is null)
        throw new ArgumentNullException(nameof(FhirUri));
            

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
        if (string.IsNullOrWhiteSpace(fhirApiResourceInstanceQuery.ResourceId))
        {
          string message = $"An empty {nameof(fhirApiResourceInstanceQuery.ResourceId)} found in the {nameof(fhirApiResourceInstanceQuery)} object instance for the request. " +
            $"All {nameof(fhirApiResourceInstanceQuery)} requests must have a populated {nameof(fhirApiResourceInstanceQuery.ResourceId)}.";
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
        }

        if (string.IsNullOrWhiteSpace(FhirUri.ResourceId))
        {
          string message = $"An empty resource id found in the request URL for a {nameof(fhirApiResourceInstanceQuery)} request. " +
            $"All {nameof(fhirApiResourceInstanceQuery)} requests must have a resource id. The full URL was: {FhirUri.OriginalString}";
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
        }
      }

      if (fhirApiQuery is FhirApiResourceInstanceHistoryInstanceQuery fhirApiResourceInstanceHistoryInstanceQuery)
      {
        if (fhirApiResourceInstanceHistoryInstanceQuery.VersionId == 0)
        {
          string message = $"A {nameof(fhirApiResourceInstanceHistoryInstanceQuery.VersionId)} must begin at one for this server. The request had a {nameof(fhirApiResourceInstanceHistoryInstanceQuery.VersionId)} equal to {fhirApiResourceInstanceHistoryInstanceQuery.VersionId.ToString()}. " +
            $"All {nameof(fhirApiResourceInstanceHistoryInstanceQuery.VersionId)} requests must be numeric and begin from one within this server.";          
          OperationOutCome = IOperationOutcomeSupport.GetError(fhirApiResourceInstanceHistoryInstanceQuery.FhirVersion, new string[] { message });
          return false;
        }

        if (string.IsNullOrWhiteSpace(FhirUri.VersionId))
        {
          string message = $"An empty {nameof(FhirUri.VersionId)} found in the request URL for a {nameof(fhirApiResourceInstanceHistoryInstanceQuery)} request. " +
            $"All {nameof(fhirApiResourceInstanceHistoryInstanceQuery)} requests must have a {nameof(FhirUri.VersionId)}. The full URL was: {FhirUri.OriginalString}";
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
          OperationOutCome = IOperationOutcomeSupport.GetError(fhirApiQuery.FhirVersion, new string[] { message });
          return false;
        }

        //Check the FHIR Resource's name equals the FHIR URL endpoint
        string ResourceName = IFhirResourceNameSupport.GetName(createQuery.FhirResource);
        if (!ResourceName.Equals(FhirUri.ResourseName, StringComparison.CurrentCulture))
        {
          string message = $"The resource provided in the body of the request does not match the resource type stated in the URL. The resource in the body was of type '{ResourceName}' and the URL stated the type '{FhirUri.ResourseName}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetError(createQuery.FhirResource.FhirVersion, new string[] { message });
          return false;
        }

        //Check the FHIR Resource's name equals the query property ResourceName (should never fail but worth checking)
        if (!ResourceName.Equals(createQuery.ResourceName, StringComparison.CurrentCulture))
        {
          string message = $"The resource provided in the body of the request does not match the resource type found on the {nameof(createQuery)}. The resource in the body was of type: {ResourceName} and the {nameof(createQuery)} stated the type: {createQuery.ResourceName}. The full URL was: {FhirUri.OriginalString}";
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
        }

        //Check that the Create resource has no resourceId
        string? ResourceResourceId = IFhirResourceIdSupport.GetResourceId(createQuery.FhirResource);
        if (!string.IsNullOrWhiteSpace(ResourceResourceId))
        {
          string message = $"The Create ({createQuery.Method.GetCode()}) interaction creates a new resource in a server-assigned location with a server assigned resource id. If the client wishes to have control over the id of a newly submitted resource, it should use the update ({HttpVerb.PUT.GetCode()}) interaction instead. The resource provide was found to contain the id: {ResourceResourceId}";
          OperationOutCome = IOperationOutcomeSupport.GetError(createQuery.FhirResource.FhirVersion, new string[] { message });
          return false;
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
          OperationOutCome = IOperationOutcomeSupport.GetError(fhirApiQuery.FhirVersion, new string[] { message });
          return false;
        }

        //Check the FHIR Resource's name equals the FHIR URL endpoint
        string ResourceName = IFhirResourceNameSupport.GetName(updateQuery.FhirResource);
        if (!ResourceName.Equals(FhirUri.ResourseName, StringComparison.CurrentCulture))
        {
          string message = $"The resource provided in the body of the request does not match the resource type stated in the URL. The resource in the body was of type '{ResourceName}' and the URL stated the type '{FhirUri.ResourseName}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetError(updateQuery.FhirResource.FhirVersion, new string[] { message });
          return false;
        }

        //Check the FHIR Resource's name equals the query property ResourceName (should never fail but worth checking)
        if (!ResourceName.Equals(updateQuery.ResourceName, StringComparison.CurrentCulture))
        {
          string message = $"The resource provided in the body of the request does not match the resource type found on the {nameof(updateQuery)}. The resource in the body was of type: {ResourceName} and the {nameof(updateQuery)} stated the type: {updateQuery.ResourceName}. The full URL was: {FhirUri.OriginalString}";
          throw new Bug.Common.Exceptions.FhirFatalException(System.Net.HttpStatusCode.InternalServerError, message);
        }

        string? ResourceResourceId = IFhirResourceIdSupport.GetResourceId(updateQuery.FhirResource);
        
        //Check resource has a resource id
        if (string.IsNullOrWhiteSpace(ResourceResourceId))
        {
          string message = $"There was no resource id found in the resource of the request. " +
            $"An update ({updateQuery.Method.GetCode()}) request must contain a resource with a resource id in the body of the request.";
          OperationOutCome = IOperationOutcomeSupport.GetError(updateQuery.FhirResource.FhirVersion, new string[] { message });
          return false;
        }

        //Check the URL has a resource id equals the resource's id              
        if (!ResourceResourceId.Equals(FhirUri.ResourceId, StringComparison.CurrentCulture))
        {
          string message = $"The resource id found in the resource of the request does not match the resource id stated in the request URL. " +
            $"The resource id in the body was: '{ResourceResourceId}' and in the URL it was: '{FhirUri.ResourceId}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetError(updateQuery.FhirResource.FhirVersion, new string[] { message });
          return false;
        }

        //Check the UpdateQuery ResourceId property matches the URL and Resource (this should never fail)
        if (!updateQuery.ResourceId.Equals(FhirUri.ResourceId, StringComparison.CurrentCulture) &&
          !updateQuery.ResourceId.Equals(ResourceResourceId, StringComparison.CurrentCulture))
        {
          string message = $"The resource id found in the body of the request does not match the resource id stated in the request URL. " +
            $"The resource id in the body was: '{ResourceResourceId}' and in the URL it was: '{FhirUri.ResourceId}'. The full URL was: {FhirUri.OriginalString}";
          OperationOutCome = IOperationOutcomeSupport.GetError(updateQuery.FhirResource.FhirVersion, new string[] { message });
          return false;
        }
      }

      //if (fhirApiQuery is ReadQuery readQuery)
      //{

      //}

      //if (fhirApiQuery is VReadQuery vReadQuery)
      //{

      //}

      //if (fhirApiQuery is DeleteQuery DeleteQuery)
      //{

      //}

      return true;
    }

  }
}
