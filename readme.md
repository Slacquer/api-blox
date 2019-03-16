[1]: https://github.com/dotnet/sourcelink
[2]: mailto:slacquer2018@gmail.com
[3]: ./response-objects.md
[4]: https://github.com/Slacquer/api-blox/tree/master/Examples/APIBlox%20Features/Examples.Features
[5]: https://github.com/Slacquer/api-blox/tree/master/Examples/Dynamic%20Controllers/Examples.DynamicControllers
[cqrs]: https://github.com/Slacquer/api-blox/tree/master/Examples/CQRS/Examples.Cqrs
[events]: https://github.com/Slacquer/api-blox/tree/master/Examples/Domain%20Events/Examples.DomainEvents

[rn]: ./releaseNotes.md

[sdk]: https://www.microsoft.com/net/download
[logo]: ./logo-blue-large.png

![:)][logo]   
# API Blox

_March 16th, 2019_  **v1.0.107** _[release notes][rn]_ 

<br>

## Packages
 Solution contains the following NuGet packages.  

- APIBlox.AspNetCore  
- APIBlox.AspNetCore.CommandsAndQueries  
- APIBlox.AspNetCore.CommandsQueriesControllersOhMy
- APIBlox.AspNetCore.DynamicControllers  
- APIBlox.NetCore  
- APIBlox.NetCore.Common  
- APIBlox.NetCore.DomainEvents
- APIBlox.NetCore.EventStore
- APIBlox.NetCore.EventStore.CosmosDb
- APIBlox.NetCore.EventStore.MongoDb
- APIBlox.NetCore.EventStore.RavenDb

<br>

## Things to keep in mind  
 
- All packages have [**SourceLink**][1] enabled.  In additon, the packages contain **xml comment** files.  
**TIP** if you have never used the source link features then let me save you some trouble!  
_**As of Visual Studio v15.8.7**_
    1. Add https://nuget.smbsrc.net to **tools\options\debugging\symbols** and make sure its checked, you may want to filter out symbols but for now make sure **Load all modules, unless excluded** is selected.
    2. _UN-CHECK_ **tools\options\debugging\enable just my code**
    3. _CHECK_ **tools\options\debugging\enable source server support**
    4. _CHECK_ **tools\options\debugging\enable source link support**

- _**None**_ of the methodology included requires full blown MVC, you can use the minimalist _**MvcCore**_.  
- I love regions, most devs don't so I have removed them here to keep you happy :)
- This document and the project(s) are a work in progress, feedback and changes would be appreciated but please be kind.
- _Also see_ [_Response Object.docx_][3] for in-depth response information.
- **I know the documentation is lacking, but I will get to at some point**, if you have questions or just need some help, my contact is at the bottom of this document.  

<br>

## Examples
 [APIBlox Features][4]  
 [CQRS][cqrs]  
 [Domain Events][events]  
 [Dynamic Controller(s)][5]  

<br>

## Thanks for having a look :)
_My hope is that these packages may help someone other than myself_.  
Thanks,    
Slacquer -[email][2]


Thanks For Icon,
<div>Icons made by <a href="http://www.freepik.com" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a> is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY</a></div>
