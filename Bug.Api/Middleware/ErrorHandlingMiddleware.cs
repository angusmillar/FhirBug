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
using Bug.Stu3Fhir.Serialization;
using Bug.R4Fhir.Serialization;
using Bug.Logic.Interfaces.CompositionRoot;
using System.Diagnostics;

namespace Bug.Api.Middleware
{
  public class ErrorHandlingMiddleware
  {
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly RequestDelegate next;
    private readonly IStu3SerializationToXml IStu3SerializationToXml;
    private readonly IStu3SerializationToJson IStu3SerializationToJson;
    private readonly IR4SerializationToXml IR4SerializationToXml;
    private readonly IR4SerializationToJson IR4SerializationToJson;
    private readonly IOperationOutcomeSupportFactory IOperationOutComeSupportFactory;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger,
      IStu3SerializationToJson IStu3SerializationToJson,
      IStu3SerializationToXml IStu3SerializationToXml,
      IR4SerializationToXml IR4SerializationToXml,
      IR4SerializationToJson IR4SerializationToJson,
      IOperationOutcomeSupportFactory IOperationOutComeSupportFactory)
    {
      this.next = next;
      this._logger = logger;
      this.IStu3SerializationToXml = IStu3SerializationToXml;
      this.IStu3SerializationToJson = IStu3SerializationToJson;
      this.IR4SerializationToXml = IR4SerializationToXml;
      this.IR4SerializationToJson = IR4SerializationToJson;
      this.IOperationOutComeSupportFactory = IOperationOutComeSupportFactory;
    }

    public async Task Invoke(HttpContext context /* other dependencies */)
    {
      try
      {
        await next(context);
      }
#pragma warning disable CA1031 // Do not catch general exception types
      catch (Exception ex) //disable warning here as we do want to catch all Exceptions, this is the catch all for the whole app!
#pragma warning restore CA1031 // Do not catch general exception types
      {
        await HandleExceptionAsync(context, ex, _logger);
      }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exec, ILogger<ErrorHandlingMiddleware> _logger)
    {

      _logger.LogError(exec, exec.Message);
      FhirVersion VersionInUse = GetFhirVersionInUse(context.Request.Path.Value);
      if (exec is FhirException FhirException)
      {
        _logger.LogError(FhirException, "FhirException has been thrown");
        FhirFormatType AcceptFormatType = Bug.Api.ContentFormatters.FhirMediaType.GetFhirFormatTypeFromAcceptHeader(context.Request.Headers.SingleOrDefault(x => x.Key.ToLower(System.Globalization.CultureInfo.CurrentCulture) == "accept").Value);
        switch (VersionInUse)
        {
          case FhirVersion.Stu3:
            {
              return Stu3FhirExceptionProcessing(context, FhirException, AcceptFormatType);
            }
          case FhirVersion.R4:
            {
              return R4FhirExceptionProcessing(context, FhirException, AcceptFormatType);
            }
          default:
            throw new ApplicationException($"Unable to resolve which major version of FHIR is in use. Found enum: {VersionInUse.ToString()}");
        }
      }
      else
      {
        string ErrorGuid = Common.FhirTools.FhirGuidSupport.NewFhirGuid();
        string UsersErrorMessage = string.Empty;
        if (Debugger.IsAttached)
        {
          
          UsersErrorMessage = $"{System.Text.Encodings.Web.HtmlEncoder.Default.Encode(exec.ToString())} ->  Server Error log identifier: {ErrorGuid}";
        }
        else
        {
          UsersErrorMessage = $"An unhanded exception has been thrown. To protect data privacy the exception information has been written to the application log with the error log identifier: {ErrorGuid}";
        }
        _logger.LogError(exec, $"Error log identifier: {ErrorGuid}");
        switch (VersionInUse)
        {
          case FhirVersion.Stu3:
            {

              Stu3Model.OperationOutcome Stu3OperationOutcomeResult = IOperationOutComeSupportFactory.GetStu3().GetFatal(new string[] { UsersErrorMessage });
              context.Response.ContentType = Bug.Api.ContentFormatters.FhirMediaType.GetMediaTypeHeaderValue(Stu3OperationOutcomeResult.GetType(), FhirFormatType.xml).Value;
              context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
              return context.Response.WriteAsync(IStu3SerializationToXml.SerializeToXml(Stu3OperationOutcomeResult));
            }
          case FhirVersion.R4:
            {
              R4Model.OperationOutcome R4OperationOutcomeResult = IOperationOutComeSupportFactory.GetR4().GetFatal(new string[] { UsersErrorMessage });
              context.Response.ContentType = Bug.Api.ContentFormatters.FhirMediaType.GetMediaTypeHeaderValue(R4OperationOutcomeResult.GetType(), FhirFormatType.xml).Value;
              context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
              return context.Response.WriteAsync(IR4SerializationToXml.SerializeToXml(R4OperationOutcomeResult));
            }
          default:
            string msg = $"Unable to resolve which major version of FHIR is in use. Found enum: {VersionInUse.ToString()}";
            _logger.LogError(msg);
            throw new ApplicationException(msg);
        }
      }
    }

