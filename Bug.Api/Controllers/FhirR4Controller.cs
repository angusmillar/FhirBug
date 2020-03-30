extern alias R4;
using R4Model = R4.Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bug.Common.Constant;
using Microsoft.Extensions.Primitives;
using Bug.Common.Enums;
using Bug.Api.Extensions;
using Bug.Logic.Interfaces.CompositionRoot;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Update;
using Bug.Common.FhirTools;
using System.Net;

namespace Bug.Api.Controllers
{
  [Route(EndpointPath.R4Fhir)]
  [ApiController]
  public class FhirR4Controller : ControllerBase
  {
    private readonly IFhirApiQueryHandlerFactory IFhirApiQueryHandlerFactory;
    
    private readonly FhirVersion _ControllerFhirVersion = FhirVersion.R4;

    public FhirR4Controller(IFhirApiQueryHandlerFactory IFhirApiQueryHandlerFactory)
    {
      this.IFhirApiQueryHandlerFactory = IFhirApiQueryHandlerFactory;      
    }

    // GET fhir/values   
    [HttpGet("metadata")]
    public async Task<ActionResult<R4Model.Resource>> GetConformance()
    {
      var Cap = new R4Model.CapabilityStatement();
      await System.Threading.Tasks.Task.Run(() =>
      {        
        Cap.Id = "Test1";
        Cap.Meta = new R4Model.Meta() { LastUpdated = DateTimeOffset.Now };
        Cap.Name = "My Test CapabilityStatment";        
        Cap.Status = R4Model.PublicationStatus.Active;
        Cap.DateElement = new R4Model.FhirDateTime(DateTimeOffset.Now);
        Cap.Kind = R4Model.CapabilityStatementKind.Capability;
        Cap.Software = new R4Model.CapabilityStatement.SoftwareComponent
        {
          Name = "FhirBug"
        };
        Cap.FhirVersion = R4Model.FHIRVersion.N4_0_0;
        Cap.Format = new List<string>() { "xml", "json" };

        Cap.ResourceBase = new Uri("http://localhost/fhir");

      }).ConfigureAwait(false);
      
      return this.StatusCode((int)HttpStatusCode.OK, Cap);
    }

    // GET: stu3/fhir/Patient/100
    [HttpGet, Route("{resourceName}/{resourceId}")]
    public async Task<ActionResult<R4Model.Resource>> Get(string resourceName, string resourceId)
    {      
      var Query = new Logic.Query.FhirApi.Read.ReadQuery(
        HttpVerb.GET,
        _ControllerFhirVersion,
        this.Request.GetUrl(),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        resourceId
        );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetReadCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      this.Response.Headers.AddHeaders(Result.Headers);
      return Result.PrepareResponse<R4Model.Resource>(this);
    }
    // GET: stu3/fhir/Patient/100/_history/2
    [HttpGet, Route("{resourceName}/{resourceId}/_history/{versionId}")]
    public async Task<ActionResult<R4Model.Resource>> GetHistoryVersion(string resourceName, string resourceId, int versionId)
    {
      var Query = new Logic.Query.FhirApi.VRead.VReadQuery(
       HttpVerb.GET,
       _ControllerFhirVersion,
       this.Request.GetUrl(),
       new Dictionary<string, StringValues>(this.Request.Headers),
       resourceName,
       resourceId,
       versionId
       );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetVReadCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<R4Model.Resource>(this);
    }

    // GET: stu3/fhir/Patient/100/_history/2
    [HttpGet, Route("{resourceName}/{resourceId}/_history")]
    public async Task<ActionResult<R4Model.Resource>> GetHistoryInstance(string resourceName, string resourceId)
    {
      var Query = new Logic.Query.FhirApi.HistoryInstance.HistoryInstanceQuery(
       HttpVerb.GET,
       _ControllerFhirVersion,
       this.Request.GetUrl(),
       new Dictionary<string, StringValues>(this.Request.Headers),
       resourceName,
       resourceId
       );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetHistoryInstanceCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<R4Model.Resource>(this);
    }

    // GET: stu3/fhir/Patient/100/_history/2
    [HttpGet, Route("{resourceName}/_history")]
    public async Task<ActionResult<R4Model.Resource>> GetHistoryResource(string resourceName)
    {
      var Query = new Logic.Query.FhirApi.HistoryResource.HistoryResourceQuery(
       HttpVerb.GET,
       _ControllerFhirVersion,
       this.Request.GetUrl(),
       new Dictionary<string, StringValues>(this.Request.Headers),
       resourceName     
       );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetHistoryResourceCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<R4Model.Resource>(this);
    }

