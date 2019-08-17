_August 17th, 2019_  **v2.1.19**
- APIBlox.AspNetCore.Types.Query - Exposed Map to children.
- APIBlox.AspNetCore.DynamicControllerFactory - Will now output CS files for each controller (if not release), allowing debugging to take place.  **Removed** Compile overload.
- **BUG** APIBlox.AspNetCore.Attributes.FromQueryWithAlternateNamesAttribute - currently the dynamic controller(s) code only supports PARAMS in constructor args, so this needed to be changed in the attribute itself.

Thanks,  
_Slacquer_  

<br>
<br>

_August 16th, 2019_  **v2.1.17**
- APIBlox.AspNetCore.Types - Seems we needed a model binder for alternate names with IQuery object maps.  So I added the following:
- _APIBlox.AspNetCore.Attributes.FromQueryWithAlternateNamesAttribute_
- _AddFromQueryWithAlternateNamesBinder_ extension method.

Thanks,  
_Slacquer_  

<br>
<br>


_August 15th, 2019_  **v2.1.16**
- APIBlox.AspNetCore.Types - Added several Query objects.

Thanks,  
_Slacquer_  

<br>
<br>

_August 13th, 2019_  **v2.1.15**
- APIBlox.AspNetCore.Types - Added IProjectedQuery and ProjectedQuery, and changed the inheritancce around a bit.  This was done to allow a GET for example to retrieve a single result, with could be projected, previously the ORDERBY and FILTER mechanisms made no sense in a single result response.
- APIBlox.NetCore.EventStore.EventStoreDocument - changed the Data to be an object rather than string, for searching.  This required making changes to _APIBlox.NetCore.EventStore.EfCore_ as well, since it can NOT use a object for a property (column).  This is a big breaking change as stored event data will no longer work.
- 
Apologies,  
_Slacquer_  

<br>
<br>

_August 11th, 2019_  **v2.1.14**
- APIBlox.NetCore.Decorators.Commands.RetryCommandHandlerDecorator, WoW! did not break if successful.
- APIBlox.AspNetCore.TypesHandlerResponseExtensions, changed comments and method names to reflect what is in _APIBlox.AspNetCore.Enums.CommonStatusCodes_.
- APIBlox.AspNetCore.ServerFaultsMiddleware, A user pointed out to me that it would be nice to see a stack trace, therefore it has been added.  The extension method _UseServerFaults_ has had a parameter added that defaults to true.  Keep in mind this is only the USER code stack trace, if you need to see a full trace, then set to false and use the AddAlterRequestErrorObject extension method to do it yourself (see the _Examples.Featers.Startup.ConfigureServices_ method for an example).

Thanks,  
_Slacquer_  

<br>
<br>

_August 8th, 2019_  **v2.1.13**
- APIBlox.NetCore.EventStore, Added default contract resolver to be _APIBlox.NetCore.Types.JsonBits.PopulateNonPublicSettersContractResolver_, and updated extension method(s).

Thanks,  
_Slacquer_  

<br>
<br>

_August 8th, 2019_  **v2.1.12**
- APIBlox.AspNetCore, changed CommonStatusCode, names and attributes.
- APIBlox.AspNetCore.CommandsQueriesControllersOhMy, Added 400 to response codes for POST, POSTACCEPTED, PUT, PATCH

Thanks,  
_Slacquer_  

<br>
<br>

_August 8th, 2019_  **v2.1.11**
- APIBlox.AspNetCore.DynamicControllers DynamicController, Turns out the templates for POST were causing this issue, it had bits it shouldnt have had!

Thanks,  
_Slacquer_  

<br>
<br>

_August 8th, 2019_  **v2.1.10**
- APIBlox.AspNetCore.DynamicControllers DynamicController, was allowing duplicate entries in templates, added distinct.

Thanks,  
_Slacquer_  

<br>
<br>

_August 8th, 2019_  **v2.1.9**
- MetricsCommandHandlerDecorator(s), Changed logger to reflect WHAT its wrapping.
- RetryCommandHandlerDecorator, Added, simply retry twice with a 1 second delay.
- APIBlox.AspNetCore.CommandsQueriesControllersOhMy Post/CtorBody template, had a log decleration and should not have!
 
