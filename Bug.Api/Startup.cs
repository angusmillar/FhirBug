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
      container.RegisterInstance<Common.ApplicationConfig.IEnforceResourceReferentialIntegrity>(fhirServerConfig);
      
      container.Register<Common.ApplicationConfig.IServiceBaseUrlConfi, Common.ApplicationConfig.ServiceBaseUrlConfig>(Lifestyle.Singleton);      
      container.Register<Common.DateTimeTools.IServerDateTimeSupport, Common.DateTimeTools.ServerDateTimeSupport>(Lifestyle.Singleton);            

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
      container.Register<Bug.Logic.Interfaces.CompositionRoot.IFhirContainedSupportFactory, Bug.Api.CompositionRoot.FhirContainedSupportFactory>(Lifestyle.Singleton);

      //-- Singleton Factories ---------------
      container.Register<Bug.Common.Interfaces.IFhirUriFactory, Bug.Logic.UriSupport.FhirUriFactory>(Lifestyle.Singleton);


      //-- Serialization & Compression ---------------      
      container.Register<Bug.Stu3Fhir.Serialization.IStu3SerializationToJsonBytes, Bug.Stu3Fhir.Serialization.SerializationSupport>(Lifestyle.Singleton);      
      container.Register<Bug.R4Fhir.Serialization.IR4SerializationToJsonBytes, Bug.R4Fhir.Serialization.SerializationSupport>(Lifestyle.Singleton);
      container.Register<Bug.Stu3Fhir.Serialization.IStu3ParseJson, Bug.Stu3Fhir.Serialization.SerializationSupport>(Lifestyle.Singleton);
      container.Register<Bug.R4Fhir.Serialization.IR4ParseJson, Bug.R4Fhir.Serialization.SerializationSupport>(Lifestyle.Singleton);


      //-- Thread safe Tools ---------------            
      container.Register<Bug.Common.Enums.IResourceNameToTypeMap, Bug.Common.Enums.ResourceNameToTypeMap>(Lifestyle.Singleton);
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
      container.Register<Common.DateTimeTools.IFhirDateTimeFactory, Common.DateTimeTools.FhirDateTimeFactory>(Lifestyle.Singleton);
      
      var FhirDateTimeSupportRegistration = Lifestyle.Singleton.CreateRegistration<Common.DateTimeTools.FhirDateTimeSupport>(container);
      container.AddRegistration(typeof(Common.DateTimeTools.IIndexSettingCalcHighDateTime), FhirDateTimeSupportRegistration);
      container.AddRegistration(typeof(Common.DateTimeTools.ISearchQueryCalcHighDateTime), FhirDateTimeSupportRegistration);


      //--Thread safe Predicates -----------------
      container.Register<Bug.Data.Predicates.IIndexNumberPredicateFactory, Bug.Data.Predicates.IndexNumberPredicateFactory>(Lifestyle.Singleton);
      container.Register<Bug.Data.Predicates.IIndexQuantityPredicateFactory, Bug.Data.Predicates.IndexQuantityPredicateFactory>(Lifestyle.Singleton);
      container.Register<Bug.Data.Predicates.IIndexStringPredicateFactory, Bug.Data.Predicates.IndexStringPredicateFactory>(Lifestyle.Singleton);
      container.Register<Bug.Data.Predicates.IIndexTokenPredicateFactory, Bug.Data.Predicates.IndexTokenPredicateFactory>(Lifestyle.Singleton);
      container.Register<Bug.Data.Predicates.IIndexUriPredicateFactory, Bug.Data.Predicates.IndexUriPredicateFactory>(Lifestyle.Singleton);
      container.Register<Bug.Data.Predicates.IIndexDateTimePredicateFactory, Bug.Data.Predicates.IndexDateTimePredicateFactory>(Lifestyle.Singleton);
     


      //-- Thread safe FHIR Tools---------------   One Implementation many Interfaces
      var Stu3FhirResourceSupportRegistration = Lifestyle.Singleton.CreateRegistration<Bug.Stu3Fhir.ResourceSupport.FhirResourceSupport>(container);
      var R4FhirResourceSupportRegistration = Lifestyle.Singleton.CreateRegistration<Bug.R4Fhir.ResourceSupport.FhirResourceSupport>(container);

      container.AddRegistration(typeof(Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceIdSupport), Stu3FhirResourceSupportRegistration);
      container.AddRegistration(typeof(Bug.R4Fhir.ResourceSupport.IR4FhirResourceIdSupport), R4FhirResourceSupportRegistration);
      container.Register<Logic.Service.Fhir.IFhirResourceIdSupport, Logic.Service.Fhir.FhirResourceIdSupport>(Lifestyle.Singleton);

      container.AddRegistration(typeof(Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceLastUpdatedSupport), Stu3FhirResourceSupportRegistration);
      container.AddRegistration(typeof(Bug.R4Fhir.ResourceSupport.IR4FhirResourceLastUpdatedSupport), R4FhirResourceSupportRegistration);
      container.Register<Logic.Service.Fhir.IFhirResourceLastUpdatedSupport, Logic.Service.Fhir.FhirResourceLastUpdatedSupport>(Lifestyle.Singleton);

      container.AddRegistration(typeof(Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceVersionSupport), Stu3FhirResourceSupportRegistration);
      container.AddRegistration(typeof(Bug.R4Fhir.ResourceSupport.IR4FhirResourceVersionSupport), R4FhirResourceSupportRegistration);
      container.Register<Logic.Service.Fhir.IFhirResourceVersionSupport, Logic.Service.Fhir.FhirResourceVersionSupport>(Lifestyle.Singleton);

      container.AddRegistration(typeof(Bug.Stu3Fhir.ResourceSupport.IStu3FhirResourceNameSupport), Stu3FhirResourceSupportRegistration);
      container.AddRegistration(typeof(Bug.R4Fhir.ResourceSupport.IR4FhirResourceNameSupport), R4FhirResourceSupportRegistration);
      container.Register<Logic.Service.Fhir.IFhirResourceNameSupport, Logic.Service.Fhir.FhirResourceNameSupport>(Lifestyle.Singleton);

      container.AddRegistration(typeof(Bug.Stu3Fhir.ResourceSupport.IStu3IsKnownResource), Stu3FhirResourceSupportRegistration);
      container.AddRegistration(typeof(Bug.R4Fhir.ResourceSupport.IR4IsKnownResource), R4FhirResourceSupportRegistration);      
      container.Register<Bug.Logic.Service.Fhir.IKnownResource, Bug.Logic.Service.Fhir.KnownResource>(Lifestyle.Singleton);

      container.AddRegistration(typeof(Bug.Stu3Fhir.ResourceSupport.IStu3ContainedResourceDictionary), Stu3FhirResourceSupportRegistration);
      container.AddRegistration(typeof(Bug.R4Fhir.ResourceSupport.IR4ContainedResourceDictionary), R4FhirResourceSupportRegistration);
      container.Register<Logic.Service.Fhir.IFhirResourceContainedSupport, Logic.Service.Fhir.FhirResourceContainedSupport>(Lifestyle.Singleton);

      //-- Thread safe Indexing -------------
      container.Register<Bug.R4Fhir.Indexing.Setter.Support.IR4DateTimeIndexSupport, Bug.R4Fhir.Indexing.Setter.Support.R4DateTimeIndexSupport>(Lifestyle.Singleton);
      container.Register<Bug.Stu3Fhir.Indexing.Setter.Support.IStu3DateTimeIndexSupport, Bug.Stu3Fhir.Indexing.Setter.Support.Stu3DateTimeIndexSupport>(Lifestyle.Singleton);



      //############## Scoped ###############################################################################

      //-- Command &  Decorators ---------------

      //Register all ICommandHandlers
      container.Register(typeof(IQueryHandler<,>),
        AppDomain.CurrentDomain.GetAssemblies(), Lifestyle.Scoped);

      //3. Wrap all ICommandHandlers with this Decorator
      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Decorator.FhirApiQueryDecorator<,>), Lifestyle.Scoped);

      //2. Only wrap the FhirApiQueryDbTransactionDecorator around Query types with an attribute of TransactionalAttribute
      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Decorator.FhirApiQueryDbTransactionDecorator<,>), Lifestyle.Scoped,
        context => context.ImplementationType.GetCustomAttributes(false)
        .Any(a => a.GetType() == typeof(Bug.Logic.Attributes.TransactionalAttribute)));


      //1. Wrap all ICommandHandlers with this Decorator
      container.RegisterDecorator(typeof(IQueryHandler<,>),
        typeof(Bug.Logic.Query.FhirApi.Decorator.FhirApiQueryLoggingDecorator<,>), Lifestyle.Scoped);


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



      //-- Fhir Version Supports ---------------           

      container.Register<Bug.Stu3Fhir.OperationOutCome.IStu3OperationOutComeSupport, Bug.Stu3Fhir.OperationOutCome.OperationOutComeSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.OperationOutCome.IR4OperationOutComeSupport, Bug.R4Fhir.OperationOutCome.OperationOutComeSupport>(Lifestyle.Scoped);

      container.Register<Bug.Stu3Fhir.ResourceSupport.IStu3BundleSupport, Bug.Stu3Fhir.ResourceSupport.BundleSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.ResourceSupport.IR4BundleSupport, Bug.R4Fhir.ResourceSupport.BundleSupport>(Lifestyle.Scoped);

      container.Register<Bug.Stu3Fhir.Indexing.IStu3TypedElementSupport, Bug.Stu3Fhir.Indexing.Stu3TypedElementSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.Indexing.IR4TypedElementSupport, Bug.R4Fhir.Indexing.R4TypedElementSupport>(Lifestyle.Scoped);
      container.Register<Bug.R4Fhir.Indexing.Resolver.IFhirPathResolve, Bug.R4Fhir.Indexing.Resolver.FhirPathResolve>(Lifestyle.Scoped);



      //-- Scoped Fhir Services ---------------      
      container.Register<Logic.Service.Fhir.IUpdateResourceService, Logic.Service.Fhir.UpdateResourceService>(Lifestyle.Scoped);
      container.Register<Logic.Service.ValidatorService.IValidateQueryService, Logic.Service.ValidatorService.ValidateQueryService>(Lifestyle.Scoped);      
      
      container.Register<Logic.Service.Fhir.IFhirResourceJsonSerializationService, Logic.Service.Fhir.FhirResourceJsonSerializationService>(Lifestyle.Scoped);
      container.Register<Logic.Service.Fhir.IFhirResourceParseJsonService, Logic.Service.Fhir.FhirResourceParseJsonService>(Lifestyle.Scoped);
      
      
      container.Register<Logic.Service.Fhir.IOperationOutcomeSupport, Logic.Service.Fhir.OperationOutcomeSupport>(Lifestyle.Scoped);
      container.Register<Logic.Service.Fhir.IFhirResourceBundleSupport, Logic.Service.Fhir.FhirResourceBundleSupport>(Lifestyle.Scoped);
      container.Register<Logic.Service.Fhir.IHistoryBundleService, Logic.Service.Fhir.HistoryBundleService>(Lifestyle.Scoped);
      container.Register<Logic.Service.Fhir.ISearchBundleService, Logic.Service.Fhir.SearchBundleService>(Lifestyle.Scoped);
      

      //-- Scoped General Services -----------------
      container.Register<Logic.Service.ReferentialIntegrity.IReferentialIntegrityService, Logic.Service.ReferentialIntegrity.ReferentialIntegrityService>(Lifestyle.Scoped);
      container.Register<Logic.Service.Headers.IHeaderService, Logic.Service.Headers.HeaderService>(Lifestyle.Scoped);
      


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
      
      //Scoped Search Services -----------------
      container.Register<Logic.Service.SearchQuery.ISearchQueryService, Logic.Service.SearchQuery.SearchQueryService>(Lifestyle.Scoped);
      container.Register<Logic.Service.SearchQuery.ChainQuery.IChainQueryProcessingService, Logic.Service.SearchQuery.ChainQuery.ChainQueryProcessingService>(Lifestyle.Scoped);
      container.Register<Logic.Service.SearchQuery.SearchQueryEntity.ISearchQueryFactory, Logic.Service.SearchQuery.SearchQueryEntity.SearchQueryFactory>(Lifestyle.Scoped);
      container.Register<Logic.Service.Serach.ISearchService, Logic.Service.Serach.SearchService>(Lifestyle.Scoped);
      

      //-- Scoped Cache Services ---------------            
      container.Register<Logic.CacheService.IFhirVersionCache, Logic.CacheService.FhirVersionCache>(Lifestyle.Scoped);
      container.Register<Logic.CacheService.IMethodCache, Logic.CacheService.MethodCache>(Lifestyle.Scoped);
      container.Register<Logic.CacheService.IHttpStatusCodeCache, Logic.CacheService.HttpStatusCodeCache>(Lifestyle.Scoped);
      container.Register<Logic.CacheService.ISearchParameterCache, Logic.CacheService.SearchParameterCache>(Lifestyle.Scoped);
      container.Register<Common.Interfaces.CacheService.IServiceBaseUrlCache, Logic.CacheService.ServiceBaseUrlCache>(Lifestyle.Scoped);

      //--Scoped Repositories ---------------
      container.Register<IUnitOfWork, Bug.Data.AppDbContext>(Lifestyle.Scoped);
      container.Register<IResourceStoreRepository, Bug.Data.Repository.ResourceStoreRepository>(Lifestyle.Scoped);
      container.Register<IFhirVersionRepository, Bug.Data.Repository.FhirVersionRepository>(Lifestyle.Scoped);
      container.Register<IMethodRepository, Bug.Data.Repository.MethodRepository>(Lifestyle.Scoped);
      container.Register<IHttpStatusCodeRepository, Bug.Data.Repository.HttpStatusCodeRepository>(Lifestyle.Scoped);
      container.Register<ISearchParameterRepository, Bug.Data.Repository.SearchParameterRepository>(Lifestyle.Scoped);
      container.Register<IIndexReferenceRepository, Bug.Data.Repository.IndexReferenceRepository>(Lifestyle.Scoped);
      container.Register<Common.Interfaces.Repository.IServiceBaseUrlRepository, Bug.Data.Repository.ServiceBaseUrlRepository>(Lifestyle.Scoped);

      //--Scoped Predicates ---------------
      container.Register<Data.Predicates.IPredicateFactory, Data.Predicates.PredicateFactory>(Lifestyle.Scoped);
      container.Register<Bug.Data.Predicates.IResourceStorePredicateFactory, Bug.Data.Predicates.ResourceStorePredicateFactory>(Lifestyle.Scoped);
      container.Register<Bug.Data.Predicates.IIndexReferencePredicateFactory, Bug.Data.Predicates.IndexReferencePredicateFactory>(Lifestyle.Scoped);
      container.Register<Bug.Data.Predicates.IIndexCompositePredicateFactory, Bug.Data.Predicates.IndexCompositePredicateFactory>(Lifestyle.Scoped);
      



      //container.Register<Bug.Stu3Fhir.ResourceSupport.IResourceNameSupport, Bug.Stu3Fhir.ResourceSupport.ResourceNameSupport>(Lifestyle.Scoped);
      //container.Register<Bug.R4Fhir.ResourceSupport.IResourceNameSupport, Bug.R4Fhir.ResourceSupport.ResourceNameSupport>(Lifestyle.Scoped);


      // ## Transient ###################################################################

      container.Register<Logic.Service.SearchQuery.Tools.IFhirSearchQuery, Logic.Service.SearchQuery.Tools.FhirSearchQuery>(Lifestyle.Transient);

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