    // GET: r4/fhir/_history
    [HttpGet, Route("_history")]
    public async Task<ActionResult<R4Model.Resource>> GetHistoryBase()
    {
      var Query = new Logic.Query.FhirApi.HistoryBase.HistoryBaseQuery(
       HttpVerb.GET,
       _ControllerFhirVersion,
       this.Request.GetUrl(),
       new Dictionary<string, StringValues>(this.Request.Headers)
       );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetHistoryBaseCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<R4Model.Resource>(this);
    }

    // GET: stu3/fhir/Patient
    //[HttpGet, Route("{resourceName}")]
    //public async Task<ActionResult<Stu3Model.Resource>> GetSearch(string resourceName)
    //{
    //  string test1 = resourceName;
    //  return StatusCode((int)HttpStatusCode.OK, GetTestPateint());
    //}

    // GET: stu3/fhir/Patient
    //[HttpGet, Route("{compartment}/{fhirId}/{resourceName}")]
    //public async Task<ActionResult<Stu3Model.Resource>> GetCompartmentSearch(string compartment, string fhirId, string resourceName)
    //{
    //  string test1 = compartment;
    //  string test2 = fhirId;
    //  string test3 = resourceName;

    //  return StatusCode((int)HttpStatusCode.OK, GetTestPateint());
    //}

    //#####################################################################
    //## |POST - CREATE| ##################################################
    //#####################################################################

    // POST: stu3/fhir/Patient
    [HttpPost("{resourceName}")]
    public async Task<ActionResult<R4Model.Resource>> Post(string resourceName, [FromBody]R4Model.Resource resource)
    {

      if (resource == null)
        return BadRequest();
      if (string.IsNullOrWhiteSpace(resourceName))
        return BadRequest();

      var Query = new Logic.Query.FhirApi.Create.CreateQuery(
        HttpVerb.POST,
        _ControllerFhirVersion,
        this.Request.GetUrl(),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        new FhirResource(_ControllerFhirVersion) { R4 = resource }
        );

      var CreateQueryHandler = this.IFhirApiQueryHandlerFactory.GetCreateCommand();
      FhirApiResult Result = await CreateQueryHandler.Handle(Query);
      return Result.PrepareResponse<R4Model.Resource>(this);
    }


    //[HttpPost, Route("{resourceName}/_search")]
    //public async Task<ActionResult<Stu3Model.Resource>> PostFormSearch(string resourceName, [FromBody] System.Net.Http.Formatting.FormDataCollection FormDataCollection)
    //{
    //  string resname = resourceName;
    //  return StatusCode((int)HttpStatusCode.OK, GetTestPateint());
    //}

    //#####################################################################
    //## |PUT - UPDATE| ###################################################
    //#####################################################################


    // PUT: stu3/fhir/Patient/100
    [HttpPut("{resourceName}/{resourceId}")]
    public async Task<ActionResult<R4Model.Resource>> Put(string resourceName, string resourceId, [FromBody] R4Model.Resource resource)
    {
      if (resource == null)
        return BadRequest();

      var command = new UpdateQuery(
        HttpVerb.PUT,
        _ControllerFhirVersion,
        this.Request.GetUrl(),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        resourceId,
        new FhirResource(_ControllerFhirVersion) { R4 = resource }
        );

      var UpdateCommandHandler = this.IFhirApiQueryHandlerFactory.GetUpdateCommand();
      FhirApiResult Result = await UpdateCommandHandler.Handle(command);
      return Result.PrepareResponse<R4Model.Resource>(this);    
    }

    

    //#####################################################################
    //##|DELETE|#############################################################
    //#####################################################################


    // DELETE: stu3/fhir/Patient/100    
    [HttpDelete("{resourceName}/{resourceId}")]
    public async Task<ActionResult<R4Model.Resource>> Delete(string resourceName, string resourceId)
    {
      var Query = new Logic.Query.FhirApi.Delete.DeleteQuery(
        HttpVerb.DELETE,
        _ControllerFhirVersion,
        this.Request.GetUrl(),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        resourceId
        );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetDeleteCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      this.Response.Headers.AddHeaders(Result.Headers);
      return Result.PrepareResponse<R4Model.Resource>(this);
    }


    
  }
}