extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Bug.Common.Enums;
using Bug.Common.FhirTools;
using Bug.Logic.Query.FhirApi;
using Bug.Logic.Query.FhirApi.Update;
using Bug.Logic.Interfaces.CompositionRoot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bug.Common.Constant;
using Bug.Api.Extensions;
using Microsoft.Extensions.Primitives;
using Bug.Api.ActionResults;

namespace Bug.Api.Controllers
{
  //stu3/fhir
  [Route(EndpointPath.Stu3Fhir)]
  [ApiController]
  public class FhirStu3Controller : ControllerBase
  {    
    private readonly IFhirApiQueryHandlerFactory IFhirApiQueryHandlerFactory;
    private readonly IActionResultFactory IActionResultFactory;


    private readonly FhirMajorVersion _ControllerFhirMajorVersion = FhirMajorVersion.Stu3;

    public FhirStu3Controller(IFhirApiQueryHandlerFactory IFhirApiQueryHandlerFactory, IActionResultFactory IActionResultFactory)
    {      
      this.IFhirApiQueryHandlerFactory = IFhirApiQueryHandlerFactory;
      this.IActionResultFactory = IActionResultFactory;      
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
      return new FhirStu3ResourceActionResult(HttpStatusCode.OK, Cap);
    }


    // GET: stu3/fhir/Patient/100
    [HttpGet, Route("{resourceName}/{resourceId}")]
    public async Task<ActionResult<Stu3Model.Resource>> Get(string resourceName, string resourceId)
    {
      var Query = new Logic.Query.FhirApi.Read.ReadQuery(
        HttpVerb.PUT,
        _ControllerFhirMajorVersion,
        this.Request.GetUrl(),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        resourceId
        );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetReadCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);

      return IActionResultFactory.Get(Result);
    }

    // GET: stu3/fhir/Patient/100/_history/2
    [HttpGet, Route("{resourceName}/{resourceId}/_history/{versionId?}")]
    public async Task<ActionResult<Stu3Model.Resource>> Get(string resourceName, string resourceId, int versionId)
    {
      var Query = new Logic.Query.FhirApi.VRead.VReadQuery(
       HttpVerb.PUT,
       _ControllerFhirMajorVersion,
       this.Request.GetUrl(),
       new Dictionary<string, StringValues>(this.Request.Headers),
       resourceName,
       resourceId,
       versionId
       );

      var ReadQueryHandler = this.IFhirApiQueryHandlerFactory.GetVReadCommand();
      FhirApiResult Result = await ReadQueryHandler.Handle(Query);

      return IActionResultFactory.Get(Result);
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
    public async Task<ActionResult<Stu3Model.Resource>> Post(string resourceName, [FromBody]Stu3Model.Resource resource)
    {

      if (resource == null)
        return BadRequest();
      if (string.IsNullOrWhiteSpace(resourceName))
        return BadRequest();

      var Query = new Logic.Query.FhirApi.Create.CreateQuery(
        HttpVerb.POST,
        _ControllerFhirMajorVersion,
        this.Request.GetUrl(),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        new FhirResource(_ControllerFhirMajorVersion) { Stu3 = resource }
        );

      var CreateQueryHandler = this.IFhirApiQueryHandlerFactory.GetCreateCommand();
      FhirApiResult Result = await CreateQueryHandler.Handle(Query);
      return IActionResultFactory.Get(Result);
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
        _ControllerFhirMajorVersion,
        this.Request.GetUrl(),
        new Dictionary<string, StringValues>(this.Request.Headers),
        resourceName,
        resourceId,
        new FhirResource(_ControllerFhirMajorVersion) { Stu3 = resource }
        );

      var UpdateCommandHandler = this.IFhirApiQueryHandlerFactory.GetUpdateCommand();
      FhirApiResult Result = await UpdateCommandHandler.Handle(command);      
      return IActionResultFactory.Get(Result);
    }

    //#####################################################################
    //##|DELETE|#############################################################
    //#####################################################################


    // DELETE: stu3/fhir/Patient/100    
    //[HttpDelete("{resourceName}/{fhirId}")]
    //public IActionResult Delete(string resourceName, string fhirId)
    //{
    //  return NoContent();
    //}


  }
}