Thanks,  
_Slacquer_  

<br>
<br>

_August 7th, 2019_  **v2.1.8**
- APIBlox.NetCore.EventStore, method BuildEventModel was being used incorrectly thus causing deserialization to occur twice.  Sadly the Data element of the event store document is of type string becuase of EFCore requiring a type OTHER than object.  I need to move this functionality OUT, or better yet require the providers to provide their own document object.

Thanks,  
_Slacquer_  

<br>
<br>

_August 5th, 2019_  **v2.1.7**
- APIBlox.NetCore.EventStore.CosmosDb, changed configuration options to allow using a name for the collection id.

Thanks,  
_Slacquer_  

<br>
<br>

_August 3rd, 2019_  **v2.1.6**
- APIBlox.AspNetCore.CommandsQueriesControllersOhMy, missing namespace in template.  Whoops, bad merge!

Thanks,  
_Slacquer_  

<br>
<br>

_August 3rd, 2019_  **v2.1.5**
- APIBlox.AspNetCore.CommandsQueriesControllersOhMy, missing namespace in template.
- Packages updates.

Thanks,  
_Slacquer_  

<br>
<br>

_July 24th, 2019_  **v2.1.3**
- APIBlox.AspNetCore.CommandsAndQueries, all ASPNET dependencies have been removed/moved, as technically this should NEVER have been an ASPNET project, its a .NETCORE thing.
- It now has the name **APIBlox.NetCore.CommandsAndQueries**
- This also means that the templates in *APIBlox.AspNetCore.CommandsQueriesControllersOhMy* had to be changed (namespace changes only).  Therefore if using dyncamic controllers, then be sure to let them get regenerated (turn off cache).
 
