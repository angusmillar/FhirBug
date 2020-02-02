using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bug.Api.ContentFormatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Buffers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Bug.Api.Middleware;

namespace Bug.Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public static void ConfigureServices(IServiceCollection services)
    {
      // If using Kestrel:
      services.Configure<KestrelServerOptions>(options =>
      {
        options.AllowSynchronousIO = true;
      });

      // If using IIS:
      services.Configure<IISServerOptions>(options =>
      {
        options.AllowSynchronousIO = true;
      });

      services.AddControllers();
      services.AddMvcCore(config =>
      {
        //config.InputFormatters.Clear();
        config.InputFormatters.Add(new XmlFhirInputFormatter());
        config.InputFormatters.Add(new JsonFhirInputFormatter());
    
        config.OutputFormatters.Clear();
        config.OutputFormatters.Add(new XmlFhirOutputFormatter());
        config.OutputFormatters.Add(new JsonFhirOutputFormatter(ArrayPool<char>.Shared));
   

        // And include our custom content negotiator filter to handle the _format parameter
        // (from the FHIR spec:  http://hl7.org/fhir/http.html#mime-type )
        // https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters
        config.Filters.Add(new FhirFormatParameterFilter());
        config.Filters.Add(new FhirVersionParameterFilter());
        
      });
      
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseExceptionHandler("/system/error");
        //app.UseDeveloperExceptionPage();
      }
      app.UseMiddleware(typeof(ErrorHandlingMiddleware));

      app.UseHttpsRedirection();

      app.UseRouting();

      //app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
