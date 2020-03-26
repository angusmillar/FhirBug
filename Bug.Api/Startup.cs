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
using AutoMapper;

namespace Bug.Api
{
  public class Startup : IDisposable
  {
    private readonly Container container = new SimpleInjector.Container();
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {   
      container.Options.ResolveUnregisteredConcreteTypes = false;
      Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddStackExchangeRedisCache(config =>
      {
        config.Configuration = "Redis";
        config.InstanceName = "BugInstance";
      });

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
      //they are later auto cross-wired to the simpleinjector container
      services.AddSingleton<Bug.Stu3Fhir.Serialization.IStu3SerializationToJson, Bug.Stu3Fhir.Serialization.SerializationSupport>();
      services.AddSingleton<Bug.Stu3Fhir.Serialization.IStu3SerializationToXml, Bug.Stu3Fhir.Serialization.SerializationSupport>();
      services.AddSingleton<Bug.R4Fhir.Serialization.IR4SerializationToJson, Bug.R4Fhir.Serialization.SerializationSupport>();
      services.AddSingleton<Bug.R4Fhir.Serialization.IR4SerializationToXml, Bug.R4Fhir.Serialization.SerializationSupport>();
      services.AddSingleton<Bug.Logic.Interfaces.CompositionRoot.IOperationOutcomeSupportFactory, Bug.Api.CompositionRoot.OperationOutComeSupportFactory>();      

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

      //-- AppSettings Configuration Loading ---------------
      Common.ApplicationConfig.FhirServerConfig fhirServerConfig = Configuration.GetSection(typeof(Common.ApplicationConfig.FhirServerConfig).Name).Get<Common.ApplicationConfig.FhirServerConfig>();
      container.RegisterInstance<Common.ApplicationConfig.IFhirServerConfig>(fhirServerConfig);
      container.RegisterInstance<Common.ApplicationConfig.IServerDefaultTimeZoneTimeSpan>(fhirServerConfig);
      container.Register<Common.ApplicationConfig.IServiceBaseUrl, Common.ApplicationConfig.ServiceBaseUrl>(Lifestyle.Singleton);      
      container.Register<Common.DateTimeTools.IServerDateTimeSupport, Common.DateTimeTools.ServerDateTimeSupport>(Lifestyle.Singleton);
      container.Register<Bug.Api.ActionResults.IActionResultFactory, Bug.Api.ActionResults.ActionResultFactory>(Lifestyle.Singleton);

      //var profiles =
      //      from t in typeof(AutoMapperRegistry).Assembly.GetTypes()
      //      where typeof(Profile).IsAssignableFrom(t)
      //      select (Profile)Activator.CreateInstance(t);

      var MapperConfig = new MapperConfiguration(cfg =>
      {
        cfg.AllowNullCollections = true;
        cfg.CreateMap<Bug.Common.Dto.Indexing.IndexDateTime, Bug.Logic.DomainModel.IndexDateTime>();
        cfg.CreateMap<Bug.Common.Dto.Indexing.IndexQuantity, Bug.Logic.DomainModel.IndexQuantity>();
        cfg.CreateMap<Bug.Common.Dto.Indexing.IndexReference, Bug.Logic.DomainModel.IndexReference>();
        cfg.CreateMap<Bug.Common.Dto.Indexing.IndexString, Bug.Logic.DomainModel.IndexString>();
        cfg.CreateMap<Bug.Common.Dto.Indexing.IndexToken, Bug.Logic.DomainModel.IndexToken>();
        cfg.CreateMap<Bug.Common.Dto.Indexing.IndexUri, Bug.Logic.DomainModel.IndexUri>();
        cfg.CreateMap<Bug.Common.Dto.Indexing.ServiceBaseUrl, Bug.Logic.DomainModel.ServiceBaseUrl>();
        cfg.CreateMap<Bug.Logic.Service.Indexing.IndexerOutcome, Bug.Logic.DomainModel.ResourceStore>();

        
      });

      container.RegisterSingleton<IMapper>(() => MapperConfig.CreateMapper(container.GetInstance));

      //-- CompositionRoot Factories ---------------
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirApiQueryHandlerFactory, Bug.Api.CompositionRoot.FhirApiQueryHandlerFactory>(Lifestyle.Singleton);            
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirResourceIdSupportFactory, Bug.Api.CompositionRoot.FhirResourceIdSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirResourceVersionSupportFactory, Bug.Api.CompositionRoot.FhirResourceVersionSupportFactory>(Lifestyle.Singleton);      
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirResourceLastUpdatedSupportFactory, Bug.Api.CompositionRoot.FhirResourceLastUpdatedSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirResourceNameSupportFactory, Bug.Api.CompositionRoot.FhirResourceNameSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IValidateResourceNameFactory, Bug.Api.CompositionRoot.ValidateResourceNameFactory>(Lifestyle.Singleton);      
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirResourceBundleSupportFactory, Bug.Api.CompositionRoot.FhirResourceBundleSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirIndexDateTimeSetterSupportFactory, Bug.Api.CompositionRoot.FhirIndexDateTimeSetterSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirIndexReferenceSetterSupportFactory, Bug.Api.CompositionRoot.FhirIndexReferenceSetterSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirIndexStringSetterSupportFactory, Bug.Api.CompositionRoot.FhirIndexStringSetterSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirIndexQuantitySetterSupportFactory, Bug.Api.CompositionRoot.FhirIndexQuantitySetterSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirIndexUriSetterSupportFactory, Bug.Api.CompositionRoot.FhirIndexUriSetterSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirIndexNumberSetterSupportFactory, Bug.Api.CompositionRoot.FhirIndexNumberSetterSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirIndexTokenSetterSupportFactory, Bug.Api.CompositionRoot.FhirIndexTokenSetterSupportFactory>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirTypedElementSupportFactory, Bug.Api.CompositionRoot.FhirTypedElementSupportFactory>(Lifestyle.Singleton);

      




      //-- Singleton Factories ---------------
      container.Register<Bug.Common.Interfaces.IFhirUriFactory, Bug.Logic.UriSupport.FhirUriFactory>(Lifestyle.Singleton);


      //-- Serialization & Compression ---------------      
      container.Register<Bug.Stu3Fhir.Serialization.IStu3SerializationToJsonBytes, Bug.Stu3Fhir.Serialization.SerializationSupport>(Lifestyle.Singleton);      
      container.Register<Bug.R4Fhir.Serialization.IR4SerializationToJsonBytes, Bug.R4Fhir.Serialization.SerializationSupport>(Lifestyle.Singleton);
      container.Register<Bug.Stu3Fhir.Serialization.IStu3ParseJson, Bug.Stu3Fhir.Serialization.SerializationSupport>(Lifestyle.Singleton);
      container.Register<Bug.R4Fhir.Serialization.IR4ParseJson, Bug.R4Fhir.Serialization.SerializationSupport>(Lifestyle.Singleton);
      

      //-- Thread safe Tools ---------------      
      container.Register<Bug.Common.Compression.IGZipper, Bug.Common.Compression.GZipper>(Lifestyle.Singleton);
      container.Register<Bug.Common.FhirTools.IResourceVersionIdSupport, Bug.Common.FhirTools.ResourceVersionIdSupport>(Lifestyle.Singleton);      
      container.Register<Bug.Stu3Fhir.ResourceSupport.IStu3ValidateResourceName, Bug.Stu3Fhir.ResourceSupport.ValidateResourceName>(Lifestyle.Singleton);
      container.Register<Bug.R4Fhir.ResourceSupport.IR4ValidateResourceName, Bug.R4Fhir.ResourceSupport.ValidateResourceName>(Lifestyle.Singleton);
      container.Register<Common.FhirTools.IResourceTypeSupport, Common.FhirTools.ResourceTypeSupport>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Service.Indexing.Setter.IDateTimeSetterSupport, Bug.Logic.Service.Indexing.Setter.DateTimeSetterSupport>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Service.Indexing.Setter.INumberSetterSupport, Bug.Logic.Service.Indexing.Setter.NumberSetterSupport>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Service.Indexing.Setter.IReferenceSetterSupport, Bug.Logic.Service.Indexing.Setter.ReferenceSetterSupport>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Service.Indexing.Setter.IStringSetterSupport, Bug.Logic.Service.Indexing.Setter.StringSetterSupport>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Service.Indexing.Setter.ITokenSetterSupport, Bug.Logic.Service.Indexing.Setter.TokenSetterSupport>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Service.Indexing.Setter.IQuantitySetterSupport, Bug.Logic.Service.Indexing.Setter.QuantitySetterSupport>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Service.Indexing.Setter.IUriSetterSupport, Bug.Logic.Service.Indexing.Setter.UriSetterSupport>(Lifestyle.Singleton);
      container.Register<Bug.Logic.Service.Indexing.ITypedElementSupport, Bug.Logic.Service.Indexing.TypedElementSupport>(Lifestyle.Singleton);
      

      //-- Thread safe Indexing -------------
      container.Register<Bug.R4Fhir.Indexing.Setter.Support.IR4DateTimeIndexSupport, Bug.R4Fhir.Indexing.Setter.Support.R4DateTimeIndexSupport>(Lifestyle.Singleton);
      container.Register<Bug.Stu3Fhir.Indexing.Setter.Support.IStu3DateTimeIndexSupport, Bug.Stu3Fhir.Indexing.Setter.Support.Stu3DateTimeIndexSupport>(Lifestyle.Singleton);



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
      
      ////Only wrap ICommandHandlers with this Decorator where the TCommand is an UpdateCommand
      //container.RegisterDecorator(typeof(IQueryHandler<,>),
      //  typeof(Bug.Logic.Query.FhirApi.Update.Decorator.UpdateValidatorQueryDecorator<,>), Lifestyle.Scoped,
      //  c =>
      //  {
      //    return (c.ServiceType.GenericTypeArguments[0].Name == typeof(Bug.Logic.Query.FhirApi.Update.UpdateQuery).Name);
      //  }
      //);

      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Decorator.FhirApiQueryDbTransactionDecorator<,>), Lifestyle.Scoped);

