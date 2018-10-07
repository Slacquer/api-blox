[1]: https://github.com/dotnet/sourcelink
[3]: ./response-objects.md
[sdk]: https://www.microsoft.com/net/download/dotnet-core/2.0
[logo]: ./logo-blue-large.png

![:)][logo]   
# API Blox

## Minimal Instructions
 Solution contains the following NuGet packages.  

- APIBlox.NetCore.Common  
- APIBlox.NetCore  
- APIBlox.AspNetCore  
- APIBlox.AspNetCore.DynamicControllers  
- APIBlox.AspNetCore.CommandsAndQueries  

## Things to keep in mind  
 - Solution requires AspNetCore 2.1.4 SDK to be installed, get it [**here**][sdk] (scroll to bottom somewhere).
 Also regarding the sdk, if you are having issues adding packages like mine that use 2.1.4, be sure to 
 alter you project file first, otherwise you will end up getting the  
  **Detected package downgrade: Microsoft.AspNetCore.App from 2.1.4 to 2.1.1 blah blah blah**  
 So change your project file first, for example:  
```xml

<!-- A new project may start like this -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  ...
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
</Project>

<!-- You need to alter it to this (note the version) -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  ...
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.App"  Version="2.1.4"/>
  </ItemGroup>
</Project>
```

- All have [**SourceLink**][1] enabled.  In additon, the packages contain **xml comment** files.  
- _**None**_ of the methodology included requires full blown MVC, you can use the minimalist _**MvcCore**_.  
- I love regions, most devs don't so I have removed them here to keep you happy :)
- This document and the project(s) are a work in progress, feedback and changes would be appreciated but please be kind.

<br>