- AddQueryHandlerDecoration & AddCommandHandlerDecoration(s) - failed when decorators were not GENERIC (as they shouldn't need to be).
 
Thanks,  
_Slacquer_  

<br>
<br>

_June 10th, 2019_  **v2.0.33**
- PostLocationHeaderResultFilter - Changed Critical message to warning.
- APIBlox.AspNetCore.CommandsQueriesControllersOhMy.Templates.Post - Changed fail message to warning.
  
Thanks,  
_Slacquer_  

<br>
<br>


_June 10th, 2019_  **v2.0.32**
- IReadOnlyEventStoreService.ReadEventStreamVersionAsync, will now return NULL when a stream is not found.
- ComposedTemplatesExtensions.WriteQueryByController, was too restrictive, as it required an actionRoute with tokens.
  
Thanks,  
_Slacquer_  

<br>
<br>


_June 10th, 2019_  **v2.0.30**
- DynamicControllerTemplateOptions, routes are camel cased when set.
- ComposedTemplatesExtensions, it was not exactly clear but by design ALL actions generated return a HandlerResponse, therefore the enduser MUST have query/command handlers that return HandlerResponse.
- ServiceCollectionExtensionsCommandsAndQueries, command and query decorators now wrap each implemented instance (like many-to-many). 

Thanks,  
_Slacquer_  

<br>
<br>

_May 15th, 2019_  **v2.0.29**
- DynamicControllerTemplateOptions, defaults for comments.
- Nuget Packages update(s).

Thanks,  
_Slacquer_  

<br>
<br>

_May 15th, 2019_  **v2.0.28**
- DynamicControllerTemplateOptions, makes extension methods cleaner.  Not to mention I will changing how comments are added to actions most likely through this class.
- QueryalbExtensions, added.
- REMARKS, added to controller/action templates.

Thanks,  
_Slacquer_  

<br>
<br>

_May 9th, 2019_  **v2.0.27**
- DynamicController templates, Missing System namespace using statement, this has only been an issue when a request object does not contain anything other than something from body.
 
Thanks,  
_Slacquer_  

<br>
<br>

_May 2nd, 2019_  **v2.0.26**
- DynamicController templates, bug for whatever reason I was not compensating for OTHER codes.
 
Thanks,  
_Slacquer_  

<br>
<br>

_April 29th, 2019_  **v2.0.24**
- PaginationResult, seems when the results coming back are LESS than the max allowed we were always returning null for next and previous, but we did not take into consideration that this may NOT have been the first call, therefore the PREVIOUS needed to be filled in.
 
Thanks,  
_Slacquer_  

<br>
<br>

_April 29th, 2019_  **v2.0.23**
- Added an EmptyStringToNullConverter
- SimpleMapper, needed to check if incoming src was already a json string.  Removed overload.
- CommandsQueriesControllersOhMy - changed comments in templates to use markdwon syntax for possible response codes.

Thanks,  
_Slacquer_  

<br>
<br>

_April 13th, 2019_  **v2.0.21**
- EnsureResponseResultActionFilter - failing when getsonly true.

Thanks,  
_Slacquer_  

<br>
<br>

_April 11th, 2019_  **v2.0.19**
- DynamicControllerFactory - not correctly setting outputs on successful compile.
- DynamicControllerFactory - caching is no longer being used (here) it is now done soley in the AddDynamicControllerConfigurations extension method.
- EventStore EfCore  - Added create migration for EventStoreDocuments table.
- Examples.EventSourcing - refactored.
Thanks,  
_Slacquer_  

<br>
<br>

_April 10th, 2019_  **v2.0.16**
- BUG! DynamicControllerFactory.EmitToFile - was NOT returning the newly created FileInfo.
- AddDynamicControllerConfigurations - No longer throws when templates is null, will simply log critical message.
 
Thanks,  
_Slacquer_  

<br>
<br>

_April 9th, 2019_  **v2.0.15**
- DynamicDataObject - Added RemoveProperty overloads.
- HandlerResponseExtensions - CreateError is obsolete, added direct extension methods for RequestErrorObjectExtensions.
 
Thanks,  
_Slacquer_  

<br>
<br>

_April 7th, 2019_  **v2.0.14**
- ServerFaultsMiddleware - removed stack trace from result.  However added callback to the extension method to allow consumer to do what they want with the error object prior to returning results, like add a stack trace :)
- ValidationFailureResult - will no longer throw when errors are empty, it will just reset the "Detail" to "Errors have occured."
- EnsureResponseResultActionFilter - Will not wrap when status codes are >= 300 or < 200.

Thanks,  
_Slacquer_  

<br>
<br>

_April 7th, 2019_  **v2.0.13**
- DynamicControllerFactroy, refactoring and using CodeDom instead of home grown code for input parameters.  In time I will likely remove templates altogether.

Thanks,  
_Slacquer_  

<br>
<br>

_April 6th, 2019_  **v2.0.12**
- BUG: seems when DynamicControllerFactory.EmitToFile was called it would return null when possibly the file existed.
- Added summary comments for controllers.

Thanks,  
_Slacquer_  

<br>
<br>

_April 5th, 2019_  **v2.0.10**
- Cleaned up what I did below, kept it all in the entension method.  Also allowed changed RootAssemblyPath to be a list rather than a single file.
- Added RootAssemblyPath to XmlDocumentationExtensions, to allow fixing the below issue.  Its a workaround...
- Issues with xml comments - Seems when another process runs an asseembly that is using our blox, then the xml comments are not being found.  Some error messages have been added to help facilitate why.  However, this is NOT the responsibility of APIBlox to fix.
- Wow didnt add the PreCompile to Compile... \*sadface\*  
- Added a PreCompile and PostCompile events to DynamicControllerFactory, essentially this was done to allow adding additional references prior to compiling.
 
Thanks,  
_Slacquer_  

<br>
<br>

_April 4th, 2019_  **v2.0.4**  
- First bug, added a must be public check for responses but didnt' bother to consider if it was IEnumerable.  Doh!  
- Going to be building with DEBUG on for now, so that SourceLink will hopefully work.
 
Thanks,  
_Slacquer_  

<br>
<br>

