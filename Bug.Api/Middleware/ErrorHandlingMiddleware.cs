extern alias R4;
extern alias Stu3;
using R4Model = R4.Hl7.Fhir.Model;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Bug.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bug.Common.Enums;
using Microsoft.Extensions.Logging;

namespace Bug.Api.Middleware
{
  public class ErrorHandlingMiddleware
  {
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly RequestDelegate next;
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
      this.next = next;
      this._logger = logger;
    }

    public async Task Invoke(HttpContext context /* other dependencies */)
    {
      try
      {
        await next(context);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex, _logger);
      }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exec, ILogger<ErrorHandlingMiddleware> _logger)
    {
      _logger.LogError(exec, exec.Message);
      FhirMajorVersion VersionInUse = GetFhirVersionInUse(context.Request.Path.Value);
      if (exec is FhirException FhirException)
      {
        _logger.LogError(FhirException, "FhirException has been throwen");
        FhirFormatType AcceptFormatType = Bug.Api.ContentFormatters.FhirMediaType.GetFhirFormatTypeFromAcceptHeader(context.Request.Headers.SingleOrDefault(x => x.Key.ToLower(System.Globalization.CultureInfo.CurrentCulture) == "accept").Value);        
        switch (VersionInUse)
        {
          case FhirMajorVersion.Stu3:
            {
              return Stu3FhirExceptionProcessing(context, FhirException, AcceptFormatType);
            }
          case FhirMajorVersion.R4:
            {
              return R4FhirExceptionProcessing(context, FhirException, AcceptFormatType);
            }
          case FhirMajorVersion.Unknown:
            R4Model.OperationOutcome UnknownOperationOutcomeResult = Bug.R4Fhir.OperationOutComeSupport.GetFatal(new string[] { "Unable to resolve which major version of FHIR is in use." });
            context.Response.ContentType = Bug.Api.ContentFormatters.FhirMediaType.GetMediaTypeHeaderValue(UnknownOperationOutcomeResult.GetType(), FhirFormatType.xml).Value;
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(Bug.R4Fhir.SerializationSupport.SerializeToXml(UnknownOperationOutcomeResult));
          default:
            throw new ApplicationException($"Unable to resolve which major version of FHIR is in use. Found enum: {VersionInUse.ToString()}");

        }        
      }
      else
      {
        string ErrorGuid = Common.FhirTools.FhirGuidSupport.NewFhirGuid();
        string UsersErrorMessage = $"An unhandled exception has been throwen. To protect data privacy the exception information has been writen to the application log with the error log identifier: {ErrorGuid}";
        _logger.LogError(exec, $"Error log identifier: {ErrorGuid}");
        switch (VersionInUse)
        {
          case FhirMajorVersion.Stu3:
            {
              Stu3Model.OperationOutcome Stu3OperationOutcomeResult = Bug.Stu3Fhir.OperationOutComeSupport.GetFatal(new string[] { UsersErrorMessage } );
              context.Response.ContentType = Bug.Api.ContentFormatters.FhirMediaType.GetMediaTypeHeaderValue(Stu3OperationOutcomeResult.GetType(), FhirFormatType.xml).Value;
              context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
              return context.Response.WriteAsync(Bug.Stu3Fhir.SerializationSupport.SerializeToXml(Stu3OperationOutcomeResult));
            }
          case FhirMajorVersion.R4:
            {
              R4Model.OperationOutcome R4OperationOutcomeResult = Bug.R4Fhir.OperationOutComeSupport.GetFatal(new string[] { UsersErrorMessage });             
              context.Response.ContentType = Bug.Api.ContentFormatters.FhirMediaType.GetMediaTypeHeaderValue(R4OperationOutcomeResult.GetType(), FhirFormatType.xml).Value;
              context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
              return context.Response.WriteAsync(Bug.R4Fhir.SerializationSupport.SerializeToXml(R4OperationOutcomeResult));
            }
          case FhirMajorVersion.Unknown:
            Stu3Model.OperationOutcome UnknownOperationOutcomeResult = Bug.Stu3Fhir.OperationOutComeSupport.GetFatal(new string[] { "Unable to resolve which major version of FHIR is in use." });
            context.Response.ContentType = Bug.Api.ContentFormatters.FhirMediaType.GetMediaTypeHeaderValue(UnknownOperationOutcomeResult.GetType(), FhirFormatType.xml).Value;
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(Bug.Stu3Fhir.SerializationSupport.SerializeToXml(UnknownOperationOutcomeResult));
          default:
            string msg = $"Unable to resolve which major version of FHIR is in use. Found enum: {VersionInUse.ToString()}";
            _logger.LogError(msg);
            throw new ApplicationException(msg);            
        }
      }     
    }

    private Task R4FhirExceptionProcessing(HttpContext context, FhirException FhirException, FhirFormatType AcceptFormatType)
    {
      R4Model.OperationOutcome R4OperationOutcomeResult = null;
      if (FhirException is FhirFatalException FatalExec)
      {
        R4OperationOutcomeResult = Bug.R4Fhir.OperationOutComeSupport.GetFatal(FatalExec.MessageList);
      }
      else if (FhirException is FhirErrorException ErrorExec)
      {
        R4OperationOutcomeResult = Bug.R4Fhir.OperationOutComeSupport.GetError(ErrorExec.MessageList);
      }
      else if (FhirException is FhirWarnException WarnExec)
      {
        R4OperationOutcomeResult = Bug.R4Fhir.OperationOutComeSupport.GetWarning(WarnExec.MessageList);
      }
      else if (FhirException is FhirInfoException InfoExec)
      {
        R4OperationOutcomeResult = Bug.R4Fhir.OperationOutComeSupport.GetInformation(InfoExec.MessageList);
      }
      else
      {
        R4OperationOutcomeResult = Bug.R4Fhir.OperationOutComeSupport.GetFatal(new string[] { $"Unexpected FhirException type encountered of : {FhirException.GetType().FullName}" });
      }

      context.Response.StatusCode = (int)FhirException.HttpStatusCode;
      context.Response.ContentType = Bug.Api.ContentFormatters.FhirMediaType.GetMediaTypeHeaderValue(R4OperationOutcomeResult.GetType(), AcceptFormatType).Value;
      if (AcceptFormatType == FhirFormatType.xml)
      {
        return context.Response.WriteAsync(Bug.R4Fhir.SerializationSupport.SerializeToXml(R4OperationOutcomeResult));
      }
      else if (AcceptFormatType == FhirFormatType.json)
      {
        return context.Response.WriteAsync(Bug.R4Fhir.SerializationSupport.SerializeToJson(R4OperationOutcomeResult));
      }
      else
      {
        string msg = $"Unexpected FhirFormatType type encountered of : {AcceptFormatType.GetType().FullName}";
        _logger.LogError(msg);
        throw new ApplicationException(msg);
      }
    }

    private Task Stu3FhirExceptionProcessing(HttpContext context, FhirException FhirException, FhirFormatType AcceptFormatType)
    {
      Stu3Model.OperationOutcome Stu3OperationOutcomeResult = null;
      if (FhirException is FhirFatalException FatalExec)
      {
        Stu3OperationOutcomeResult = Bug.Stu3Fhir.OperationOutComeSupport.GetFatal(FatalExec.MessageList);
      }
      else if (FhirException is FhirErrorException ErrorExec)
      {
        Stu3OperationOutcomeResult = Bug.Stu3Fhir.OperationOutComeSupport.GetError(ErrorExec.MessageList);
      }
      else if (FhirException is FhirWarnException WarnExec)
      {
        Stu3OperationOutcomeResult = Bug.Stu3Fhir.OperationOutComeSupport.GetWarning(WarnExec.MessageList);
      }
      else if (FhirException is FhirInfoException InfoExec)
      {
        Stu3OperationOutcomeResult = Bug.Stu3Fhir.OperationOutComeSupport.GetInformation(InfoExec.MessageList);
      }
      else
      {
        Stu3OperationOutcomeResult = Bug.Stu3Fhir.OperationOutComeSupport.GetFatal(new string[] { $"Unexpected FhirException type encountered of : {FhirException.GetType().FullName}" });
      }
      context.Response.StatusCode = (int)FhirException.HttpStatusCode;
      context.Response.ContentType = Bug.Api.ContentFormatters.FhirMediaType.GetMediaTypeHeaderValue(Stu3OperationOutcomeResult.GetType(), AcceptFormatType).Value;
      if (AcceptFormatType == FhirFormatType.xml)
      {
        return context.Response.WriteAsync(Bug.Stu3Fhir.SerializationSupport.SerializeToXml(Stu3OperationOutcomeResult));
      }
      else if (AcceptFormatType == FhirFormatType.json)
      {
        return context.Response.WriteAsync(Bug.Stu3Fhir.SerializationSupport.SerializeToJson(Stu3OperationOutcomeResult));
      }
      else
      {
        string msg = $"Unexpected FhirFormatType type encountered of : {AcceptFormatType.GetType().FullName}";
        _logger.LogError(msg);
        throw new ApplicationException(msg);        
      }
    }

    private FhirMajorVersion GetFhirVersionInUse(string RequestPath)
    {
      if (RequestPath.Contains(FhirMajorVersion.Stu3.GetLiteral(), StringComparison.CurrentCultureIgnoreCase))
      {
        return FhirMajorVersion.Stu3;
      }
      else if (RequestPath.Contains(FhirMajorVersion.R4.GetLiteral(), StringComparison.CurrentCultureIgnoreCase))
      {
        return FhirMajorVersion.R4;
      }
      else
      {
        return FhirMajorVersion.Unknown;
      }      
    }
  }
}
