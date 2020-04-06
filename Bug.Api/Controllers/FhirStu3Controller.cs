extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Update;
using Bug.Logic.Interfaces.CompositionRoot;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bug.Common.Constant;
using Bug.Api.Extensions;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.WebUtilities;

namespace Bug.Api.Controllers
{
  //stu3/fhir
  [Route(EndpointPath.Stu3Fhir)]
  [ApiController]
  public class FhirStu3Controller : ControllerBase
  {    
    private readonly IFhirApiQueryHandlerFactory IFhirApiQueryHandlerFactory;
    
    private readonly Common.Enums.FhirVersion _ControllerFhirVersion = Common.Enums.FhirVersion.Stu3;

    public FhirStu3Controller(IFhirApiQueryHandlerFactory IFhirApiQueryHandlerFactory)
    {      
      this.IFhirApiQueryHandlerFactory = IFhirApiQueryHandlerFactory;      
    }

    //#####################################################################
    //##|GET|##############################################################
    //#####################################################################

    // GET: stu3/fhir/metadata   
    [HttpGet("metadata")]
    public async Task<ActionResult<Stu3Model.Resource>> GetConformance()
    {
      var Cap = new Stu3Model.CapabilityStatement();
      await System.Threading.Tasks.Task.Run(() =>
      {
        Cap.Id = "Test1";
        Cap.Meta = new Stu3Model.Meta() { LastUpdated = DateTimeOffset.Now };
        Cap.Name = "My Test CapabilityStatment";
        Cap.Status = Stu3Model.PublicationStatus.Active;
        Cap.DateElement = new Stu3Model.FhirDateTime(DateTimeOffset.Now);
        //Cap.Kind = Stu3Model.CapabilityStatementKind.Capability;
        Cap.Software = new Stu3Model.CapabilityStatement.SoftwareComponent
        {
          Name = "FhirBug"
        };
        //Cap.FhirVersion = Stu3Model.FHIRVersion.N4_0_0;
        Cap.Format = new List<string>() { "xml", "json" };

        Cap.ResourceBase = new Uri("http://localhost/fhir");

      }).ConfigureAwait(false);     
      return this.StatusCode((int)HttpStatusCode.OK, Cap);
    }


    // GET: stu3/fhir/Patient/100
    [HttpGet, Route("{resourceName}/{resourceId}")]
    public async Task<ActionResult<Stu3Model.Resource>> Get(string resourceName, string resourceId)
    {
      var Query = new Logic.Query.FhirApi.Read.ReadQuery(
        HttpVerb.GET,
        _ControllerFhirVersion,
        this.Request.GetUrl(),
        QueryHelpers.ParseQuery(this.Request.QueryString.ToString()),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        resourceId
        );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetReadCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<Stu3Model.Resource>(this);
    }

    // GET: stu3/fhir/Patient/100/_history/2
    [HttpGet, Route("{resourceName}/{resourceId}/_history/{versionId}")]
    public async Task<ActionResult<Stu3Model.Resource>> GetVRead(string resourceName, string resourceId, int versionId)
    {
      var Query = new Logic.Query.FhirApi.VRead.VReadQuery(
       HttpVerb.GET,
       _ControllerFhirVersion,
       this.Request.GetUrl(),
       QueryHelpers.ParseQuery(this.Request.QueryString.ToString()),
       new Dictionary<string, StringValues>(this.Request.Headers),
       resourceName,
       resourceId,
       versionId
       );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetVReadCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<Stu3Model.Resource>(this);
    }

    // GET: stu3/fhir/Patient/100/_history/2
    [HttpGet, Route("{resourceName}/{resourceId}/_history")]
    public async Task<ActionResult<Stu3Model.Resource>> GetHistoryInstance(string resourceName, string resourceId)
    {
      var Query = new Logic.Query.FhirApi.HistoryInstance.HistoryInstanceQuery(
       HttpVerb.GET,
       _ControllerFhirVersion,
       this.Request.GetUrl(),
       QueryHelpers.ParseQuery(this.Request.QueryString.ToString()),
       new Dictionary<string, StringValues>(this.Request.Headers),
       resourceName,
       resourceId
       );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetHistoryInstanceCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<Stu3Model.Resource>(this);
    }

