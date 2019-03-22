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