_April 4th, 2019_  **v2.0.2**  
- DynamicControllers!  This was a big one, no longer are generic classes being used along with silly conventions.  Classes are generated on the fly and compiled.  Things like swashbuckle have no clue, therefore just work correctly out of the box!
- Removed PopulateAttribute and its extension methods.
- Removed APIBlox.AspNetCore.Types.Errors namespace, now just APIBlox.AspNetCore.Types
- Removed lots of things...  Mostly with DynamicForms, however I also nuked IPatchCommandHandler and IGenericPatchCommandHandler as they are no longer needed (they were born from dynamic controller input parameter issues).
- AddEnsureResponseResultActionFilter, added onlyQueryActions option, allowing structure to overwrite how all http methods ruturn (IE: POST result gets wrapped like GET).
 
Thanks,  
_Slacquer_  

<br>
<br>

_March 28th, 2019_  **v1.0.119**  
- DynamicController(s) GET and GETALL, added overload for nothing more than a requestObject.
- ValidateEnumerable, there technically is no restriction on the DynamicQueryAllController to prevent you from returning back a NON collection.  Meaninging you should have been using DynamicQueryByController.  This will now check for this and log a warning.
- EventStore, EfCore  
- Added an EFCore repository, this required me to change the EventStoreDocument "Data" property from type Object to string.

Thanks,  
_Slacquer_  

<br>
<br>

_March 22nd, 2019_  **v1.0.118**  
- DynamicControllers  
- Having an issue with swashbuckle and ApiExplorer where operationId's are getting duplicated.

Thanks,  
_Slacquer_  

<br>
<br>


_March 21st, 2019_  **v1.0.117**  
- Added - AddCamelCaseResultsOptions  
- Added - Stacktrace to results on ServerFaultsMiddleware
- Altered - DynamicControllersConvertResponseTypeConvention, now prefixes action names with the controller name, this should help prevent the need to have filters when using swagger.
- Deprecated - AddConsumesProducesJsonResourceResultFilters, it causes issues with version 4 of swagger.
  
Thanks,  
_Slacquer_  

<br>
<br>

_March 20th, 2019_  **v1.0.116**  
- CosmosDbRepository - accidentally left a try catch in code.  
- IReadOnlyEventStoreService - changed storage type of TimeStamp to long.  Will be storing and parsing DateTimeOffset.Now.ToUnixTimeSeconds().  Also added the to and from params for predicates.

Thanks,  
_Slacquer_  

<br>
<br>

_March 19th, 2019_  **v1.0.115**  
- InjectableServices - During assembly lookup we had the following in "GetAbsDirectoryInfos" and should not have
```csharp
// shouldnt be doing this, it disregards use selected paths.  
//absDis.AddRange(di.GetDirectories("*", SearchOption.AllDirectories).Except(absDis));
```
- ReadEventStreamVersionAsync - When null was bombing on return values, now returns (0,"") when emtpy.

Thanks,  
_Slacquer_  

<br>
<br>


_March 18th, 2019_  **v1.0.114**  
- EventStore service(s)
- Added ReadEventStreamVersionAsync, removed includeEvents from ReadEventStreamAsync.

Thanks,  
_Slacquer_  

<br>
<br>

_March 18th, 2019_  **v1.0.113**  
- AssemblyResolver, found a bug where it would return null rather than a cached assembly.
- EventStore exceptions, renamed.

Thanks,  
_Slacquer_  

<br>
<br>

_March 17th, 2019_  **v1.0.112**  
- EventstoreDocument, added DateTimeOffset TimeStamp, stored as a string.

Thanks,  
_Slacquer_  

<br>
<br>

_March 16th, 2019_  **v1.0.111**  
- Removed metadata from WriteToEventStreamAsync

Thanks,  
_Slacquer_  

<br>
<br>

_March 16th, 2019_  **v1.0.110**  
- Potential bug with event store service where you are allowed to write to 
a stream without specifying the version.  When the stream exists, it would 
overwrite the 1st entry.  Now will throw error if stream root exists but 
you have not specified an expectedVersion.

Thanks,  
_Slacquer_  

<br>
<br>

_March 16th, 2019_  **v1.0.109**  
- ServiceCollection extensions had incorrect namespace.