    private Task R4FhirExceptionProcessing(HttpContext context, FhirException FhirException, FhirFormatType AcceptFormatType)
    {
      R4Model.OperationOutcome? R4OperationOutcomeResult;
      if (FhirException is FhirFatalException FatalExec)
      {
        R4OperationOutcomeResult = IOperationOutComeSupportFactory.GetR4().GetFatal(FatalExec.MessageList);
      }
      else if (FhirException is FhirErrorException ErrorExec)
      {
        R4OperationOutcomeResult = IOperationOutComeSupportFactory.GetR4().GetError(ErrorExec.MessageList);
      }
      else if (FhirException is FhirWarnException WarnExec)
      {
        R4OperationOutcomeResult = IOperationOutComeSupportFactory.GetR4().GetWarning(WarnExec.MessageList);
      }
      else if (FhirException is FhirInfoException InfoExec)
      {
        R4OperationOutcomeResult = IOperationOutComeSupportFactory.GetR4().GetInformation(InfoExec.MessageList);
      }
      else
      {
        R4OperationOutcomeResult = IOperationOutComeSupportFactory.GetR4().GetFatal(new string[] { $"Unexpected FhirException type encountered of : {FhirException.GetType().FullName}" });
      }

      context.Response.StatusCode = (int)FhirException.HttpStatusCode;
      context.Response.ContentType = Bug.Api.ContentFormatters.FhirMediaType.GetMediaTypeHeaderValue(R4OperationOutcomeResult.GetType(), AcceptFormatType).Value;
      if (AcceptFormatType == FhirFormatType.xml)
      {
        return context.Response.WriteAsync(IR4SerializationToXml.SerializeToXml(R4OperationOutcomeResult));
      }
      else if (AcceptFormatType == FhirFormatType.json)
      {
        return context.Response.WriteAsync(IR4SerializationToJson.SerializeToJson(R4OperationOutcomeResult));
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
      Stu3Model.OperationOutcome? Stu3OperationOutcomeResult;
      if (FhirException is FhirFatalException FatalExec)
      {
        Stu3OperationOutcomeResult = IOperationOutComeSupportFactory.GetStu3().GetFatal(FatalExec.MessageList);
      }
      else if (FhirException is FhirErrorException ErrorExec)
      {
        Stu3OperationOutcomeResult = IOperationOutComeSupportFactory.GetStu3().GetError(ErrorExec.MessageList);
      }
      else if (FhirException is FhirWarnException WarnExec)
      {
        Stu3OperationOutcomeResult = IOperationOutComeSupportFactory.GetStu3().GetWarning(WarnExec.MessageList);
      }
      else if (FhirException is FhirInfoException InfoExec)
      {
        Stu3OperationOutcomeResult = IOperationOutComeSupportFactory.GetStu3().GetInformation(InfoExec.MessageList);
      }
      else
      {
        Stu3OperationOutcomeResult = IOperationOutComeSupportFactory.GetStu3().GetFatal(new string[] { $"Unexpected FhirException type encountered of : {FhirException.GetType().FullName}" });
      }
      context.Response.StatusCode = (int)FhirException.HttpStatusCode;
      context.Response.ContentType = Bug.Api.ContentFormatters.FhirMediaType.GetMediaTypeHeaderValue(Stu3OperationOutcomeResult.GetType(), AcceptFormatType).Value;
      if (AcceptFormatType == FhirFormatType.xml)
      {
        return context.Response.WriteAsync(IStu3SerializationToXml.SerializeToXml(Stu3OperationOutcomeResult));
      }
      else if (AcceptFormatType == FhirFormatType.json)
      {
        return context.Response.WriteAsync(IStu3SerializationToJson.SerializeToJson(Stu3OperationOutcomeResult));
      }
      else
      {
        string msg = $"Unexpected FhirFormatType type encountered of : {AcceptFormatType.GetType().FullName}";
        _logger.LogError(msg);
        throw new ApplicationException(msg);
      }
    }

    private static FhirVersion GetFhirVersionInUse(string RequestPath)
    {
      if (RequestPath.Contains(FhirVersion.Stu3.GetCode(), StringComparison.CurrentCultureIgnoreCase))
      {
        return FhirVersion.Stu3;
      }
      else if (RequestPath.Contains(FhirVersion.R4.GetCode(), StringComparison.CurrentCultureIgnoreCase))
      {
        return FhirVersion.R4;
      }
      else
      {
        throw new ApplicationException($"Unable to resolve the FHIR version to use for this exception message.");
      }
    }

  }
}
