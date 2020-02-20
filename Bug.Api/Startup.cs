extern alias Stu3;
extern alias R4;
using Stu3Model = Stu3.Hl7.Fhir.Model;
using R4Model = R4.Hl7.Fhir.Model;
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
using Bug.Logic.Query;
using Bug.Logic.Interfaces.Repository;
using Bug.Data;
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

      services.AddDistributedMemoryCache();
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

      services.AddMemoryCache();

      //These services are added to the .Net Core DI framework as they are required for a Middleware component
      //they are later auto crosswired to the simpleinjector container
      services.AddSingleton<Bug.Stu3Fhir.Serialization.IStu3SerializationToJson, Bug.Stu3Fhir.Serialization.SerializationSupport>();
      services.AddSingleton<Bug.Stu3Fhir.Serialization.IStu3SerializationToXml, Bug.Stu3Fhir.Serialization.SerializationSupport>();
      services.AddSingleton<Bug.R4Fhir.Serialization.IR4SerializationToJson, Bug.R4Fhir.Serialization.SerializationSupport>();
      services.AddSingleton<Bug.R4Fhir.Serialization.IR4SerializationToXml, Bug.R4Fhir.Serialization.SerializationSupport>();
      services.AddSingleton<Bug.Logic.Interfaces.CompositionRoot.IOperationOutComeSupportFactory, Bug.Api.CompositionRoot.OperationOutComeSupportFactory>();      

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
      //############## Singleton ###############################################################################

      //-- AppSettings Conofiguration Loading ---------------
      Common.ApplicationConfig.FhirServerConfig fhirServerConfig = Configuration.GetSection(typeof(Common.ApplicationConfig.FhirServerConfig).Name).Get<Common.ApplicationConfig.FhirServerConfig>();
      container.RegisterInstance<Common.ApplicationConfig.IFhirServerConfig>(fhirServerConfig);
      container.Register<Common.ApplicationConfig.IServiceBaseUrl, Common.ApplicationConfig.ServiceBaseUrl>(Lifestyle.Singleton);

      //-- CompositionRoot Factories ---------------
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirApiQueryHandlerFactory, Bug.Api.CompositionRoot.FhirApiQueryHandlerFactory>(Lifestyle.Singleton);            
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirResourceIdSupportFactory, Bug.Api.CompositionRoot.FhirResourceIdSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirResourceVersionSupportFactory, Bug.Api.CompositionRoot.FhirResourceVersionSupportFactory>(Lifestyle.Singleton);      
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirResourceLastUpdatedSupportFactory, Bug.Api.CompositionRoot.FhirResourceLastUpdatedSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IValidateResourceNameFactory, Bug.Api.CompositionRoot.ValidateResourceNameFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirUriFactory, Bug.Api.CompositionRoot.FhirUriFactory>(Lifestyle.Singleton);
      

      //-- Serialization & Compression ---------------      
      container.Register<Bug.Stu3Fhir.Serialization.IStu3SerializationToJsonBytes, Bug.Stu3Fhir.Serialization.SerializationSupport>(Lifestyle.Singleton);      
      container.Register<Bug.R4Fhir.Serialization.IR4SerializationToJsonBytes, Bug.R4Fhir.Serialization.SerializationSupport>(Lifestyle.Singleton);

      //-- Thread safe Tools ---------------      
      container.Register<Bug.Common.Compression.IGZipper, Bug.Common.Compression.GZipper>(Lifestyle.Singleton);
      container.Register<Bug.Common.FhirTools.IResourceVersionIdSupport, Bug.Common.FhirTools.ResourceVersionIdSupport>(Lifestyle.Singleton);
      //container.Register<Bug.R4Fhir.ResourceSupport.IResourceNameSupport, Bug.R4Fhir.ResourceSupport.ResourceNameSupport>(Lifestyle.Singleton);
      container.Register<Bug.Stu3Fhir.ResourceSupport.IValidateResourceName, Bug.Stu3Fhir.ResourceSupport.ValidateResourceName>(Lifestyle.Singleton);
      container.Register<Bug.R4Fhir.ResourceSupport.IValidateResourceName, Bug.R4Fhir.ResourceSupport.ValidateResourceName>(Lifestyle.Singleton);


      //############## Scoped ###############################################################################

      //-- Command &  Decorators ---------------

      //Register all ICommandHandlers
      container.Register(typeof(IQueryHandler<,>),
        AppDomain.CurrentDomain.GetAssemblies(), Lifestyle.Scoped);

      //Wrap all ICommandHandlers with this Decorator
      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Decorator.FhirApiQueryDecorator<,>), Lifestyle.Scoped);

      //Only wrap ICommandHandlers with this Decorator where the TCommand is an CreateCommand
      //container.RegisterDecorator(typeof(IQueryHandler<,>),
      //  typeof(Bug.Logic.Query.FhirApi.Create.Decorator.CreateDataCollectionDecorator<,>), Lifestyle.Scoped,
      //  c =>
      //  {
      //    return (c.ServiceType.GenericTypeArguments[0].Name == typeof(Bug.Logic.Query.FhirApi.Create.CreateQuery).Name);
      //  }
      //);
      
      //Only wrap ICommandHandlers with this Decorator where the TCommand is an CreateCommand
      //container.RegisterDecorator(typeof(IQueryHandler<,>),
      //  typeof(Bug.Logic.Query.FhirApi.Create.Decorator.CreateValidatorDecorator<,>), Lifestyle.Scoped,
      //  c =>
      //  {
      //    return (c.ServiceType.GenericTypeArguments[0].Name == typeof(Bug.Logic.Query.FhirApi.Create.CreateQuery).Name);
      //  }
      //);

      //Only wrap ICommandHandlers with this Decorator where the TCommand is an UpdateCommand
      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Update.Decorator.UpdateDataCollectionQueryDecorator<,>), Lifestyle.Scoped,
        c =>
        {
          return (c.ServiceType.GenericTypeArguments[0].Name == typeof(Bug.Logic.Query.FhirApi.Update.UpdateQuery).Name);
        }
      );

      //Only wrap ICommandHandlers with this Decorator where the TCommand is an UpdateCommand
      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Update.Decorator.UpdateDataCollectionQueryDecorator<,>), Lifestyle.Scoped,
        c =>
        {
          return (c.ServiceType.GenericTypeArguments[0].Name == typeof(Bug.Logic.Query.FhirApi.Update.UpdateQuery).Name);
        }
      );


      //Only wrap ICommandHandlers with this Decorator where the TCommand is an UpdateCommand
      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Update.Decorator.UpdateValidatorQueryDecorator<,>), Lifestyle.Scoped,
        c =>
        {
          return (c.ServiceType.GenericTypeArguments[0].Name == typeof(Bug.Logic.Query.FhirApi.Update.UpdateQuery).Name);
        }
      );

      //Only wrap ICommandHandlers with this Decorator where the TCommand is an UpdateCommand
      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Update.Decorator.UpdateValidatorQueryDecorator<,>), Lifestyle.Scoped,
        c =>
        {
          return (c.ServiceType.GenericTypeArguments[0].Name == typeof(Bug.Logic.Query.FhirApi.Update.UpdateQuery).Name);
        }
      );

      //Wrap all ICommandHandlers with this Decorator
      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Decorator.FhirApiQueryLoggingDecorator<,>), Lifestyle.Scoped);


      //-- Fhir Version Supports ---------------      
      container.Register<Bug.Stu3Fhir.ResourceSupport.IFhirResourceIdSupport, Bug.Stu3Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.ResourceSupport.IFhirResourceIdSupport, Bug.R4Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);

      container.Register<Bug.Stu3Fhir.ResourceSupport.IFhirResourceVersionSupport, Bug.Stu3Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.ResourceSupport.IFhirResourceVersionSupport, Bug.R4Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);

      container.Register<Bug.Stu3Fhir.ResourceSupport.IFhirResourceLastUpdatedSupport, Bug.Stu3Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.ResourceSupport.IFhirResourceLastUpdatedSupport, Bug.R4Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);

      container.Register<Bug.Stu3Fhir.OperationOutCome.IStu3OperationOutComeSupport, Bug.Stu3Fhir.OperationOutCome.OperationOutComeSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.OperationOutCome.IR4OperationOutComeSupport, Bug.R4Fhir.OperationOutCome.OperationOutComeSupport>(Lifestyle.Scoped);
     
      //-- Repositories ---------------
      container.Register<IResourceStoreRepository, Bug.Data.Repository.ResourceStoreRepository>(Lifestyle.Scoped);

      //container.Register<Bug.Stu3Fhir.ResourceSupport.IResourceNameSupport, Bug.Stu3Fhir.ResourceSupport.ResourceNameSupport>(Lifestyle.Scoped);
      //container.Register<Bug.R4Fhir.ResourceSupport.IResourceNameSupport, Bug.R4Fhir.ResourceSupport.ResourceNameSupport>(Lifestyle.Scoped);


      // ## Transient ###################################################################
      container.Register<Bug.Logic.UriSupport.IFhirUri, Bug.Logic.UriSupport.FhirUri>(Lifestyle.Transient);
      

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
