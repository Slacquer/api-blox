[1]: https://github.com/dotnet/sourcelink
[2]: mailto:slacquer2018@gmail.com
[3]: ./response-objects.md
[4]: ../Examples/APIBlox%20%Features
[5]: ../Examples/Dynamic%20%Controllers

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

## Examples
 [General Features][4]    
 [Dynamic Controller(s)][5]  

## Code

<br>


#### Thanks for having a look :)
_My hope is that these packages may help someone other than myself_.  
Thanks,    
Slacquer -[email][2]


Thanks For Icon,
<div>Icons made by <a href="http://www.freepik.com" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a> is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY</a></div>