Thanks,  
_Slacquer_  

<br>
<br>

_March 16th, 2019_  **v1.0.108**  
- Various package updates.

Thanks,  
_Slacquer_  

<br>
<br>

_March 16th, 2019_  **v1.0.107**  
- MongoDb, 
- Added indexing to configuration options.
- CosmosDb
- Added indexes for streamId and documentType by default.
- RavenDb
- New to solution.   

Thanks,  
_Slacquer_  

<br>
<br>

_March 15th, 2019_  **v1.0.106**  
- Event Sourcing!, 
- Created an backend agnostic event sourcing library.  So far I have 
CosmosDb and MongoDb repositories that can be used.
Sadly, I have not had the patience to deal with MOCKing the repositories, so the unit tests actually talk to MongoDB and CosmosDB (locally)
- Injectable and InvertedConfiguration extension methods:  
 2 new methods that will only dig through the current AppDomain.  
IE: _"I'm too lazy to really follow the rules so lets just add references to the net core API application!"_

Thanks,  
_Slacquer_  

<br>
<br>

_March 8th, 2019_  **v1.0.105**  
- PostAcceptedController, 
- When doing event sourcing for example, it would be common practice
to not be able to set a location header (the ID created by some orchestration
layer for a particular read model would be unknown to the command handler),
therefore it would be necessary to return a Accepted http status (202).

Thanks,  
_Slacquer_  

<br>
<br>

_March 8th, 2019_  **v1.0.104**  
- PostController, 
- ID equals failed when NOT an int, duh.

Thanks,  
_Slacquer_  

<br>
<br>

_March 8th, 2019_  **v1.0.103**  
- IDomainEventHandler, 
- added async

Thanks,  
_Slacquer_  

<br>
<br>

_March 8th, 2019_  **v1.0.102**  
- IDomainEventHandler, 
- added CancellationToken

Thanks,  
_Slacquer_  

<br>
<br>

_March 8th, 2019_  **v1.0.101**  
- DomainEvents, 
- removed AggregateId from IDomainEvent

Thanks,  
_Slacquer_  

<br>
<br>

_January 17th, 2019_  **v1.0.100**  
- DotNetCore 2.2 Upgrade  

Thanks,  
_Slacquer_  

<br>
<br>

_December 6th, 2018_  **v1.0.99**  
- IDynamicController contract(s),  
- DynamicQueryByController and DynamicQueryAllController,  
...Turns out sometimes you don't need to return an ID with a response, IE an aggregate 
response that has no id to speak of...  

Thanks,  
_Slacquer_  

<br>
<br>

_December 5th, 2018_  **v1.0.98**  
- Patch Controller(s),  
...Up until now had not actually used them, they did not handle any request data 
(other than a patch document) very well...  

Thanks,  
_Slacquer_  

<br>
<br>


