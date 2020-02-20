extern alias Stu3;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using Bug.Common.Enums;
using Bug.Logic.Query;
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

namespace Bug.Api.Controllers
{
  //stu3/fhir
  [Route(EndpointPath.Stu3Fhir)]
  [ApiController]
  public class FhirStu3Controller : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly IFhirApiQueryHandlerFactory IFhirApiQueryHandlerFactory;
    private readonly IFhirUriFactory IFhirUriFactory;

    private readonly FhirMajorVersion _FhirMajorVersion = FhirMajorVersion.Stu3;

    public FhirStu3Controller(ILogger logger, IFhirApiQueryHandlerFactory IFhirApiQueryHandlerFactory, IFhirUriFactory IFhirUriFactory)
    {
      this.IFhirUriFactory = IFhirUriFactory;

      _logger = logger;
      //_FhirApiCommandHandler = FhirApiCommandHandler;
      this.IFhirApiQueryHandlerFactory = IFhirApiQueryHandlerFactory;
      _logger.LogInformation($"FhirStu3Controller Construtor {DateTimeOffset.Now.Offset.ToString()}");      
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
        Cap.Software = new Stu3Model.CapabilityStatement.SoftwareComponent();
        Cap.Software.Name = "FhirBug";
        //Cap.FhirVersion = Stu3Model.FHIRVersion.N4_0_0;
        Cap.Format = new List<string>() { "xml", "json" };

        Cap.ResourceBase = new Uri("http://localhost/fhir");

      }).ConfigureAwait(false);
      _logger.LogError($"Hello Metadata {DateTime.Now.ToString()}");
      return new FhirActionResult(HttpStatusCode.OK, Cap);
    }


    // GET: stu3/fhir/Patient/100
    [HttpGet, Route("{resourceName}/{fhirId}")]
    public async Task<ActionResult<Stu3Model.Resource>> Get(string resourceName, string fhirId)
    {
      string test1 = resourceName;
      string test2 = fhirId;
      return StatusCode((int)HttpStatusCode.OK, GetTestPateint());
    }

    // GET: stu3/fhir/Patient/100/_history/2
    [HttpGet, Route("{resourceName}/{fhirId}/_history/{fhirVersionid?}")]
    public async Task<ActionResult<Stu3Model.Resource>> Get(string resourceName, string fhirId, string fhirVersionId)
    {
      string test1 = resourceName;
      string test2 = fhirId;
      string test3 = fhirVersionId;
      return StatusCode((int)HttpStatusCode.OK, GetTestPateint());
    }

    // GET: stu3/fhir/Patient
    [HttpGet, Route("{resourceName}")]
    public async Task<ActionResult<Stu3Model.Resource>> GetSearch(string resourceName)
    {
      string test1 = resourceName;
      return StatusCode((int)HttpStatusCode.OK, GetTestPateint());
    }

    // GET: stu3/fhir/Patient
    [HttpGet, Route("{compartment}/{fhirId}/{resourceName}")]
    public async Task<ActionResult<Stu3Model.Resource>> GetCompartmentSearch(string compartment, string fhirId, string resourceName)
    {
      string test1 = compartment;
      string test2 = fhirId;
      string test3 = resourceName;

      return StatusCode((int)HttpStatusCode.OK, GetTestPateint());
    }

    //#####################################################################
    //##|POST|#############################################################
    //#####################################################################

    // POST: stu3/fhir/Patient
    [HttpPost("{resourceName}")]
    public async Task<ActionResult<Stu3Model.Resource>> Post(string resourceName, [FromBody]Stu3Model.Resource resource)
    {
      if (resource == null)
        return BadRequest();
      if (string.IsNullOrWhiteSpace(resourceName))
        return BadRequest();

      var Query = new Logic.Query.FhirApi.Create.CreateQuery()
      {
        FhirMajorVersion = _FhirMajorVersion,
        RequestUriString = this.Request.GetUrl(),
        RequestHeaderDictionary = new Dictionary<string, StringValues>(this.Request.Headers),
        RequestResourceName = resourceName,
        Resource = resource,
        FhirResource = new Logic.Query.FhirApi.Create.FhirResource() { Stu3 = resource }
      };

      var CreateQueryHandler = this.IFhirApiQueryHandlerFactory.GetCreateCommand();
      FhirApiResult AcceptedAtActionResult = await CreateQueryHandler.Handle(Query);

      resource.ResourceBase = new Uri("http://localhost/fhir");
      return new FhirActionResult(AcceptedAtActionResult.HttpStatusCode, AcceptedAtActionResult.Resource as Stu3Model.Resource);
    }

    [HttpPost, Route("{resourceName}/_search")]
    public async Task<ActionResult<Stu3Model.Resource>> PostFormSearch(string resourceName, [FromBody] System.Net.Http.Formatting.FormDataCollection FormDataCollection)
    {
      string resname = resourceName;
      return StatusCode((int)HttpStatusCode.OK, GetTestPateint());
    }

    //#####################################################################
    //##|PUT|#############################################################
    //#####################################################################


    // PUT: stu3/fhir/Patient/100
    [HttpPut("{resourceName}/{fhirId}")]
    public async Task<ActionResult<Stu3Model.Resource>> Put(string resourceName, string fhirId, [FromBody] Stu3Model.Resource resource)
    {
      if (resource == null)
        return BadRequest();

      var FhirUri = IFhirUriFactory.Get();
      if (FhirUri.TryParse(this.Request.GetUrl(), _FhirMajorVersion, out FhirUri))
      {
        string x = FhirUri.ResourseName;

      }

      var command = new Logic.Query.FhirApi.Update.UpdateQuery()
      {
        FhirMajorVersion = _FhirMajorVersion,
        RequestUriString = this.Request.GetUrl(),
        Resource = resource,
        FhirResource = new Logic.Query.FhirApi.Create.FhirResource() { Stu3 = resource }
      };

      var UpdateCommandHandler = this.IFhirApiQueryHandlerFactory.GetUpdateCommand();
      FhirApiResult AcceptedAtActionResult = await UpdateCommandHandler.Handle(command);

      resource.ResourceBase = new Uri("http://localhost/fhir");
      return new FhirActionResult(AcceptedAtActionResult.HttpStatusCode, AcceptedAtActionResult.Resource as Stu3Model.Resource);
    }

    //#####################################################################
    //##|DELETE|#############################################################
    //#####################################################################


    // DELETE: stu3/fhir/Patient/100    
    [HttpDelete("{resourceName}/{fhirId}")]
    public IActionResult Delete(string resourceName, string fhirId)
    {
      return NoContent();
    }

    private Stu3Model.Patient GetTestPateint()
    {
      return new Stu3Model.Patient()
      {
        Name = new List<Stu3Model.HumanName>()
         {
            new Stu3Model.HumanName()
            {
                Given = new string[] {"Angus"},
                Family = "Millar"
            }
         }
      };
    }
  }
}