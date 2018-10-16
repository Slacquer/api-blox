[1]: https://github.com/dotnet/sourcelink
[2]: mailto:slacquer2018@gmail.com
[3]: ./response-objects.md
[sdk]: https://www.microsoft.com/net/download
[logo]: ./logo-blue-large.png

![:)][logo]   
# API Blox

October 15th, 2018 **v1.0.9**

## Minimal Instructions
 Solution contains the following NuGet packages.  

- APIBlox.AspNetCore  
- APIBlox.AspNetCore.CommandsAndQueries  
- APIBlox.AspNetCore.CommandsQueriesControllersOhMy
- APIBlox.AspNetCore.DynamicControllers  
- APIBlox.NetCore  
- APIBlox.NetCore.Common  
- APIBlox.NetCore.DomainEvents



## Things to keep in mind  
 - Solution requires AspNetCore 2.1.5 SDK to be installed, get it [**here**][sdk].
 Also regarding the sdk, if you are having issues adding packages like mine that use 2.1.5, be sure to 
 alter you project file first, otherwise you will end up getting the  
  **Detected package downgrade: Microsoft.AspNetCore.App from 2.1.5 to 2.\*.\* blah blah blah**  
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
    <PackageReference Include="Microsoft.AspNetCore.App"  Version="2.1.5"/>
  </ItemGroup>
</Project>
```

- All have [**SourceLink**][1] enabled.  In additon, the packages contain **xml comment** files.  
- _**None**_ of the methodology included requires full blown MVC, you can use the minimalist _**MvcCore**_.  
- I love regions, most devs don't so I have removed them here to keep you happy :)
- This document and the project(s) are a work in progress, feedback and changes would be appreciated but please be kind.
- _Also see_ [_Response Object.docx_][3] for in-depth response information.
- **I know the documentation is lacking, but I will get to at some point**, if you have questions or just need some help, my contact is at the bottom of this docuemnt.
<br>

## Quick Starts
 [Simple](#quick-start-simple)  
 [Inverted Dependencies](#quick-start-inverted-dependencies)  
 [Dynamic Controller(s)](#quick-start-dynamic-controllers)   
 [CommandsQueriesControllersOhMy Quick Start](#quick-start-commands-queries-controllers-oh-my)  
 [Commands, Queries and Decorators](#quick-start-commands-queries-and-decorators)  
 [Domain Events](#quick-start-domain-events) 

## Code

<br>

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
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, IHostingEnvironment environment)
        {
            _config = configuration;
            _loggerFactory = loggerFactory;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddInjectableServices(_loggerFactory, new[] {"MyAssemblyName"}, new[] {"MyAssemblyPath"});

            services.AddMvcCore() // Or AddMvc()
                .AddConsumesProducesJsonResourceResultFilters()
                .AddValidateResourceActionFilter()
                .AddOperationCancelledExceptionFilter()
                .AddEnsureResponseResultActionFilter()
                .AddRouteTokensConvention(_config, _environment)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseServerFaults();

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

**Instead of rolling your own**, you may want to take a look a the features found in the [CommandsQueriesControllersOhMy Quick Start](#quick-start-cqrs-ohmy)
 
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
     public class MyDynamicQueryLikeController<TRequest, TResponse, TId> : ControllerBase,
        IDynamicController<TRequest, TResponse, TId>
        where TResponse : IResource<TId>
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
        where TRequest : class
        where TResponse : IResource<TId>
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
 

 ### Quick Start Commands Queries Controllers Oh My 
 ##### *[Back to quick starts list](#quick-starts)
 
 In addition to whats found in [Quick Start Simple](#quick-start-simple) and [Dynamic Controller(s)](#quick-start-dynamic-controllers)  

Theres not much todo here, simply use the controllers that are available.  Keep in mind that the following controllers use command and query handlers [(see commands and queries quick start)](#quick-start-commands-queries-and-decorators).  If this isn't your thing, then you will need to create your own controllers.

- DynamicDeleteByController
- DynamicGenericPatchController
- DynamicPatchController
- DynamicPostController
- DynamicPutController
- DynamicQueryAllController
- DynamicQueryByController

<br>


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
                             peopleRoutes, typeof(DynamicQueryAllController<,>)
                         );
 
                         // Since our Post takes 1 more generic arg, we need to call AddController again.
                         // With that being said, we COULD simply have a GIANT single controller.
 
                         configurations.AddController<RequestResource, ResponseResource, int>(
                             peopleRoutes, typeof(DynamicPostController<,,>)
                         );
                     }
                 )
                 ...;
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
Slacquer -[email][2]


Thanks For Icon,
<div>Icons made by <a href="http://www.freepik.com" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a> is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY</a></div>