    // GET: stu3/fhir/Patient/100/_history
    [HttpGet, Route("{resourceName}/_history")]
    public async Task<ActionResult<Stu3Model.Resource>> GetHistoryResource(string resourceName)
    {
      var Query = new Logic.Query.FhirApi.HistoryResource.HistoryResourceQuery(
       HttpVerb.GET,
       _ControllerFhirVersion,
       this.Request.GetUrl(),
       QueryHelpers.ParseQuery(this.Request.QueryString.ToString()),
       new Dictionary<string, StringValues>(this.Request.Headers),
       resourceName   
       );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetHistoryResourceCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<Stu3Model.Resource>(this);
    }

    // GET: stu3/fhir/_history
    [HttpGet, Route("_history")]
    public async Task<ActionResult<Stu3Model.Resource>> GetHistoryBase()
    {
      var Query = new Logic.Query.FhirApi.HistoryBase.HistoryBaseQuery(
       HttpVerb.GET,
       _ControllerFhirVersion,
       this.Request.GetUrl(),
       QueryHelpers.ParseQuery(this.Request.QueryString.ToString()),
       new Dictionary<string, StringValues>(this.Request.Headers)       
       );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetHistoryBaseCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<Stu3Model.Resource>(this);
    }

    // GET: r4/fhir/Patient?family=millar&given=angus
    [HttpGet, Route("{resourceName}")]
    public async Task<ActionResult<Stu3Model.Resource>> GetSearch(string resourceName)
    {
      var Query = new Logic.Query.FhirApi.Search.SearchQuery(
       HttpVerb.GET,
       _ControllerFhirVersion,
       this.Request.GetUrl(),
       QueryHelpers.ParseQuery(this.Request.QueryString.ToString()),
       new Dictionary<string, StringValues>(this.Request.Headers),
       resourceName
       );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetSearchCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<Stu3Model.Resource>(this);
    }

    //#####################################################################
    //## |POST - CREATE| ##################################################
    //#####################################################################

    // POST: stu3/fhir/Patient
    [HttpPost("{resourceName}")]
    public async Task<ActionResult<Stu3Model.Resource>> Post(string resourceName, [FromBody]Stu3Model.Resource resource)
    {

      if (resource == null)
        return BadRequest();
      if (string.IsNullOrWhiteSpace(resourceName))
        return BadRequest();

      var Query = new Logic.Query.FhirApi.Create.CreateQuery(
        HttpVerb.POST,
        _ControllerFhirVersion,
        this.Request.GetUrl(),
        QueryHelpers.ParseQuery(this.Request.QueryString.ToString()),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        new Common.FhirTools.FhirResource(_ControllerFhirVersion) { Stu3 = resource }
        );

      var CreateQueryHandler = this.IFhirApiQueryHandlerFactory.GetCreateCommand();
      FhirApiResult Result = await CreateQueryHandler.Handle(Query);
      return Result.PrepareResponse<Stu3Model.Resource>(this);
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
    public async Task<ActionResult<Stu3Model.Resource>> Put(string resourceName, string resourceId, [FromBody] Stu3Model.Resource resource)
    {
      if (resource == null)
        return BadRequest();

      var command = new UpdateQuery(
        HttpVerb.PUT,
        _ControllerFhirVersion,
        this.Request.GetUrl(),
        QueryHelpers.ParseQuery(this.Request.QueryString.ToString()),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        resourceId,
        new Common.FhirTools.FhirResource(_ControllerFhirVersion) { Stu3 = resource }
        );

      var UpdateCommandHandler = this.IFhirApiQueryHandlerFactory.GetUpdateCommand();
      FhirApiResult Result = await UpdateCommandHandler.Handle(command);
      return Result.PrepareResponse<Stu3Model.Resource>(this);
    }

    //#####################################################################
    //##|DELETE|#############################################################
    //#####################################################################


    // DELETE: stu3/fhir/Patient/100    
    [HttpDelete("{resourceName}/{resourceId}")]
    public async Task<ActionResult<Stu3Model.Resource>> Delete(string resourceName, string resourceId)
    {
      var Query = new Logic.Query.FhirApi.Delete.DeleteQuery(
        HttpVerb.DELETE,
        _ControllerFhirVersion,
        this.Request.GetUrl(),
        QueryHelpers.ParseQuery(this.Request.QueryString.ToString()),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        resourceId
        );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetDeleteCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);
      return Result.PrepareResponse<Stu3Model.Resource>(this);
    }

    

  }
}