## Quick Starts
 [Simple](#quick-start-simple)  
 [Inverted Dependencies](#quick-start-inverted-dependencies)  
 [Dynamic Controller(s)](#quick-start-dynamic-controllers)   
 [Commands, Queries and Decorators](#quick-start-commands-queries-and-decorators)  
 [Domain Events](#quick-start-domain-events) 

## Code

 ### APIBlox.NetCore.Common
 Cross-cutting concerns

 ### Bits Of Interest
 _**DynamicDataObject**_  
 _Also see_ [_Response Object.docx_][3]
 ```csharp
 ```

 ### APIBlox.NetCore  
 NetCore Bits
 ```csharp
 // IServiceCollection extensions
 
 // TODO
 services.AddDependencyWithOptions<TOptions, TDependent>();
 
 // TODO
 services.AddInjectableServices();
 
 // TODO
 services.AddInvertedDependentsAndConfigureServices();
 
 // TODO
 services.AddServiceDecoration<TService>, TDecorator();
 services.AddServiceDecoration(serviceType, decoratorType);
 
 // ILogger extensions
 // These extensions are provided so that message building (which can be expensive) 
 // does not occur unless logging is enabled.  Meaning, if logging is not enabled 
 // for whichever log level, then the callback isn't called.
 
 logger.LogCritical(Func<string> messageFunc)
 logger.LogError(Func<string> messageFunc)
 logger.LogWarning(Func<string> messageFunc)
 logger.LogInformation(Func<string> messageFunc)
 ```
 
 
 ### APIBlox.AspNetCore  
 AspNetCore Bits  
 ### Middleware
 _**LameApiExplorerMiddleware**_
 Exactly as the name implies.  Yes this isn't pretty but if you want to keep the API light (as much as possible) and only use MvcCore, then you will not be able to use **Swashbuckle** as it requires full blown MVC.  This will show you your controllers, method types and actions in a sortable table (doesn't sound like much but it does actually help when figuring out WHY a route is kicking your ass).

 _**ServerFaultsMiddleware**_
 TBD

 _**SimulateWaitTimeMiddleware**_
 TBD

 ```csharp
 // IServiceCollection extensions
 
 // TODO
 services.AddMvcConvention<TConvention>();
 
 // TODO
 services.AddMvcFilter<TFilter>();
 
 // Requires an configuration entry for 
 // APIBlox.AspNetCore.Conventions.RouteTokenOptions
 services.AddMvcRouteRouteTokensConvention();
 
 // TODO
 services.AddETagActionFilter();
 
 // Adds the MVC consumes produces json resource result filters.  For either IMvcBuilder 
 // or IMvcCoreBuilder, when using IMvcCoreBuilder it will also call MvcCoreBuilders
 // .AddJsonFormatters().AddDataAnnotations() extension methods.
 services.AddMvcConsumesProducesJsonResourceResultFilters();
 
 // TODO
 services.AddMvcEnsureResponseResultActionFilter();
 
 // TODO
 services.AddMvcEnsurePaginationResultActionFilter();
 
 // TODO
 services.AddMvcOperationCancelledExceptionFilter();
 
 // Adds RFC 7807 Problem Details for HTTP APIs support.  We are using our
 // APIBlox.AspNetCore.Filters.ValidateResourceActionFilter which will
 // generate an APIBlox.AspNetCore.ActionResults.ValidationFailureResult
 // Note this filter requires that ApiBehaviorOptions.SuppressModelStateInvalidFilter = true
 // and it will be set for you in this extension method.
 services.AddValidateResourceActionFilter();
 ```
 
 ### APIBlox.AspNetCore.DynamicControllers  
 AspNetCore Bits  
 Simple dynamic controllers package that puts the focus on resources.
 
 ```csharp
 // IDynamicControllerConfigurations extensions
 
 // TODO
 configurations.AddController<TRequest, TResponse, TId>();
 configurations.AddController<TResponse, TId>(routes, controllers);
 
 
 // IDynamicControllerConfiguration extensions
 parentConfiguration.AddSubController<TRequest, TResponse, TId>();
 parentConfiguration.AddSubController<TResponse, TId>();
 
 // IMvcBuilder, IMvcCoreBuild extensions
 
 //
 builder.AddDynamicControllersFeature();
 ```
  #### Dynamic Controller MVC Conventions
  The following conventions are used  
-  DynamicControllersRouteConvention  
-  DynamicControllerSubRouteConvention  
-  DynamicControllersConvertResponseTypeConvention
 
 
 ```csharp
 // Note the excerpt here.  This is how the 
 // DynamicControllersConvertResponseTypeConvention populates your response types.
 //
 // So for example your generic controller could have a GET like so
 [HttpGet]
 [ProducesResponseType(typeof(IEnumerable), StatusCodes.Status200OK)]
 public Task<IActionResult> Get()
 {
     ...
 }
 
 // DynamicControllersConvertResponseTypeConvention.cs
 ...
var t = cType.GenericTypeArguments[cType.GenericTypeArguments.Length == 1 ? 0 : 1];

foreach (var rt in responseTypes)
{
    if (typeof(IEnumerable).IsAssignableTo(rt.Type))
        rt.Type = typeof(IEnumerable<>).MakeGenericType(t);
    else if (typeof(IResource).IsAssignableTo(rt.Type)) // Using marker interface
        rt.Type = t;
}
 ...
 ```
 
 ### APIBlox.AspNetCore.CommandsAndQueries  
 AspNetCore Bits  
  Simple CQRS implementation that utilizes the decorator pattern.
 
 ```csharp
 // IServiceCollection extensions
 
 // TODO
 services.AddCommandHandlerDecoration<THandler>();
 
 // TODO
 services.AddQueryHandlerDecoration<THandler>();
 ```
 
 ## Quick Starts
 
 ### Quick Start Simple
 ##### *[Back to quick starts list](#quick-starts)
 
 To simply get up and running without dynamic controllers or CQRS bits, but want to take advantage of things like server fault handling, services self registration, model state validation, controller route tokens, common response structure etc.
 
 - Startup.cs
 ```csharp
 using Microsoft.AspNetCore.Builder;
 using Microsoft.AspNetCore.Hosting;
 using Microsoft.AspNetCore.Mvc;
 using Microsoft.Extensions.Configuration;
 using Microsoft.Extensions.DependencyInjection;
 
 namespace MyNamespace
 {
     public class Startup
     {
         private readonly IConfiguration _configuration;
         private readonly IHostingEnvironment _hostingEnvironment;
         
         public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
         {
             _configuration = configuration;
             _hostingEnvironment = hostingEnvironment;
         }
         
         public void ConfigureServices(IServiceCollection services)
         {
             services.AddMvcCore() // Or AddMvc()
                 .AddMvcConsumesProducesJsonResourceResultFilters()
                 .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
 
             services
                 .AddInjectableServices(new[] { "MyAssemblyName" })
                 .AddValidateResourceActionFilter()
                 .AddMvcOperationCancelledExceptionFilter()
                 .AddMvcEnsureDataObjectActionFilter()
                 .AddMvcRouteRouteTokensConvention(_configuration, _hostingEnvironment);
         }
 
         public void Configure(IApplicationBuilder app, IHostingEnvironment env)
         {
             app.UseServerFaultsMiddleware();
             app.UseLameApiExplorerMiddleware();
 
             app.UseMvc();
         }
     }
 }
 ```
 
 - MyService.cs
 ```csharp
 using APIBlox.NetCore.Attributes;
 using Microsoft.Extensions.Logging;
 
 namespace MyNamespace
 {
     [InjectableService] // I default to Scoped.
     public class MyService : ISomeServiceContract
     {
         private readonly ILogger<MyService> _log;
 
         public MyService(ILoggerFactory loggerFactory)
         {
             _log = loggerFactory.CreateLogger<MyService>();
         }
         
         public void DoSomething()
         {
             _log.LogInformation(() => "I did something");
         }
     }
 }
 ```
 
 ### Quick Start Inverted Dependencies
 ##### *[Back to quick starts list](#quick-starts)
 
 In addition to whats found in [Quick Start Simple](#quick-start-simple)

 What's this for?  
 SOLID design principles tells us that Implementations should depend on abstractions and not the other way around.  So when using your favorite flavor of a _**Clean Architecture**_ you will find that since your abstractions (**A**) (interfaces) live at the application layer, and your implementations (**B**) (for example an repository class) live at the infrastructure layer.  Therefore **B** has a reference to **A** and not the other way around.  Then comes the presentation layer (**C**).  It typically only has a reference(s) to the application layer (and perhaps cross-cutting but forget that for now).  So **C** has a reference to **A** but how can your startup.cs file add things like _services.AddSingleton<IAbstraction,ConcreteImplementation()_?!?!
 
 _Thats where **AddInvertedDependentsAndConfigureServices** comes in to play...__
 
 - Startup.cs
 ```csharp
 using Microsoft.Extensions.DependencyInjection;
 using Microsoft.Extensions.Logging;
 
 namespace MyNamespace
 {
     public class Startup
     {
         private readonly ILoggerFactory _loggerFactory;
 
         public Startup(... 
             ILoggerFactory loggerFactory
             ...
         )
         {
             _loggerFactory = loggerFactory;
         }
         
         public void ConfigureServices(IServiceCollection services)
         {
             services
                 .AddInvertedDependentsAndConfigureServices(
                     _configuration,
                     _loggerFactory,
                     _hostingEnvironment.EnvironmentName, 
                     new[] { "AssemblyNameLike" }, 
                     new[] { "../../SomeOtherAssemblyPath" }
                 )
                 ...;
         }
     }
 }
 ```
 
 - MyInvertedAssembliesConfiguration.cs
 ```csharp
 using System;
 using APIBlox.NetCore.Contracts;
 using Microsoft.Extensions.Configuration;
 using Microsoft.Extensions.DependencyInjection;
 using Microsoft.Extensions.Logging;
 
 namespace SomeOtherAssembly
 {
     public class MyInvertedAssembliesConfiguration : IDependencyInvertedConfiguration
     {
         private ILogger<MyInvertedAssembliesConfiguration> _log;
 
         public void Configure(IServiceCollection services, IConfiguration configuration,
             ILoggerFactory loggerFactory, string environment
         )
         {
             // Configure my services, add options, etc.
 
             _log = loggerFactory.CreateLogger<MyInvertedAssembliesConfiguration>();
 
             _log.LogInformation(() => "I'm fully configured.");
         }
     }
 }
 ```
 
 
 ### Quick Start Dynamic Controllers 
 ##### *[Back to quick starts list](#quick-starts)
 
 In addition to whats found in [Quick Start Simple](#quick-start-simple)
 
 - Startup.cs
 ```csharp
 namespace MyNamespace
 {
     public class Startup
     {
         public void ConfigureServices(IServiceCollection services)
         {
             services.AddMvcCore() // Or AddMvc()
                 ...
                 .AddDynamicControllersFeature(configurations =>
                     {
                         var peopleRoutes = new[]
                         {
                             "api/[environment]/[version]/people",
                             //
                             // For those that believe routes should be nothing but
                             // resources, who says versions isn't a resource?!
                             "api/[environment]/versions/[version]/people"
                         };
 
                         configurations.AddController<ResponseResource, int>(
                             peopleRoutes, typeof(MyDynamicQueryLikeController<,>)
                         );
 
                         // Since our Post takes 1 more generic arg, we need to call AddController again.
                         // With that being said, we COULD simply have a GIANT single controller.
 
                         configurations.AddController<RequestResource, ResponseResource, int>(
                             peopleRoutes, typeof(MyDynamicCommandLikeController<,,>)
                         );
                     }
                 )
                 ...;
         }
     }
 }
 ```
 
 - RequestResource.cs
 ```csharp
 using System.ComponentModel.DataAnnotations;
 using APIBlox.AspNetCore.Contracts;
 
 namespace MyNamespace
 {
     public class RequestResource : IResource
     {
         [Required]
         public string Property1 { get; set; }
     }
 }
 ```
 
 - ResponseResource.cs
 ```csharp
 using APIBlox.AspNetCore.Contracts;
 
 namespace MyNamespace
 {
     public class ResponseResource : RequestResource, IResource<int
     {
         public int Id { get; set; }
     }
 }
 ```
 
 - MyDynamicQueryLikeController.cs
 ```csharp
 using System.Collections;
 using System.Collections.Generic;
 using System.Threading.Tasks;
 using APIBlox.AspNetCore.Contracts;
 using Microsoft.AspNetCore.Http;
 using Microsoft.AspNetCore.Mvc;
 
 namespace MyNamespace
 {
     [Route("api/[controller]")]
     [ApiController]
     public class MyDynamicQueryLikeController<TResponse, TId> : ControllerBase,
         IDynamicController<TResponse, TId>
         where TResponse : class, IResource<TId>
     {
         [HttpGet]
         //
         // The APIBlox.AspNetCore.Conventions.DynamicControllersConvertResponseTypeConvention
         // will convert to IEnumerable<TResponse>
         [ProducesResponseType(typeof(IEnumerable), StatusCodes.Status200OK)]
         //
         [ProducesResponseType(StatusCodes.Status204NoContent)]
         public Task<IActionResult Get()
         {
             ...
         }
 
         [HttpGet("{id}")]
         //
         // The APIBlox.AspNetCore.Conventions.DynamicControllersConvertResponseTypeConvention
         // will convert to TResponse (Actually you can just use the IResource marker as well).
         [ProducesResponseType(typeof(IResource<>), StatusCodes.Status200OK)]
         //
         [ProducesResponseType(StatusCodes.Status404NotFound)]
         public ResponseResource Get(TId id)
         {
             ...
         }
     }
 }
 ```
 
 - MyDynamicCommandLikeController.cs
 ```csharp
 using System.Threading.Tasks;
 using APIBlox.AspNetCore.Contracts;
 using Microsoft.AspNetCore.Http;
 using Microsoft.AspNetCore.Mvc;
 
 namespace MyNamespace
 {
     [Route("api/[controller]")]
     [ApiController]
     public class MyDynamicCommandLikeController<TRequest, TResponse, TId> : ControllerBase,
         IDynamicController<TRequest, TResponse, TId>
         where TResponse : class, IResource<TId>, new()
         where TRequest : class, IResource
     {
         [HttpPost]
         [ProducesResponseType(typeof(IResource<>), StatusCodes.Status201Created)]
         [ProducesResponseType(StatusCodes.Status409Conflict)]
         public Task<IActionResult> Post(TRequest value)
         {
             ...
         }
     }
 }
 ```
 
 ### Quick Start Commands Queries And Decorators
 ##### *[Back to quick starts list](#quick-starts)
 
 In addition to whats found in [Quick Start Simple](#quick-start-simple) and [Dynamic Controller(s)](#quick-start-dynamic-controllers)
 
 - Startup.cs
 ```csharp
 // TODO
 ```
 
 ### Quick Start Domain Events
 ##### *[Back to quick starts list](#quick-starts)
 
 In addition to whats found in [Quick Start Simple](#quick-start-simple) and [Commands Queries And Decorators](#quick-start-commands-queries-and-decorators)
 
 - Startup.cs
 ```csharp
 // TODO
 ```

<br>

#### Thanks for having a look :)
_My hope is that these packages may help someone other than myself_.  
Thanks,    
Slacquer


Thanks For Icon,
<div>Icons made by <a href="http://www.freepik.com" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a> is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY</a></div>