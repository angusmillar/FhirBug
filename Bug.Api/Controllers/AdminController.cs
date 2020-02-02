using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bug.Api.Controllers
{
  [Route("admin")]
  [ApiController]
  public class AdminController : ControllerBase
  {
   
    // POST: api/Admin
    [HttpPost("globalConfig")]
    public void Post([FromBody] GlobalConfiguration globalConfiguration)
    {


    }

  }
}
