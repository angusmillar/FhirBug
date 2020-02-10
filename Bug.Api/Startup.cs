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
using SimpleInjector;
using Bug.Logic.Command;
using Bug.Logic.Interfaces.Repository;
using Bug.Data;
//using Npgsql;
using Microsoft.EntityFrameworkCore;

namespace Bug.Api
{
  public class Startup
  {
    private Container container = new SimpleInjector.Container();
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      container.Options.ResolveUnregisteredConcreteTypes = false;
      Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
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

      // Add to the built-in ServiceCollection
      services.AddDbContext<AppDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DatabaseConnection")));

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

      services.AddSimpleInjector(container, options =>
      {
        options.AutoCrossWireFrameworkComponents = true;
        options.AddLogging();
        
        options.AddAspNetCore()
               .AddControllerActivation();                  
      });

      InitializeContainer();

    }

    private void InitializeContainer()
    {
      // Add application services. For instance:
      //container.Register<IUserService, UserService>(Lifestyle.Singleton);

      //Register all ICommandHandlers
      container.Register(typeof(ICommandHandler<,>),
        AppDomain.CurrentDomain.GetAssemblies());

      //Wrap all ICommandHandlers with this Decorator
      container.RegisterDecorator(typeof(ICommandHandler<,>),
        typeof(Bug.Logic.Command.FhirApi.Decorator.FhirApiCommandDecorator<,>));
      
      //Wrap all ICommandHandlers with this Decorator
      container.RegisterDecorator(typeof(ICommandHandler<,>),
        typeof(Bug.Logic.Command.FhirApi.Decorator.FhirApiCommandLoggingDecorator<,>));
      
      //Only wrap ICommandHandlers with this Decorator where the TCommand is an UpdateCommand
      container.RegisterDecorator(typeof(ICommandHandler<,>),
        typeof(Bug.Logic.Command.FhirApi.Update.Decorator.UpdateValidatorDecorator<,>),
        c =>
        {
          return (c.ServiceType.GenericTypeArguments[0].Name == typeof(Bug.Logic.Command.FhirApi.Update.UpdateCommand).Name);
        }
      );


      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirApiCommandHandlerFactory, Bug.Api.CompositionRoot.FhirApiCommandHandlerFactory>(Lifestyle.Singleton);
      
      
      container.Register<Bug.Stu3Fhir.IFhirResourceSupport, Bug.Stu3Fhir.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.IFhirResourceSupport, Bug.R4Fhir.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirResourceSupportFactory, Bug.Api.CompositionRoot.FhirResourceSupportFactory>(Lifestyle.Singleton);

      container.Register<Bug.Stu3Fhir.IFhirResourceIdSupport, Bug.Stu3Fhir.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.IFhirResourceIdSupport, Bug.R4Fhir.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirResourceIdSupportFactory, Bug.Api.CompositionRoot.FhirResourceIdSupportFactory>(Lifestyle.Singleton);

      container.Register<IResourceStoreRepository, Bug.Data.Repository.ResourceStoreRepository>(Lifestyle.Scoped);
      


    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseSimpleInjector(container);

      if (env.IsDevelopment())
      {        
        app.UseDeveloperExceptionPage();
      }

      app.UseMiddleware(typeof(ErrorHandlingMiddleware));

      app.UseHttpsRedirection();

      app.UseRouting();

      //app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      container.Verify();
    }


    
  }
}