_November 22nd, 2018_  **v1.0.97**  
- EnsureResponseResultActionFilter,  
...Tests for IEnumberable but would fail on strings (treated them as an 
array and shouldn't have)...  

- Exampls,  
...Added CQRS example...  

Thanks,  
_Slacquer_  

<br>
<br>



_November 15th, 2018_  **v1.0.96**  
- Sln,  
...Turned off explicit RuntimeFrameworkVersion in project files And explicity set 
AspNetCoreApp to lowerbound 2.1.*...  

Thanks,  
_Slacquer_  

<br>
<br>

_November 15th, 2018_  **v1.0.95**  
- Sln,  
...Just a whole new level of dumb, why is versioning sucha pita.  I made the mistake of 
upgrading to 2.1.6 and now nothing builds correctly in VSTS, when I finally get it to build 
correctly in VSTS, then nothing works in Azure app services becausse 2.1.6 doesnt 
exist.  Oh buy hey theres an extension update for 2.2, which is completely useless, 
where is the support for 2.1.6?!?!...  

Thanks,  
_Slacquer_  

<br>
<br>

_November 15th, 2018_  **v1.0.93**  
- Sln,  
...Changing versions to use lowerbound...  

Thanks,  
_Slacquer_  

<br>
<br>

_November 14th, 2018_  **v1.0.92**  
- NetCore 2.1.5 & AspNetCore 2.1.6,  
...Updated...  

Thanks,  
_Slacquer_  

<br>
<br>

_November 12th, 2018_  **v1.0.91**  
- ServerFaultsMiddleware,  
...v1.0.90, inccorect, the problem was not the content-type, it was the fact we only wrote a 
reponse as a string but didn't take into consideration anything else.  Therefore we are now 
converting our errors to ObjectResults and writing those with the help of the 
WriteResultExecutorAsync extension method.  This means we removed the 
_AddApplicationJsonAsProblemResultContentType_ extension method...  

Thanks,  
_Slacquer_  

<br>
<br>

_November 12th, 2018_  **v1.0.90**  
- ServerFaultsMiddleware & ProblemResult,  
...Seems there may be an issue with Angular 5 and the application/problem+json content-type.  So 
I have added the ability to force both the middleware and the actionresult to use application/json 
when using the AddApplicationJsonAsProblemResultContentType extension method...  

Thanks,  
_Slacquer_  

<br>
<br>

_November 12th, 2018_  **v1.0.89**  
- ServerFaultsMiddleware,  
...When handled by soemthing else, the middleware was NOT setting response headers to problem result...  

Thanks,  
_Slacquer_  

<br>
<br>

_November 12th, 2018_  **v1.0.88**  
- Pagination,  
...Simplified, Simplified, Simplified.  Removed all alias output, 
this was just overkill and was not being used in a practical sense, 
unnecessarily overcomplicated the code.  Inputs can still be aliased, 
but return property names will always be the same.  Also removed the json 
bits for processing alias input...  

- FilteredQuery,  
...Added another query, along with this added several contacts for queries...  

- Examples,  
...I have yet to continue with them but I did alter them for the changes 
in the last few releases...  

Thanks,  
_Slacquer_  

<br>
<br>

_November 10th, 2018_  **v1.0.87**  
- OrderedQuery,  
...Added this for times when pagination and filtering are unnecessary....  


Thanks,  
_Slacquer_  

<br>
<br>

_November 10th, 2018_  **v1.0.86**  
- Pagination,  
...When results are less than the specified max, we will no longer pass 
anything for next and previous....  


Thanks,  
_Slacquer_  

<br>
<br>

_November 8th, 2018_  **v1.0.84**  
- RequestErrorObject,  
...Added/changed a few of the extension methods...  

- CommonStatusCodes,  
...Added a few codes...  


Thanks,  
_Slacquer_  

<br>
<br>


_October 31th, 2018_  **v1.0.81**  
- DynamicControllers,  
...Changed all queryable controllers to use [FromRoute] 
rather than [FromQuery].  This does force the consumer to be specific in 
their request models (IE: if you expect a value to come from body then 
you muse use [FromBody] on that property).  This plays nicely with 
swashbuckle, however non queryable controllers do not use any attribute 
(FromBody or form), thus attributes are of little significance...  

- PaginationQuery and FilteredPaginationQuery  
...Changed to use [FromRoute] 
on their public properties in accordance with the above entry...  

- ServerFaultsMiddleware,  
...Changed the UNKNOWN message to show exception.message...  

Thanks,  
_Slacquer_  

<br>
<br>


_October 31th, 2018_  **v1.0.80**  
- DynamicDataObjects,  
...Changed DynamicDataObjects default serialization settings to be indented...  

Thanks,  
_Slacquer_  

<br>
<br>


_October 31th, 2018_  **v1.0.79**  
- Dynamic Post Controller,  
...Apparently sometimes responses do not match the request and just have the id populated, 
so I have to dig a little looking for an ID to use in the createdAtRoute....  

- ServerFaultsMiddleware,  
...I have created an HandledRequestException that contains a RequestErrorObject, the ServerFaultsMiddleware will check the current exception, if it is HandledRequestException, it will simply check its status code and set the response to the code and serialize the HandledRequestException.RequestErrorObject and be done...  

Thanks,  
_Slacquer_  

<br>
<br>

_October 31th, 2018_  **v1.0.78**  
- Dynamic Controller action parameters,  
...In regards to previous release **v1.0.76**, The answer may have been staring 
me in the face the whole time.  I did NOT add a TRequest input to any of 
the generic actions, thus requiring me to do all this filter adding BS.  
I have now added it, the theory is that with it now having a input parameter 
like  _**[FromQuery] TRequest request**_ then things like APIExplorer, 
Swashbuckle etc will pick up on the attribute(s) in the models 
themselves!....  

Thanks,  
_Slacquer_  

<br>
<br>

_October 30th, 2018_  **v1.0.77**  
- ServerErrorFaults,  
...Added ab verboseProduction switch to allow seeing whats going on 
even in production (IE: you release your software to dev in production 
mode, you may want to actually see whats wrong!)....  

Thanks,  
_Slacquer_  

<br>
<br>


_October 30th, 2018_  **v1.0.76**  
- HELP ME!,  
...I have implemented a rediculous way of handling required inputs, 
I need help figuring this out....  

Thanks,  
_Slacquer_  

<br>
<br>


_October 30th, 2018_  **v1.0.75**  
- AssemblyLoader,  
...Previous statemnt is untrue, had to put it back as when used in 
published apps (all assemblies in one folder) for whatever reason 
SOME referenced assemblies never get loaded....  

Thanks,  
_Slacquer_  

<br>
<br>

_October 30th, 2018_  **v1.0.74**  
- AssemblyLoader,  
...Was being used incorrectly by the AddInjectableServices extension, 
it should have new'd it up only once...  

Thanks,  
_Slacquer_  

<br>
<br>

_October 29th, 2018_  **v1.0.73**  
- Pagination,  
...Would return a next value when the array was empty, now it will be null...  
...Would return default query param names but should not have.  IE: consumer 
sends $Limit, pagination always returned Top.  Now it will send whatever 
the consumer uses, as long as it's in the known map list See 
APIBlox.ApsNetCore.Types.PaginationQuery.PaginationMap for map information.
 
- Serializtion  
...Added an overload to simple mapper, specifying settings for both serialize 
and deserialize...  

Thanks,  
_Slacquer_  

<br>
<br>


_October 28th, 2018_  **v1.0.72**  
- Couple of issues,  
...Extension method mishap, looking at implementation types rather than 
the contracts.  Also changed caching for assembly loader to use just the 
file name rather than full path...
 
Thanks,  
_Slacquer_  

<br>
<br>

_October 27th, 2018_  **v1.0.69**
- A simple pagination issue,  
...The contract reolver was not being injected for alias mapping, therefore 
when 
pagination entries came in the request, unless they were an exact match 
IE: SKIP not 
$SKIP, then they were not being auto populated...
 
Thanks,  
_Slacquer_  

<br>
<br>

_October 27th, 2018_  **v1.0.68**
- A fairly significant bug - continues, **again**   
...After some soul searching I realized I was just bandaiding things, so 
I stopped took a breath and re-wrote most of the ServerFaultsMiddleware....
 
Thanks,  
_Slacquer_  

<br>
<br>

_October 27th, 2018_  **v1.0.49**
- A fairly significant bug - continues  
...Same as below, but it has been bugging me for a while, I didnt think any 
throwing should be going on while the middleware was doing its thing.  
So I have introduced a NoThrow protected internal property 
to allow the faults middlware to tun OFF throwing....
 
Thanks,  
_Slacquer_  

<br>
<br>

_October 27th, 2018_  **v1.0.48**
- A fairly significant bug.  
...*ServerFaultsMiddleware* sets errors to null when NOT in development 
(after logging) to prevent consumers from seeing too much.  However, the  
Base objects were doing a Errors.Any() and not checking for null first.  
Therefore in prod, NOTHING would 
get returned to consumer.  This is nasty and requires a build...
 
Thanks,  
_Slacquer_  

<br>
<br>

_October 23th, 2018_  **v1.0.47**
- A few minor bug fixes have been applied.  
- Also updated the examples, with more still to come.  
 
Thanks,  
_Slacquer_
