extern alias R4;
using R4Model = R4.Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bug.Api.Controllers
{
  [Route("r4/fhir")]
  [ApiController]
  public class FhirR4Controller : ControllerBase
  {
    private readonly ILogger<FhirR4Controller> _logger;

    public FhirR4Controller(ILogger<FhirR4Controller> logger)
    {
      _logger = logger;
      _logger.LogDebug(1, "Angus NLog injected into HomeController");
    }

    // GET fhir/values   
    [HttpGet("metadata")]
    public async Task<R4Model.CapabilityStatement> GetConformance()
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
        Cap.Software = new R4Model.CapabilityStatement.SoftwareComponent();
        Cap.Software.Name = "FhirBug";
        Cap.FhirVersion = R4Model.FHIRVersion.N4_0_0;
        Cap.Format = new List<string>() { "xml", "json" };

        Cap.ResourceBase = new Uri("http://localhost/fhir");

      }).ConfigureAwait(false);
      
      return Cap;
    }

    // GET: api/Fhir
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new string[] { "value1", "value2" };
    }

    // GET: api/Fhir/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    // POST: r4/fhir
    [HttpPost("{resourceName}")]
    public async Task<ActionResult<R4Model.Resource>> Post(string resourceName, [FromBody]R4Model.Resource resource)
    {
      string resname = resourceName;
      if (resource != null)
        resource.ResourceBase = new Uri("http://localhost/fhir");
      throw new Common.Exceptions.FhirInfoException(System.Net.HttpStatusCode.BadRequest, new string[] { "error1" });
      return Ok(resource);
    }

    // PUT: api/Fhir/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] R4Model.Resource value)
    {
    }

    // DELETE: api/ApiWithActions/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