      //Wrap all ICommandHandlers with this Decorator
      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Decorator.FhirApiQueryLoggingDecorator<,>), Lifestyle.Scoped);


      //-- Fhir Version Supports ---------------      
      container.Register<Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceIdSupport, Bug.Stu3Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.ResourceSupport.IR4FhirResourceIdSupport, Bug.R4Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);

      container.Register<Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceVersionSupport, Bug.Stu3Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.ResourceSupport.IR4FhirResourceVersionSupport, Bug.R4Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);

      container.Register<Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceLastUpdatedSupport, Bug.Stu3Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.ResourceSupport.IR4FhirResourceLastUpdatedSupport, Bug.R4Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);

      container.Register<Bug.R4Fhir.ResourceSupport.IR4FhirResourceNameSupport, Bug.R4Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);
      container.Register<Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceNameSupport, Bug.Stu3Fhir.ResourceSupport.FhirResourceSupport>(Lifestyle.Scoped);

      container.Register<Bug.Stu3Fhir.OperationOutCome.IStu3OperationOutComeSupport, Bug.Stu3Fhir.OperationOutCome.OperationOutComeSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.OperationOutCome.IR4OperationOutComeSupport, Bug.R4Fhir.OperationOutCome.OperationOutComeSupport>(Lifestyle.Scoped);

      container.Register<Bug.Stu3Fhir.ResourceSupport.IStu3BundleSupport, Bug.Stu3Fhir.ResourceSupport.BundleSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.ResourceSupport.IR4BundleSupport, Bug.R4Fhir.ResourceSupport.BundleSupport>(Lifestyle.Scoped);

      container.Register<Bug.Stu3Fhir.Indexing.IStu3TypedElementSupport, Bug.Stu3Fhir.Indexing.Stu3TypedElementSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.Indexing.IR4TypedElementSupport, Bug.R4Fhir.Indexing.R4TypedElementSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.Indexing.Resolver.IFhirPathResolve, Bug.R4Fhir.Indexing.Resolver.FhirPathResolve>(Lifestyle.Scoped);
      


      //-- Fhir Services ---------------      
      container.Register<Logic.Service.IUpdateResourceService, Logic.Service.UpdateResourceService>(Lifestyle.Scoped);
      container.Register<Logic.Service.ValidatorService.IValidateQueryService, Logic.Service.ValidatorService.ValidateQueryService>(Lifestyle.Scoped);      
      container.Register<Logic.Service.IFhirResourceIdSupport, Logic.Service.FhirResourceIdSupport>(Lifestyle.Scoped);
      container.Register<Logic.Service.IFhirResourceJsonSerializationService, Logic.Service.FhirResourceJsonSerializationService>(Lifestyle.Scoped);
      container.Register<Logic.Service.IFhirResourceParseJsonService, Logic.Service.FhirResourceParseJsonService>(Lifestyle.Scoped);
      container.Register<Logic.Service.IFhirResourceLastUpdatedSupport, Logic.Service.FhirResourceLastUpdatedSupport>(Lifestyle.Scoped);
      container.Register<Logic.Service.IFhirResourceVersionSupport, Logic.Service.FhirResourceVersionSupport>(Lifestyle.Scoped);
      container.Register<Logic.Service.IFhirResourceNameSupport, Logic.Service.FhirResourceNameSupport>(Lifestyle.Scoped);
      container.Register<Logic.Service.IOperationOutcomeSupport, Logic.Service.OperationOutcomeSupport>(Lifestyle.Scoped);
      container.Register<Logic.Service.IFhirResourceBundleSupport, Logic.Service.FhirResourceBundleSupport>(Lifestyle.Scoped);
      container.Register<Logic.Service.FhirResourceService.IHistoryBundleService, Logic.Service.FhirResourceService.HistoryBundleService>(Lifestyle.Scoped);

      //-- Scoped Fhir Indexing -------------
      container.Register<Logic.Service.Indexing.IIndexer, Logic.Service.Indexing.Indexer>(Lifestyle.Scoped);

      container.Register<Stu3Fhir.Indexing.Setter.IStu3DateTimeSetter, Stu3Fhir.Indexing.Setter.Stu3DateTimeSetter>(Lifestyle.Scoped);
      container.Register<R4Fhir.Indexing.Setter.IR4DateTimeSetter, R4Fhir.Indexing.Setter.R4DateTimeSetter>(Lifestyle.Scoped);

      container.Register<Stu3Fhir.Indexing.Setter.IStu3NumberSetter, Stu3Fhir.Indexing.Setter.Stu3NumberSetter>(Lifestyle.Scoped);
      container.Register<R4Fhir.Indexing.Setter.IR4NumberSetter, R4Fhir.Indexing.Setter.R4NumberSetter>(Lifestyle.Scoped);

      container.Register<Stu3Fhir.Indexing.Setter.IStu3ReferenceSetter, Stu3Fhir.Indexing.Setter.Stu3ReferenceSetter>(Lifestyle.Scoped);
      container.Register<R4Fhir.Indexing.Setter.IR4ReferenceSetter, R4Fhir.Indexing.Setter.R4ReferenceSetter>(Lifestyle.Scoped);

      container.Register<Stu3Fhir.Indexing.Setter.IStu3StringSetter, Stu3Fhir.Indexing.Setter.Stu3StringSetter>(Lifestyle.Scoped);
      container.Register<R4Fhir.Indexing.Setter.IR4StringSetter, R4Fhir.Indexing.Setter.R4StringSetter>(Lifestyle.Scoped);

      container.Register<Stu3Fhir.Indexing.Setter.IStu3TokenSetter, Stu3Fhir.Indexing.Setter.Stu3TokenSetter>(Lifestyle.Scoped);
      container.Register<R4Fhir.Indexing.Setter.IR4TokenSetter, R4Fhir.Indexing.Setter.R4TokenSetter>(Lifestyle.Scoped);

      container.Register<Stu3Fhir.Indexing.Setter.IStu3QuantitySetter, Stu3Fhir.Indexing.Setter.Stu3QuantitySetter>(Lifestyle.Scoped);
      container.Register<R4Fhir.Indexing.Setter.IR4QuantitySetter, R4Fhir.Indexing.Setter.R4QuantitySetter>(Lifestyle.Scoped);

      container.Register<Stu3Fhir.Indexing.Setter.IStu3UriSetter, Stu3Fhir.Indexing.Setter.Stu3UriSetter>(Lifestyle.Scoped);
      container.Register<R4Fhir.Indexing.Setter.IR4UriSetter, R4Fhir.Indexing.Setter.R4UriSetter>(Lifestyle.Scoped);


      //-- Cache Services ---------------            
      container.Register<Logic.CacheService.IFhirVersionCache, Logic.CacheService.FhirVersionCache>(Lifestyle.Scoped);
      container.Register<Logic.CacheService.IMethodCache, Logic.CacheService.MethodCache>(Lifestyle.Scoped);
      container.Register<Logic.CacheService.IHttpStatusCodeCache, Logic.CacheService.HttpStatusCodeCache>(Lifestyle.Scoped);
      container.Register<Logic.CacheService.ISearchParameterCache, Logic.CacheService.SearchParameterCache>(Lifestyle.Scoped);
      container.Register<Common.Interfaces.CacheService.IServiceBaseUrlCache, Logic.CacheService.ServiceBaseUrlCache>(Lifestyle.Scoped);

      //-- Repositories ---------------
      container.Register<IUnitOfWork, Bug.Data.AppDbContext>(Lifestyle.Scoped);
      container.Register<IResourceStoreRepository, Bug.Data.Repository.ResourceStoreRepository>(Lifestyle.Scoped);
      container.Register<IFhirVersionRepository, Bug.Data.Repository.FhirVersionRepository>(Lifestyle.Scoped);
      container.Register<IMethodRepository, Bug.Data.Repository.MethodRepository>(Lifestyle.Scoped);
      container.Register<IHttpStatusCodeRepository, Bug.Data.Repository.HttpStatusCodeRepository>(Lifestyle.Scoped);
      container.Register<ISearchParameterRepository, Bug.Data.Repository.SearchParameterRepository>(Lifestyle.Scoped);
      container.Register<Common.Interfaces.Repository.IServiceBaseUrlRepository, Bug.Data.Repository.ServiceBaseUrlRepository>(Lifestyle.Scoped);
      



      //container.Register<Bug.Stu3Fhir.ResourceSupport.IResourceNameSupport, Bug.Stu3Fhir.ResourceSupport.ResourceNameSupport>(Lifestyle.Scoped);
      //container.Register<Bug.R4Fhir.ResourceSupport.IResourceNameSupport, Bug.R4Fhir.ResourceSupport.ResourceNameSupport>(Lifestyle.Scoped);


      // ## Transient ###################################################################



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

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
      container.Dispose();
    }
  }
}
